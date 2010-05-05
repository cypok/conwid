using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid
{
    namespace Core
    {
        public class DrawSpace
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

            private void Init(Rectangle allowed, IEnumerable<Rectangle> denied)
            {
                if(allowed == null || denied == null)
                    throw new ArgumentNullException();

                allowedRect = allowed;
                deniedRects = denied;
            }

            public DrawSpace(Rectangle allowed, IEnumerable<Rectangle> denied)
            {
                Init(allowed, denied);
            }

            public DrawSpace(Rectangle allowed)
            {
                Init(allowed, new Rectangle[0]);
            }

            #endregion Constructors

            #region Drawing Methods

            public void PutCharacter(Point point, char ch)
            {
                if(point == null)
                    return;

                point.Offset( allowedRect.Location );
                if( allowedRect.Contains(point) && ! deniedRects.Any( x => x.Contains(point) ) )
                {
                    Console.SetCursorPosition(point.X, point.Y);
                    Console.Write(ch);
                }
            }

            private delegate Size IntToSize(int i);

            private void Line(Point from, int length, string pattern, IntToSize intToSize)
            {
                if( intToSize == null || pattern == null)
                    return;

                for(int i = 0; i < length; ++i)
                {
                    PutCharacter(from + intToSize(i), pattern[i % pattern.Length]);
                }
            }
            
            public void HorisontalLine(Point from, int length, string pattern)
            {
                Line(from, length, pattern, i => new Size(i, 0));
            }

            public void VerticalLine(Point from, int length, string pattern)
            {
                Line(from, length, pattern, i => new Size(0, i));
            }

            public void Rectangle(Rectangle rect, string pattern)
            {
                // TODO: better alternation
                VerticalLine(rect.Location, rect.Height, pattern);
                VerticalLine(rect.Location + new Size(rect.Width-1, 0), rect.Height, pattern);
                HorisontalLine(rect.Location, rect.Width, pattern);
                HorisontalLine(rect.Location + new Size(0, rect.Height-1), rect.Width, pattern);
            }

            #endregion Drawing Methods
        }
    }
}
