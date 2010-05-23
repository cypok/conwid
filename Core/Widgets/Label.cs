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

        #endregion // Constants
        
        #region Fields & Properties

        string text;
        public string Text
        { 
            get { return text; }
            set
            {
                text = value;
                Invalidate();
            }
        }
        bool centered;
        public bool Centered
        { 
            get { return centered; }
            set
            {
                centered = value;
                Invalidate();
            }
        }

        #endregion // Fields & Properties
        
        #region Events

        #endregion // Events

        public Label(UIElement parent, Point pos, string text, int? width = null, bool centered = false) :
            base(parent, new Rectangle(pos, new Size( width ?? 2+text.Length, 1)))
        {
            this.text = text;
            this.centered = centered;
        }

        // Handles:
        public override void Handle(IMessage msg)
        {
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveLabelColor : InactiveLabelColor;

            ds.PutString(new Point(0,0), Text, Area.Width, Centered);
        }
    }
}