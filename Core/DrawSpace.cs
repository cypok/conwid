using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    public class Color
    {
        public ConsoleColor Foreground { get; set; }
        public ConsoleColor Background { get; set; }
    }

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

        #region Colors

        public Color Color { get; set; }

        public ConsoleColor ForegroundColor
        {
            get { return Color.Foreground; }
            set { Color.Foreground = value; }
        }

        public ConsoleColor BackgroundColor
        {
            get { return Color.Background; }
            set { Color.Background = value; }
        }

        #endregion Colors

        #region Drawing Methods

        public void PutCharacter(Point point, char ch)
        {
            point.Offset( allowedRect.Location );
            if( allowedRect.Contains(point) && deniedRects.All( x => ! x.Contains(point) ) )
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.ForegroundColor = Color.Foreground;
                Console.BackgroundColor = Color.Background;
                Console.Write(ch);
                Console.SetCursorPosition(0, 0);
            }
        }

        public void DrawPatternLine(Point from, Point to, string pattern)
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

        public void DrawLine(Point from, Point to, char ch)
        {
            DrawPatternLine(from, to, ch.ToString());
        }

        public void PutString(Point from, string str)
        {
            DrawPatternLine(from, from + new Size(str.Length-1,0), str);
        }

        public void DrawRectangle(Rectangle rect, char ch)
        {
            DrawLine(new Point(rect.Left,    rect.Top     ), new Point(rect.Right-1, rect.Top     ), ch);
            DrawLine(new Point(rect.Right-1, rect.Top     ), new Point(rect.Right-1, rect.Bottom-1), ch);
            DrawLine(new Point(rect.Right-1, rect.Bottom-1), new Point(rect.Left,    rect.Bottom-1), ch);
            DrawLine(new Point(rect.Left,    rect.Bottom-1), new Point(rect.Left,    rect.Top     ), ch);
        }

        public void FillRectangle(Rectangle rect, char ch)
        {
            for(int x = rect.Left; x < rect.Right; x++)
                for(int y = rect.Top; y < rect.Bottom; y++)
                    PutCharacter(new Point(x,y), ch);
        }

        #endregion Drawing Methods
    }
}

