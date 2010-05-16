using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    using Messages;

    public class CheckBox : Widget
    {
        const char CheckCharacter = '+';

        readonly ConsoleKeyInfo ChangeCheckBoxKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Spacebar, control: false, shift: false, alt: false);
        
        readonly Color ActiveBoxCheckBoxColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.DarkGray
        };
        readonly Color InactiveBoxCheckBoxColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.DarkGray
        };
        readonly Color ActiveTextCheckBoxColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveTextCheckBoxColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };

        public string Text{ get; private set; }
        public bool Value { get; private set; }

        public CheckBox(Point pos, string text, bool value = false, int width = 0) :
            base(new Rectangle(pos, new Size( (width == 0) ? 2+text.Length : width, 1)))
        {
            Text = text;
            Value = value;
        }

        // Handles:
        // * KeyPressedMessage
        public override void Handle(IMessage msg)
        {
            // TODO:
            // C-W - close widget
            if(msg is KeyPressedMessage)
            {
                var keyInfo = (msg as KeyPressedMessage).KeyInfo;
                if( keyInfo.IsEqualTo(ChangeCheckBoxKeyInfo))
                {
                    Value = !Value;
                }
                Invalidate();
            }
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveBoxCheckBoxColor : InactiveBoxCheckBoxColor;
            ds.PutString(Point.Empty, Value ? CheckCharacter.ToString() : " ", 1);
            
            ds.Color = IsActive() ? ActiveTextCheckBoxColor : InactiveTextCheckBoxColor;
            var outText = " " + Text;
            outText = outText.PadRight(Area.Width-1);

            ds.PutString(new Point(1,0), outText, Area.Width-1);
        }
    }
}