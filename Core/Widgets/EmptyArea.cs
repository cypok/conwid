using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    class EmptyArea : Widget
    {
        public EmptyArea(Rectangle area) : base(area) {}

        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.FillRectangle( new Rectangle(Point.Empty, Size), ' ' );
        }
    }
}
