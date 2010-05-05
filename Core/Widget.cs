using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid
{
    namespace Core
    {
        abstract class Widget : IMessageHandler
        {
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
}
