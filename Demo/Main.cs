using System;
using System.Drawing;
using System.Collections.Generic;

using Conwid.Core;
using Conwid.Core.Messages;
using Conwid.Core.Widgets;

namespace Demo
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var widgets = new List<Widget>
            {
                new Frame( new Rectangle(3,3,30,10), "First" ),
                new Frame( new Rectangle(8,5,30,10) ),
                new Frame( new Rectangle(13,6,30,10), "With really long long long name" ),
                new LineEdit( new Point(2,1), 15, "Hello, World!"),
                new LineEdit( new Point(15,0), 4, "0123456789"),
                new CheckBox( new Point(3,18), "nsu student"),
                new Button( new Point(40,1), "Push Me", height: 1),
                new Button( new Point(40,2), "Push Me", height: 1, width: 7),
                new Button( new Point(40,3), "Push Me", height: 1, width: 13),
                new Button( new Point(50,5), "Big Push Me"),
                new Button( new Point(50,9), "Bigger Push Me", height: 4, width: 22),
            };

            var exitButton = new Button(new Point(50,20), " E X I T ", height: 3);
            exitButton.OnPressed += (
                _ => MessageLoop.Instance.PostMessage(new QuitMessage())
            );
            widgets.Add(exitButton);

            var label = new Label( new Point(25,0), "Edit me", width: 15);
            var leLabel = new LineEdit( new Point(20,2), 15, "Edit me");
            leLabel.OnTextChanged +=
                (_, text, __) => label.Text = text;
            widgets.Add(label);
            widgets.Add(leLabel);
            
            var cbRuby = new CheckBox( new Point(3,21), "love to Ruby", state: true);
            var cbCSharp = new CheckBox( new Point(3,22), "love to C#", state: false);
            cbRuby.OnStateChanged +=
                (_, state, __) => cbCSharp.State = !state;
            cbCSharp.OnStateChanged +=
                (_, state, __) => cbRuby.State = !state;

            widgets.Add(cbRuby);
            widgets.Add(cbCSharp);

            foreach (var w in widgets)
                WidgetManager.Instance.PostMessage(new AddWidgetMessage(w));
            foreach (var w in widgets)
                WidgetManager.Instance.PostMessage(new RedrawWidgetMessage(w));

            Console.CursorVisible = false;

            //MessageLoop.Instance.PostMessage(new QuitMessage());
            
            MessageLoop.Instance.Run();

            Console.WriteLine( WidgetManager.Instance.DebugDump() );
        }
    }
}
