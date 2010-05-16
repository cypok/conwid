using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    using Messages;

    public class Button : Widget
    {
        #region Constants

        readonly ConsoleKeyInfo PushButtonKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Enter, control: false, shift: false, alt: false);
        
        readonly Color ActiveBorderButtonColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.DarkGray
        };
        readonly Color InactiveBorderButtonColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.DarkGray
        };
        readonly Color ActiveTextButtonColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveTextButtonColor = new Color()
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

        public delegate void PressHandler(Button b);
        public event PressHandler OnPressed;

        #endregion //Events

        public Button(Point pos, string text, int height = 3, int width = 0) :
            base(new Rectangle(pos, new Size( (width == 0) ? 2+text.Length : width, height)))
        {
            if( height == 2 )
                throw new ArgumentException("Height could not be equal to 2");

            this.text = text;
        }
        
        // Handles:
        // * KeyPressedMessage
        public override void Handle(IMessage msg)
        {
            var keyInfo = (msg as KeyPressedMessage).KeyInfo;
            if( keyInfo.IsEqualTo(PushButtonKeyInfo))
            {
                Emit(OnPressed, this);
            }
            WidgetManager.Instance.PostMessage(new RedrawWidgetMessage(this));
        }

        public override void Draw(DrawSpace ds)
        {
            // horizontal alignment
            var outStr = Text;
            var delta = Area.Width-2 - Text.Length;
            if( delta > 0 )
                outStr = outStr.PadLeft(Text.Length + delta/2).PadRight(Area.Width-2);
            
            // border
            ds.Color = IsActive() ? ActiveBorderButtonColor : InactiveBorderButtonColor;
            // height is odd
            if( Area.Height == 1 )
            {
                // │ ... │
                ds.PutCharacter(new Point(0,0),             '│');
                ds.PutCharacter(new Point(Area.Width-1,0),  '│');
            }
            else
            {
                var rect = new Rectangle(Point.Empty, Size);
                ds.DrawBorder(rect, DrawSpace.SingleBorder);
            }
                
            // text
            ds.Color = IsActive() ? ActiveTextButtonColor : InactiveTextButtonColor;
            ds.PutString(new Point(1,Area.Height/2), outStr, Area.Width-2);
        }
    }
}