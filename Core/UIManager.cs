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

        public static readonly ConsoleKeyInfo UpKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.UpArrow, control: true, shift: false, alt: false);
        public static readonly ConsoleKeyInfo DownKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.DownArrow, control: true, shift: false, alt: false);
        public static readonly ConsoleKeyInfo LeftKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.LeftArrow, control: true, shift: false, alt: false);
        public static readonly ConsoleKeyInfo RightKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.RightArrow, control: true, shift: false, alt: false);

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

        public Rectangle ClientArea { get { return new Rectangle(1,1,Area.Width-2,Area.Height-2); } }
       
        private List<Child> children = new List<Child>();
        UIManager<UIManager<Child>> parent;

        private List<Child> children_order = new List<Child>();

        public Child ActiveElement
        {
            get
            {
                return children_order.FirstOrDefault();
            }
        }

        private static UIElement topLevelManager; // used to avoid creation of more than one top-level UIManager

        private ConsoleKeyInfo nextElementKeyInfo;
        private ConsoleKeyInfo prevElementKeyInfo;
                
        string title;
        public string Title
        { 
            get { return title; }
            set
            {
                title = value;
                Invalidate();
            }
        }

        private static bool firstDraw = true;
        
        #endregion // Fields and Properties

        #region Constructors
        
        /// <summary>
        /// Creates top-level UIManager
        /// </summary>
        internal UIManager(ConsoleKeyInfo? nextElemKeyInfo = null, ConsoleKeyInfo? prevElemKeyInfo = null)
            : base( new Rectangle(Point.Empty, DrawSpace.ScreenSize) )
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
        public UIManager(UIManager<UIManager<Child>> parent, Rectangle area, string title="",
                         ConsoleKeyInfo? nextElemKeyInfo = null, ConsoleKeyInfo? prevElemKeyInfo = null)
            : base(area)
        {
            Parent = parent;

            this.title = title;

            nextElementKeyInfo = nextElemKeyInfo ?? NormalNextElementKeyInfo;
            prevElementKeyInfo = prevElemKeyInfo ?? NormalPrevElementKeyInfo;
        }

        #endregion // Constructors

        #region Collection Helpers

        private IEnumerable<Child> LowerElements(Child c)
        {
            return children_order.Where((x, i) => i > children_order.IndexOf(c));
        }
        private IEnumerable<Child> UpperElements(Child c)
        {
            return children_order.Where((x, i) => i < children_order.IndexOf(c));
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
                    children_order.Add(e);//Insert(0, e);

                    if( oldActive != null)
                        oldActive.Invalidate();
                    e.Invalidate();
                }
                else if(msg is RemoveUIElementMessage<Child>)
                {
                    children_order.Remove(e);
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
                    Invalidate(actualInvalidRect);
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
                // Moving: if not top-level
                else if(keyInfo.EqualsTo(UpKeyInfo) && Parent != null)
                {
                    Location = Location + new Size(0, -1);
                }
                else if(keyInfo.EqualsTo(DownKeyInfo) && Parent != null)
                {
                    Location = Location + new Size(0, 1);
                }
                else if(keyInfo.EqualsTo(LeftKeyInfo) && Parent != null)
                {
                    Location = Location + new Size(-1, 0);
                }
                else if(keyInfo.EqualsTo(RightKeyInfo) && Parent != null)
                {
                    Location = Location + new Size(1, 0);
                }
                else if(ActiveElement != null)
                {
                    ActiveElement.SendMessage(msg);
                }
                // and if can't handle key and have no one to forward it to - nothing to do :(
            }
            else if(msg is SwitchUIElementMessage && !children.IsEmpty())
            {
                var oldActiveIndex = children.IndexOf(children_order[0]);
                int newActiveIndex;
                if((msg as SwitchUIElementMessage).Next)
                {
                    newActiveIndex = (oldActiveIndex + 1)%children_order.Count;
                }
                else
                {
                    newActiveIndex = (oldActiveIndex - 1 + children_order.Count)%children_order.Count; // additional `+ children_order.Count` needed to exclude negative numbers
                }
                
                children_order.MoveToBeginning( children_order.IndexOf(children[newActiveIndex]) );

                children[oldActiveIndex].Invalidate();
                children[newActiveIndex].Invalidate();
            }
            else if(msg is GlobalRedrawMessage)
            {
                if(Parent != null)
                    throw new InvalidOperationException("Only top-level UIManagers can handle GlobalRedrawMessage");
                var rects = (msg as GlobalRedrawMessage).Rects;
                Draw(DrawSpace.Screen.Restrict(rects));
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

        public override bool IsActive
        {
            get
            {
                if(parent == null)
                    return true;
                return parent.ActiveElement == this && parent.IsActive;
            }
        }

        public override void Draw(DrawSpace ds)
        {
            var childrenToRedraw = children.Where( c => ds.IsAffecting(c.Area) );

            var bgDS = ds.CreateSubSpace(null, childrenToRedraw.Select(x => x.Area));
            if(Parent == null)
            {
                // Top-level drawing
                bgDS.FillRectangle( new Rectangle(Point.Empty, Area.Size), ' ' );
            }
            else
            {
                // Normal drawing
                bgDS.Color = IsActive ? ActiveFrameColor : InactiveFrameColor;

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
            {
                // else I'm the big boss :)
                if(firstDraw)
                    rect = null; // on first draw draw itself entirely
                firstDraw = false;
                this.PostMessage(new GlobalRedrawMessage(rect));
            }
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
