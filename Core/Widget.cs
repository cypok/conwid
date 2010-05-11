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
        public Rectangle Position { get; protected set; }

        public Widget(Rectangle pos)
        {
            if(pos == null)
                throw new ArgumentNullException();

            Position = pos;
        }

        abstract public void Handle(IMessage msg);
        abstract public void Draw(DrawSpace ds);
    }
}

