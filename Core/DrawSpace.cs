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

        public Color() {}
        public Color(ConsoleColor fg, ConsoleColor bg)
        {
            Foreground = fg;
            Background = bg;
        }
    }

    public sealed class DrawSpace
    {
        #region Constants
        
        public const string SingleBorder = "┌┐┘└─│─│";
        public const string DoubleBorder = "╔╗╝╚═║═║";        

        #endregion // Constants

        #region Fields & Properties

        private Point referencePoint;
        private IEnumerable<Rectangle> allowedRects;
        private IEnumerable<Rectangle> deniedRects;

        public static Size ScreenSize
        {
            get { return new Size(Console.WindowWidth, Console.WindowHeight); }
        }

        #endregion // Fields & Properties

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

        
        readonly Color DefaultColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };

        #endregion // Colors
            
        #region Constructors
            
        public DrawSpace(IEnumerable<Rectangle> allowed, IEnumerable<Rectangle> denied = null, Point? refPoint = null)
        {
            if(allowed == null)
                throw new ArgumentNullException();

            allowedRects = allowed;
            deniedRects = denied ?? new Rectangle[0];
            referencePoint = refPoint ?? Point.Empty;

            Color = DefaultColor;
        }

        #endregion // Constructors

        #region Creating Helpers

        private static DrawSpace screen;
        public static DrawSpace Screen
        {
            get
            {
                if(screen == null)
                {
                    var screenArea = new Rectangle(Point.Empty, ScreenSize);
                    screen = new DrawSpace( new Rectangle[]{screenArea} );
                }
                return screen;
            }
        }

        public DrawSpace CreateSubSpace(Rectangle? allowed, IEnumerable<Rectangle> denied = null)
        {
            // if null given as allowed, it means leave the same allowedRects
            Point refPoint;
            IEnumerable<Rectangle> actualAllowedRects;
            if(allowed != null)
            {
                var actualAllowed = allowed.Value; // take value from Nullable<Rectangle>
                actualAllowed.Offset( this.referencePoint );
                
                refPoint = actualAllowed.Location;
                actualAllowedRects = this.allowedRects.SelectFromCopies( (ref Rectangle x) => x.Intersect(actualAllowed) );
            }
            else
            {
                refPoint = this.referencePoint;
                actualAllowedRects = this.allowedRects;
            }

            var actualDenied = (denied ?? new Rectangle[0]).SelectFromCopies( (ref Rectangle x) => x.Offset(this.referencePoint) );
                
            return new DrawSpace( actualAllowedRects, this.deniedRects.Concat(actualDenied), refPoint );
        }

        public DrawSpace Restrict(IEnumerable<Rectangle> restricted_areas)
        {
            IEnumerable<Rectangle> new_allowed;
            if(restricted_areas.IsEmpty())
                // If no restriction - leave allowed rect the same
                new_allowed = this.allowedRects;
            else
                // Otherwise intersect each allowed rect with each restricted area
                new_allowed =
                    restricted_areas.SelectFromPairs( allowedRects,
                                                      (ref Rectangle x, Rectangle y) => x.Intersect(y) );
            return new DrawSpace(new_allowed, this.deniedRects, this.referencePoint);
        }

        public bool IsAffecting(Rectangle rect)
        {
            rect.Offset( this.referencePoint );
            return allowedRects.Any(x => rect.IntersectsWith(x));
        }
        
        #endregion // Creating Helpers

        #region Drawing Methods

        public void PutCharacter(Point point, char ch)
        {
            point.Offset( referencePoint );
            if( allowedRects.Any( x => x.Contains(point) ) && deniedRects.All( x => ! x.Contains(point) ) )
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
            if( pattern.Length == 0 )
                return;

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

        public void PutString(Point from, string str, int length = 0, bool centered = false)
        {
            string outStr;
            if( length != 0 )
            {
                if( str.Length > length )
                {
                    outStr = str.Substring(0, length-1) + "~";
                }
                else if( centered )
                {
                    var delta = length - str.Length;
                    outStr = str.PadLeft(length - delta/2).PadRight(length);
                }
                else
                    outStr = str.PadRight(length);
            }
            else
                outStr = str;
            DrawPatternLine(from, from + new Size(outStr.Length-1,0), outStr);
        }

        public void DrawRectangle(Rectangle rect, char ch)
        {
            DrawLine(new Point(rect.Left,    rect.Top     ), new Point(rect.Right-1, rect.Top     ), ch);
            DrawLine(new Point(rect.Right-1, rect.Top     ), new Point(rect.Right-1, rect.Bottom-1), ch);
            DrawLine(new Point(rect.Right-1, rect.Bottom-1), new Point(rect.Left,    rect.Bottom-1), ch);
            DrawLine(new Point(rect.Left,    rect.Bottom-1), new Point(rect.Left,    rect.Top     ), ch);
        }

        public void DrawBorder(Rectangle rect, string pattern = SingleBorder, string title = null)
        {
            // corners
            PutCharacter(new Point(rect.Left,    rect.Top     ), pattern[0]);
            PutCharacter(new Point(rect.Right-1, rect.Top     ), pattern[1]);
            PutCharacter(new Point(rect.Right-1, rect.Bottom-1), pattern[2]);
            PutCharacter(new Point(rect.Left,    rect.Bottom-1), pattern[3]);

            // title
            title = title ?? "";

            var maxTitleLength = rect.Width - 2;
            PutString(new Point(rect.Left+1, rect.Top), title, maxTitleLength);


            // borders
            if( title.Length < maxTitleLength )
                DrawLine(new Point(rect.Left+1+title.Length, rect.Top), new Point(rect.Right-2, rect.Top), pattern[4]);
            DrawLine(new Point(rect.Right-1, rect.Top+1   ), new Point(rect.Right-1, rect.Bottom-2), pattern[5]);
            DrawLine(new Point(rect.Right-2, rect.Bottom-1), new Point(rect.Left+1,  rect.Bottom-1), pattern[6]);
            DrawLine(new Point(rect.Left,    rect.Bottom-2), new Point(rect.Left,    rect.Top+1   ), pattern[7]);
        }

        public void FillRectangle(Rectangle rect, char ch)
        {
            for(int x = rect.Left; x < rect.Right; x++)
                for(int y = rect.Top; y < rect.Bottom; y++)
                    PutCharacter(new Point(x,y), ch);
        }

        #endregion // Drawing Methods
    }
}

