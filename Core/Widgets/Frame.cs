using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    public class Frame : Widget
    {
        readonly Color ActiveFrameColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveFrameColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };

        public Frame(Rectangle area) : base(area) {}

        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveFrameColor : InactiveFrameColor;

            ds.DrawRectangle(new Rectangle(Point.Empty, Size), '+');
            ds.FillRectangle(new Rectangle(new Point(1,1), Size - new Size(2,2)), ' ');
        }
    }
}