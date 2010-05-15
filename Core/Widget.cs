using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    public abstract class Widget : IMessageHandler
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
            // TODO: ask parent
            return WidgetManager.Instance.ActiveWidget == this;
        }

        abstract public void Handle(IMessage msg);
        abstract public void Draw(DrawSpace ds);
    }
}

