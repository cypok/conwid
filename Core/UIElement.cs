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
        private Rectangle area;
        public Rectangle Area
        {
            get { return area; }
            set
            {
                if(Parent != null)
                {
                    Parent.Invalidate(this.area);
                    Parent.Invalidate(value);
                }
                this.area = value;
            }
        }
        public Size Size
        {
            get { return Area.Size; }
            set { Area = new Rectangle(Area.Location, value); }
        }
        public Point Location
        {
            get { return Area.Location; }
            set { Area = new Rectangle(value, Area.Size); }
        }   

        public bool IsEnabled { get; protected set; }
        
        public UIElement(Rectangle area)
        {
            if(area == null)
                throw new ArgumentNullException();

            this.area = area;
            IsEnabled = false;
        }
        
        abstract public void Handle(IMessage msg);

        abstract public void Invalidate(Rectangle? rect = null);
        abstract public void Draw(DrawSpace ds);
    }
}
