using System;
using Conwid.Core;
using System.Drawing;

namespace Demo
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var bad_rects = new Rectangle[]{
                new Rectangle(15,15,20,20),
                new Rectangle(0,0,11,11),
            };

            var ds = new DrawSpace(new Rectangle( 10, 10, 20, 10 ), bad_rects);

            ds.DrawRectangle(new Rectangle(Point.Empty, ds.Size), "+");

            ds.PutString(new Point(1,1), "X O _ Application");
            ds.DrawLine(new Point(1,2), new Point(ds.Size.Width-2,2), "-");

            ds.PutCharacter( new Point(19,9), '>');
        }
    }
}
