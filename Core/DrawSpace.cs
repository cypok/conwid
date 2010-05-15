﻿using System;
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
        #region Constants
        
        public const string SingleBorder = "┌┐┘└─│─│";
        public const string DoubleBorder = "╔╗╝╚═║═║";        

        #endregion Constants

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

        public void PutString(Point from, string str, int maxLength = 0)
        {
            string cutStr;
            if( maxLength != 0 && str.Length > maxLength)
                cutStr = str.Substring(0, maxLength-1) + "~";
            else
                cutStr = str;
            DrawPatternLine(from, from + new Size(cutStr.Length-1,0), cutStr);
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
            var maxTitleLength = rect.Width - 2;
            if( title != null )
                PutString(new Point(rect.Left+1, rect.Top), title, maxTitleLength);

            // borders
            if( title == null || title.Length < maxTitleLength )
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

        #endregion Drawing Methods
    }
}

