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
                new LineEdit( new Point(20,2), 15, ""),
                new LineEdit( new Point(15,0), 4, "0123456789"),
                new CheckBox( new Point(3,20), "nsu student"),
                new CheckBox( new Point(3,21), "love to DaftPunk", width: 14),
                new CheckBox( new Point(3,22), "love to Ruby", value: true),
                new Label( new Point(25,0), "\\\\ C# sucks! //"),
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
