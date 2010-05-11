using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    public sealed class DrawSpace
    {
        #region Fields & Properties

        private Rectangle allowedRect;
        private IEnumerable<Rectangle> deniedRects;

        public Size Size
        {
            get { return allowedRect.Size; }
        }

        #endregion Fields & Properties 
            
        #region Constructors
            
        public DrawSpace(Rectangle allowed, IEnumerable<Rectangle> denied = null)
        {
            if(allowed == null)
                throw new ArgumentNullException();

            allowedRect = allowed;
            deniedRects = denied != null ? denied : new Rectangle[0];
        }

        #endregion Constructors

        #region Drawing Methods

        public void PutCharacter(Point point, char ch)
        {
            point.Offset( allowedRect.Location );
            if( allowedRect.Contains(point) && deniedRects.All( x => ! x.Contains(point) ) )
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write(ch);
            }
        }

        public void DrawLine(Point from, Point to, string pattern)
        {
            if( pattern == null )
                throw new ArgumentNullException();

            Func<int, Size> intToSize;
            int diff;
            if( from.Y == to.Y )
            {
                diff = to.X - from.X;
                intToSize = i => new Size(i*Math.Sign(diff), 0);
            }
            else if( from.X == to.X)
            {
                diff = to.Y - from.Y;
                intToSize = i => new Size(0, i*Math.Sign(diff));
            }
            else
                throw new ArgumentException("bad type of line");

            for(int i = 0; i < Math.Abs(diff) + 1; ++i)
            {
                PutCharacter(from + intToSize(i), pattern[i % pattern.Length]);
            }
        }

        public void PutString(Point from, string str)
        {
            DrawLine(from, from + new Size(str.Length-1,0), str);
        }

        public void DrawRectangle(Rectangle rect, string pattern)
        {
            if( pattern == null )
                throw new ArgumentNullException();

            // TODO: better alternation
            DrawLine(new Point(rect.Left,    rect.Top     ), new Point(rect.Right-1, rect.Top     ), pattern);
            DrawLine(new Point(rect.Right-1, rect.Top     ), new Point(rect.Right-1, rect.Bottom-1), pattern);
            DrawLine(new Point(rect.Right-1, rect.Bottom-1), new Point(rect.Left,    rect.Bottom-1), pattern);
            DrawLine(new Point(rect.Left,    rect.Bottom-1), new Point(rect.Left,    rect.Top     ), pattern);
        }

        #endregion Drawing Methods
    }
}

