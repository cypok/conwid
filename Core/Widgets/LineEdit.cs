using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core.Widgets
{
    using Messages;

    public class LineEdit : Widget
    {
        #region Constants

        readonly ConsoleKeyInfo BackspaceLineEditKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Backspace, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo RollLeftLineEditKeyInfo  = new ConsoleKeyInfo('_', ConsoleKey.LeftArrow, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo RollRightLineEditKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.RightArrow, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo RollHomeLineEditKeyInfo  = new ConsoleKeyInfo('_', ConsoleKey.Home, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo RollEndLineEditKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.End, control: false, shift: false, alt: false);

        readonly Color ActiveLineEditColor = new Color()
        {
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.DarkGray
        };
        readonly Color InactiveLineEditColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.DarkGray
        };

        #endregion //Constants
        
        #region Fields & Properties

        private string text;
        public string Text
        { 
            get { return text; }
            set
            {
                if( text != value )
                {
                    var old = text;
                    text = value;
                    Emit(OnTextChanged, this, text, old);
                    WidgetManager.Instance.PostMessage(new RedrawWidgetMessage(this));
                }
            }
        }
        private int TextRolling { get; set; }

        #endregion //Fields & Properties
        
        #region Events
        
        public delegate void TextChangeHandler(LineEdit le, string newValue, string oldValue);
        public event TextChangeHandler OnTextChanged;

        #endregion //Events

        public LineEdit(Point pos, int width, string text = "") : base(new Rectangle(pos, new Size(width, 1)))
        {
            this.text = text;
            TextRolling = 0;
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
                var keyChar = keyInfo.KeyChar;
                if( keyInfo.IsEqualTo(BackspaceLineEditKeyInfo) && Text.Length > 0 )
                {
                    Text = Text.Substring(0, Text.Length-1);
                    if( TextRolling >= Text.Length && TextRolling > 0)
                        TextRolling--;
                }
                else if( keyInfo.IsEqualTo(RollLeftLineEditKeyInfo) )
                {
                    if( TextRolling > 0)
                        TextRolling--;
                }
                else if( keyInfo.IsEqualTo(RollRightLineEditKeyInfo) )
                {
                    if( TextRolling < (Text.Length - Area.Width + 1) )
                        TextRolling++;
                }
                else if( keyInfo.IsEqualTo(RollHomeLineEditKeyInfo) )
                {
                    TextRolling = 0;
                }
                else if( keyInfo.IsEqualTo(RollEndLineEditKeyInfo) )
                {
                    TextRolling = Text.Length - Area.Width + 1;
                }
                else if( IsValidForInput(keyInfo.Key) )
                {
                    Text += keyChar;
                    if( TextRolling < (Text.Length - Area.Width + 1) )
                        TextRolling = Text.Length - Area.Width + 1;
                }
                WidgetManager.Instance.PostMessage(new RedrawWidgetMessage(this));
            }
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveLineEditColor : InactiveLineEditColor;

            var outText = Text.Substring( TextRolling, Math.Min(Area.Width, Text.Length-TextRolling) );
            if( IsActive() && outText.Length < Area.Width )
                outText += "_";
            outText = outText.PadRight(Area.Width);

            ds.PutString(Point.Empty, outText, Area.Width);
        }

        bool IsValidForInput(ConsoleKey key)
        {
            return  ( ConsoleKey.D0 <= key && key <= ConsoleKey.D9 ) ||
                    ( ConsoleKey.A  <= key && key <= ConsoleKey.Z  ) ||
                    ( ConsoleKey.A  <= key && key <= ConsoleKey.Z  ) ||
                    ( ConsoleKey.Multiply <= key && key <= ConsoleKey.Divide) ||
                    ( ConsoleKey.Multiply <= key && key <= ConsoleKey.Divide) ||
                    ( ConsoleKey.Oem1 <= key && key <= ConsoleKey.Oem102 ) ||
                    key == ConsoleKey.Spacebar;
        }
    }
}