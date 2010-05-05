using System;
using Conwid.Core;
using System.Drawing;

namespace Demo
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var allowedRect = new Rectangle( 10, 10, 20, 10 );
            var ds = new DrawSpace(allowedRect);
            ds.PutCharacter( new Point(0,0), '<' );
            ds.PutCharacter( new Point(10,5), '*' );
            ds.PutCharacter( new Point(21,5), '!');
            ds.PutCharacter( new Point(5,21), '!');
            ds.PutCharacter( new Point(-5,-5), '!');

            ds.HorisontalLine( new Point(2,3), 10, "123" );
            ds.HorisontalLine( new Point(-2,7), 40, "*" );

            ds.VerticalLine( new Point (18,0), ds.Size.Height, "$" );
            ds.VerticalLine( new Point (1,1), 0, "?!" );
            ds.VerticalLine( new Point (13,1), 4, "yes!" );

            ds.Rectangle( new Rectangle( Point.Empty, ds.Size ), "#");
            ds.Rectangle( new Rectangle( 10, 6, 6, 3 ), ",.");

            ds.PutCharacter( new Point(19,9), '>');
        }
    }
}
