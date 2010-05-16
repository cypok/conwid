using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    using Messages;

    public abstract class Widget : IMessageHandler
    {
        #region Fields & Properties

        // TODO: setter notifying WidgetManager about resizing
        public Rectangle Area { get; protected set; }
        public Size Size
        {
            get { return Area.Size; }
            protected set { Area = new Rectangle(Area.Location, value); }
        }

        #endregion // Fields & Properties

        #region Events

        #endregion // Events

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

        protected void Emit(MulticastDelegate d, params object[] objs)
        {
            if(d != null)
                d.DynamicInvoke(objs);
        }

        public void Invalidate()
        {
            WidgetManager.Instance.PostMessage(new RedrawWidgetMessage(this));
        }

        abstract public void Handle(IMessage msg);
        abstract public void Draw(DrawSpace ds);
    }
}

