using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    public class Frame : Widget
    {
        #region Constants

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

        #endregion Constants
        
        #region Fields & Properties

        public string Title { get; private set; }

        #endregion Fields & Properties
        
        #region Events

        #endregion Events

        public Frame(Rectangle area, string title = "") : base(area)
        {
            Title = title;
        }
        
        // Handles:
        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveFrameColor : InactiveFrameColor;

            var rect = new Rectangle(Point.Empty, Size);
            ds.DrawBorder(rect, DrawSpace.DoubleBorder, Title);

            ds.FillRectangle(new Rectangle(new Point(1,1), Size - new Size(2,2)), ' ');
        }
    }
}