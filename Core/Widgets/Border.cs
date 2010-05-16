using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    using Messages;
 
    // TODO: rename to Frame
    public class Border : Widget
    {
        #region Constants

        readonly Color ActiveBorderColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveBorderColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };

        #endregion // Constants
        
        #region Fields & Properties

        #endregion // Fields & Properties
        
        #region Events

        #endregion // Events

        public Border(Rectangle area) : base(area)
        {
        }
        
        // Handles:
        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveBorderColor : InactiveBorderColor;

            var rect = new Rectangle(Point.Empty, Size);
            ds.DrawBorder(rect, DrawSpace.SingleBorder);
        }
    }
}