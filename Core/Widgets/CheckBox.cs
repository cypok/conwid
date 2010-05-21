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
        #region Constants

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
        private bool state;
        public bool State
        {
            get { return state; }
            set
            {
                if( state != value )
                {
                    var old = state;
                    state = value;
                    Emit(OnStateChanged, this, state, old);
                    Invalidate();
                }
            }
        }

        #endregion // Fields & Properties
        
        #region Events
        
        public delegate void StateChangeHandler(CheckBox cb, bool newValue, bool oldValue);
        public event StateChangeHandler OnStateChanged;

        #endregion // Events

        public CheckBox(Point pos, string text, bool state = false, int? width = null) :
            base(new Rectangle(pos, new Size( width ?? 2+text.Length, 1)))
        {
            this.text = text;
            this.state = state;
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
                if( keyInfo.EqualsTo(ChangeCheckBoxKeyInfo))
                {
                    State = !State;
                }
            }
            return;
        }

        public override void Draw(DrawSpace ds)
        {
            ds.Color = IsActive() ? ActiveBoxCheckBoxColor : InactiveBoxCheckBoxColor;
            ds.PutString(Point.Empty, State ? CheckCharacter.ToString() : " ", 1);
            
            ds.Color = IsActive() ? ActiveTextCheckBoxColor : InactiveTextCheckBoxColor;
            var outText = " " + Text;
            outText = outText.PadRight(Area.Width-1);

            ds.PutString(new Point(1,0), outText, Area.Width-1);
        }
    }
}