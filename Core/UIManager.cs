using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace Conwid.Core
{
    using Messages;
    using Widgets;

    public sealed class UIManager<Child> : UIElement
        where Child : UIElement
    {
        #region Constants

        public static readonly ConsoleKeyInfo TopLevelNextElementKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Tab, control: true, shift: false, alt: false);
        public static readonly ConsoleKeyInfo TopLevelPrevElementKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Tab, control: true, shift: true, alt: false);

        public static readonly ConsoleKeyInfo NormalNextElementKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Tab, control: false, shift: false, alt: false);
        public static readonly ConsoleKeyInfo NormalPrevElementKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Tab, control: false, shift: true, alt: false);

        public static readonly ConsoleKeyInfo CloseKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.W, control: true, shift: false, alt: false);

        readonly Color ActiveFrameColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveFrameColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };


        #endregion // Constants

        #region Fields and Properties
       
        private List<Child> children = new List<Child>();
        UIManager<UIManager<Child>> parent;

        private List<int> children_order = new List<int>();

        public Child ActiveElement
        {
            get
            {
                if( children_order.IsEmpty() )
                    return null;
                var i = children_order.First();
                return children[i];
            }
        }

        private static UIElement topLevelManager; // used to avoid creation of more than one top-level UIManager

        private ConsoleKeyInfo nextElementKeyInfo;
        private ConsoleKeyInfo prevElementKeyInfo;

        public string Title { get; private set; }
        
        #endregion // Fields and Properties

        #region Constructors
        
        /// <summary>
        /// Creates top-level UIManager
        /// </summary>
        internal UIManager(ConsoleKeyInfo? nextElemKeyInfo = null, ConsoleKeyInfo? prevElemKeyInfo = null)
            : base( new Rectangle(Point.Empty, DrawSpace.Screen.Size) )
        {
            if (topLevelManager != null)
                throw new InvalidOperationException("Only one top-level UIManager allowed");

            topLevelManager = this;
            parent = null;
            IsEnabled = true;

            nextElementKeyInfo = nextElemKeyInfo ?? TopLevelNextElementKeyInfo;
            prevElementKeyInfo = prevElemKeyInfo ?? TopLevelPrevElementKeyInfo;
        }

        /// <summary>
        /// Creates non-top-level UIManager
        /// </summary>
        public UIManager(UIManager<UIManager<Child>> parent_, Rectangle area, string title="",
                         ConsoleKeyInfo? nextElemKeyInfo = null, ConsoleKeyInfo? prevElemKeyInfo = null)
            : base(area)
        {
            Parent = parent_;

            Title = title;

            nextElementKeyInfo = nextElemKeyInfo ?? NormalNextElementKeyInfo;
            prevElementKeyInfo = prevElemKeyInfo ?? NormalPrevElementKeyInfo;
        }

        #endregion // Constructors

        #region Collection Helpers

        private IEnumerable<Child> LowerElements(Child c)
        {
            var ind = children.IndexOf(c);
            return children_order.FindAll(i => children_order.IndexOf(i) > children_order.IndexOf(ind))
                                 .Select(i => children[i]);
        }
        private IEnumerable<Child> UpperElements(Child c)
        {
            var ind = children.IndexOf(c);
            return children_order.FindAll(i => children_order.IndexOf(i) < children_order.IndexOf(ind))
                                 .Select(i => children[i]);
        }
        #endregion // Collection Helpers

        #region Message Handling (Main functionality here)

        // Handles:
        // * AddUIElementMessage<Element>
        // * RemoveUIElementMessage<Element>
        // * RedrawUIElementMessage<Element>
        // * SwitchUIElementMessage
        // * KeyPressedMessage
        // * GlobalRedrawMessage (valid only if UIManager is top-level)
        public override void Handle(IMessage msg)
        {
            if(msg is UIElementMessage<Child>)
            {
                var e = (msg as UIElementMessage<Child>).UIElement;
                
                
                if(msg is AddUIElementMessage<Child>)
                {
                    if(children.Contains(e))
                        throw new InvalidOperationException("Element already added to UIManager");

                    var oldActive = ActiveElement;

                    children.Add(e);
                    children_order.Insert(0, children.IndexOf(e));

                    if( oldActive != null)
                        oldActive.Invalidate();
                    e.Invalidate();
                }
                else if(msg is RemoveUIElementMessage<Child>)
                {
                    children_order.Remove(children.IndexOf(e));
                    children.Remove(e);

                    e.Invalidate();
                    if( ActiveElement != null)
                        ActiveElement.Invalidate();
                }
                else if(msg is InvalidateUIElementMessage<Child>)
                {
                    Rectangle? rect = (msg as InvalidateUIElementMessage<Child>).Rect;
                    Rectangle actualInvalidRect = rect ?? new Rectangle(Point.Empty, e.Size);
                    actualInvalidRect.Offset(e.Area.Location);
                    {
                        Invalidate(actualInvalidRect);
                    }
                }
            }
            else if(msg is KeyPressedMessage)
            {
                var keyInfo = (msg as KeyPressedMessage).KeyInfo;

                if(keyInfo.EqualsTo(nextElementKeyInfo))
                {
                    this.SendMessage(new SwitchUIElementMessage(next: true));
                }
                else if(keyInfo.EqualsTo(prevElementKeyInfo))
                {
                    this.SendMessage(new SwitchUIElementMessage(next: false));
                }
                else if(keyInfo.EqualsTo(CloseKeyInfo) && Parent != null)
                {
                    // Close it, if it is not top-level
                    Parent = null;
                }
                else if(ActiveElement != null)
                {
                    ActiveElement.SendMessage(msg);
                }
                // and if can't handle key and have no one to forward it to - nothing to do :(
            }
            else if(msg is SwitchUIElementMessage && !children.IsEmpty())
            {
                var oldActiveIndex = children_order[0];
                int newActiveIndex;
                if((msg as SwitchUIElementMessage).Next)
                {
                    newActiveIndex = (oldActiveIndex + 1)%children_order.Count;
                }
                else
                {
                    newActiveIndex = (oldActiveIndex - 1 + children_order.Count)%children_order.Count; // additional `+ children_order.Count` needed to exclude negative numbers
                }
                
                children_order.MoveToBeginning( children_order.IndexOf(newActiveIndex) );

                children[oldActiveIndex].Invalidate();
                children[newActiveIndex].Invalidate();
            }
            else if(msg is GlobalRedrawMessage)
            {
                if(Parent != null)
                    throw new InvalidOperationException("Only top-level UIManagers can handle GlobalRedrawMessage");
                var rect = (msg as GlobalRedrawMessage).Rect;
                var ds = DrawSpace.Screen;
                if(rect!=null)
                    ds = ds.Restrict(rect ?? Rectangle.Empty);
                Draw(ds);
            }
        }

        #endregion // Message Handling

        #region IUIElement Properties & Methods
        
        public override UIElement Parent
        {
            get { return parent; }
            set
            {
                // copypaste from Widget, but how to do it otherwise? :(
                if (value == null || value is UIManager<UIManager<Child>>)
                {
                    var newParent = value as UIManager<UIManager<Child>>;

                    if(newParent == parent) // nothing changed
                        return;

                    // remove itself from previous parent
                    if (parent != null)
                        parent.SendMessage(new RemoveUIElementMessage<UIManager<Child>>(this));

                    parent = newParent;
                    if(parent != null)
                    {
                        parent.SendMessage(new AddUIElementMessage<UIManager<Child>>(this));
                        IsEnabled = true;
                    }
                    else
                    {
                        IsEnabled = false;
                    }
                }
                else
                {
                    throw new InvalidCastException("Only UIManager<UIManager<Element>> can be a parent of a UIManager<Element>");
                }
            }
        }
        
        private void DrawChild(Child c, DrawSpace ds)
        {
            if(!children.Contains(c))
                throw new InvalidOperationException("Element not added to UIManager being redrawed");
            c.Draw( ds.CreateSubSpace(c.Area, UpperElements(c).Select(x => x.Area)) );
        }

        public bool IsActive()
        {
            if(parent == null)
                return true;
            return parent.ActiveElement == this && parent.IsActive();
        }

        public override void Draw(DrawSpace ds)
        {
            var childrenToRedraw = children.FindAll( c => ds.IsAffecting(c.Area) );

            var bgDS = ds.CreateSubSpace(null, childrenToRedraw.Select(x => x.Area));
            if(Parent == null)
            {
                // Top-level drawing
                bgDS.FillRectangle( new Rectangle(Point.Empty, Area.Size), ' ' );
            }
            else
            {
                // Normal drawing
                bgDS.Color = IsActive() ? ActiveFrameColor : InactiveFrameColor;

                var rect = new Rectangle(Point.Empty, Size);
                bgDS.DrawBorder(rect, DrawSpace.DoubleBorder, Title);

                //
                bgDS.FillRectangle(new Rectangle(new Point(1,1), Size - new Size(2,2)), ' ');
            }
            foreach(var c in childrenToRedraw)
                DrawChild(c, ds);
        }

        public override void Invalidate(Rectangle? rect = null)
        {
            if(Parent != null)
                Parent.PostMessage( new InvalidateUIElementMessage<UIManager<Child>>(this, rect) );
            else
                // else I'm the big boss :)
                this.PostMessage(new GlobalRedrawMessage(rect));
        }

        #endregion // IUIElement Properties & Methods

        #region Debug Methods

        public string DebugDump()
        {
            var s = "";
            s += this.GetType().Name + "\n";
            s += "Elements:\n";
            var strings = children.Select( e => String.Format(" - {0} #{1}", e, e.GetHashCode()) );
            s += String.Join("\n", strings);
            return s;
        }

        #endregion // Debug Methods
    }
}
