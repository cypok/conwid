using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    public class Frame : Widget
    {
        public Frame(Rectangle pos) : base(pos) {}

        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.DrawRectangle(new Rectangle(Point.Empty, ds.Size), "+");
        }
    }
}