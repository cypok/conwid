using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    public abstract class UIElement : IMessageHandler
    {
        abstract public UIElement Parent { get; set; }
        
        // TODO: setter notifying WidgetManager about moving and resizing
        public Rectangle Area{ get; set; }
        public Size Size
        {
            get { return Area.Size; }
            protected set { Area = new Rectangle(Area.Location, value); }
        }

        public bool IsEnabled { get; protected set; }
        
        public UIElement(Rectangle area)
        {
            if(area == null)
                throw new ArgumentNullException();

            Area = area;
            IsEnabled = false;
        }
        
        abstract public void Handle(IMessage msg);

        abstract public void Invalidate(Rectangle? rect = null);
        abstract public void Draw(DrawSpace ds);
    }
}
