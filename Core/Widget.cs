using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    using Messages;
    public abstract class Widget : IUIElement
    {
        // TODO: setter notifying WidgetManager about resizing
        public Rectangle Area { get; protected set; }
        public Size Size
        {
            get { return Area.Size; }
            protected set { Area = new Rectangle(Area.Location, value); }
        }

        public Widget(Rectangle area)
        {
            if(area == null)
                throw new ArgumentNullException();

            Area = area;
        }

        public bool IsActive()
        {
            if(parent == null)
                return false;
            return parent.ActiveElement == this;
        }

        abstract public void Handle(IMessage msg);
        abstract public void Draw(DrawSpace ds);

        UIManager<Widget> parent;
        public IUIElement Parent
        {
            get { return parent; }
            set
            {
                if(value is UIManager<Widget>)
                {
                    var newParent = value as UIManager<Widget>;

                    if(newParent == parent) // nothing changed
                        return;
                
                    // remove itself from previous parent
                    if(parent != null)
                        parent.SendMessage( new RemoveUIElementMessage<Widget>(this) );
                
                    parent = newParent;
                    if(parent != null)
                        parent.SendMessage( new AddUIElementMessage<Widget>(this) );
                }
                else
                {
                    throw new InvalidCastException("Only UIManager<Widget> can be a parent of a Widget");
                }
            }
        }

        public void Invalidate()
        {
            Parent.PostMessage( new RedrawUIElementMessage<Widget>(this) );
        }
    }
}

