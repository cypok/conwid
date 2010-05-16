using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    using Messages;

    public class Label : Widget
    {
        #region Constants

        readonly Color ActiveLabelColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveLabelColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };

        #endregion //Constants
        
        #region Fields & Properties

        string text;
        public string Text
        { 
            get { return text; }
            set
            {
                text = value;
                WidgetManager.Instance.PostMessage(new RedrawWidgetMessage(this));
            }
        }

        #endregion //Fields & Properties
        
        #region Events

        #endregion //Events

        public Label(Point pos, string text, int width = 0) :
            base(new Rectangle(pos, new Size( (width == 0) ? 2+text.Length : width, 1)))
        {
            this.text = text;
        }

        // Handles:
        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveLabelColor : InactiveLabelColor;

            var outText = Text.PadRight(Area.Width);
            ds.PutString(new Point(0,0), outText, Area.Width);
        }
    }
}