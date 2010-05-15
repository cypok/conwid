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
        public string Title { get; private set; }

        public Frame(Rectangle area, string title = "") : base(area)
        {
            Title = title;
        }

        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            //   ╔Title═══╗
            //   ║        ║
            //   ╚════════╝

            ds.Color = IsActive() ? ActiveFrameColor : InactiveFrameColor;

            var rect = new Rectangle(Point.Empty, Size);
            
            var maxTitleLength = rect.Width - 2;
            var title = Title;
            if( Title.Length > maxTitleLength )
                title = Title.Substring(0, maxTitleLength-1) + "~";
            ds.PutString(new Point(rect.Left+1, rect.Top), title);

            ds.PutCharacter(new Point(rect.Left,    rect.Top     ), '╔');
            ds.PutCharacter(new Point(rect.Right-1, rect.Top     ), '╗');
            ds.PutCharacter(new Point(rect.Right-1, rect.Bottom-1), '╝');
            ds.PutCharacter(new Point(rect.Left,    rect.Bottom-1), '╚');
            if( title.Length < maxTitleLength )
                ds.DrawLine(new Point(rect.Left+1+title.Length,  rect.Top), new Point(rect.Right-2, rect.Top     ), '═');
            ds.DrawLine(new Point(rect.Right-1, rect.Top+1   ), new Point(rect.Right-1, rect.Bottom-2), '║');
            ds.DrawLine(new Point(rect.Right-2, rect.Bottom-1), new Point(rect.Left+1,  rect.Bottom-1), '═');
            ds.DrawLine(new Point(rect.Left,    rect.Bottom-2), new Point(rect.Left,    rect.Top+1   ), '║');


            ds.FillRectangle(new Rectangle(new Point(1,1), Size - new Size(2,2)), ' ');
        }
    }
}