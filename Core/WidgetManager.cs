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

    // TODO: rename file to UIManager.cs
    public sealed class UIManager<Child> : IUIElement
        where Child : IUIElement
    {
        static IUIElement topLevelManager;

        readonly ConsoleKeyInfo NextWidgetKeyInfo =     new ConsoleKeyInfo('_', ConsoleKey.Tab, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo PreviousWidgetKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Tab, control: false, shift: true, alt: false);

        #region Fields and Properties

        private List<Child> children = new List<Child>();

        public Widget BackgroundWidget { get; private set; }
        public Rectangle Area { get { return BackgroundWidget.Area; } }

        UIManager<UIManager<Child>> parent;
        public IUIElement Parent
        {
            get { return parent; }
            set
            {
                // copypaste from Widget, but how to do it otherwise? :(
                if (value is UIManager<UIManager<Child>>)
                {
                    var newParent = value as UIManager<UIManager<Child>>;

                    if (newParent == parent) // nothing changed
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

        #endregion // Fields and Properties

        #region Constructors
        
        /// <summary>
        /// Creates top-level UIManager
        /// </summary>
        internal UIManager()
        {
            if (topLevelManager != null)
                throw new InvalidOperationException("Only one top-level UIManager allowed");

            topLevelManager = this;
            parent = null;
            var screenArea = new Rectangle(Point.Empty, DrawSpace.Screen.Size);
            BackgroundWidget = new EmptyArea(screenArea);

        }

        /// <summary>
        /// Creates non-top-level UIManager
        /// </summary>
        public UIManager(Rectangle area, string title, UIManager<UIManager<Child>> parent_)
        {
            if (parent_ == null)
                throw new ArgumentNullException("parent_");

            BackgroundWidget = new Frame(area, title);
            Parent = parent_;
        }

        #endregion // Constructors

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
        
        // Handles:
        // * AddUIElementMessage<Element>
        // * RemoveUIElementMessage<Element>
        // * RedrawUIElementMessage<Element>
        // * SwitchUIElementMessage
        // * KeyPressedMessage
        // * GlobalRedrawMessage (valid only if UIManager is top-level)
        public void Handle(IMessage msg)
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
                else if(msg is RedrawUIElementMessage<Child>)
                {
                    if(Parent != null)
                    {
                        Parent.PostMessage( new RedrawUIElementMessage<UIManager<Child>>(this));
                    }
                    else
                    {
                        // else I'm the big boss :)
                        DrawChild(e, DrawSpace.Screen);
                    }
                }
            }
            else if(msg is KeyPressedMessage)
            {
                var keyInfo = (msg as KeyPressedMessage).KeyInfo;

                if(keyInfo.IsEqualTo(NextWidgetKeyInfo))
                {
                    this.SendMessage(new SwitchUIElementMessage(next: true));
                }
                else if(keyInfo.IsEqualTo(PreviousWidgetKeyInfo))
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
                    this.PostMessage( new RedrawUIElementMessage<Child>(e) );

            }
            else if(msg is GlobalRedrawMessage)
            {
                if(Parent != null)
                    throw new InvalidOperationException("GlobalRedrawMessage");
                Draw(DrawSpace.Screen);
            }
        }

        private void DrawChild(Child c, DrawSpace ds)
        {
            if(!children.Contains(c))
                throw new InvalidOperationException("Element not added to UIManager being redrawed");
            c.Draw( ds.CreateSubSpace(c.Area, UpperElements(c).Select(x => x.Area)) );
        }

        public void Draw(DrawSpace ds)
        {
            BackgroundWidget.Draw(ds);
            // TODO: required rect should be transferred to children somewhere near here
            foreach(var e in children)
                DrawChild(e, ds);
        }

        public void Invalidate()
        {
            if(Parent != null)
                Parent.PostMessage( new RedrawUIElementMessage<UIManager<Child>>(this) );
            else
                this.PostMessage(new GlobalRedrawMessage());
        }

        public string DebugDump()
        {
            var s = "";
            s += this.GetType().Name + "\n";
            s += "Elements:\n";
            var strings = children.Select( e => String.Format(" - {0} #{1}", e, e.GetHashCode()) );
            s += String.Join("\n", strings);
            return s;
        }
    }
}
