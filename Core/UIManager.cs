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
            if (parent == null)
                throw new ArgumentNullException("parent_");

            Parent = parent;

            this.title = title;

            nextElementKeyInfo = nextElemKeyInfo ?? NormalNextElementKeyInfo;
            prevElementKeyInfo = prevElemKeyInfo ?? NormalPrevElementKeyInfo;
        }

        #endregion // Constructors

        #region Collection Helpers

        private IEnumerable<Child> LowerElements(Child c)
        {
            return children.FindAll(x => children.IndexOf(x) > children.IndexOf(c));
        }
        private IEnumerable<Child> UpperElements(Child c)
        {
            return children.FindAll(x => children.IndexOf(x) < children.IndexOf(c));
        }
        public Child ActiveElement
        {
            get { return children.FirstOrDefault(); }
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
                    children.Insert(0, e);
                    // TODO: invalidate only affected area
                    e.Invalidate();
                }
                else if(msg is RemoveUIElementMessage<Child>)
                {
                    children.Remove(e);
                    // TODO: invalidate only affected area
                    Invalidate();
                }
                else if(msg is InvalidateUIElementMessage<Child>)
                {
                    Rectangle? rect = (msg as InvalidateUIElementMessage<Child>).Rect;
                    Rectangle actualInvalidRect = rect ?? new Rectangle(Point.Empty, e.Size);
                    actualInvalidRect.Offset(e.Area.Location);

                    if(Parent != null)
                    {
                        Invalidate(actualInvalidRect);
                    }
                    else
                    {
                        // else I'm the big boss :)
                        DrawChild(e, DrawSpace.Screen.Restrict(actualInvalidRect));
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
                else if(ActiveElement != null)
                {
                    ActiveElement.SendMessage(msg);
                }
                // and if can't handle key and have no one to forward it to - nothing to do :(
            }
            else if(msg is SwitchUIElementMessage && !children.IsEmpty())
            {
                if((msg as SwitchUIElementMessage).Next)
                    children.MoveToEnding(0);
                else
                    children.MoveToBeginning(children.Count - 1);

                foreach (var e in children)
                    e.Invalidate();

            }
            else if(msg is GlobalRedrawMessage)
            {
                if(Parent != null)
                    throw new InvalidOperationException("GlobalRedrawMessage");
                Draw(DrawSpace.Screen);
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
                if (value is UIManager<UIManager<Child>>)
                {
                    var newParent = value as UIManager<UIManager<Child>>;

                    if(newParent == null)
                        throw new ArgumentNullException();

                    if(newParent == parent) // nothing changed
                        return;

                    // remove itself from previous parent
                    if (parent != null)
                        parent.SendMessage(new RemoveUIElementMessage<UIManager<Child>>(this));

                    parent = newParent;
                    parent.SendMessage(new AddUIElementMessage<UIManager<Child>>(this));
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
            // TODO: Choose affected
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
                this.PostMessage(new GlobalRedrawMessage());
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
