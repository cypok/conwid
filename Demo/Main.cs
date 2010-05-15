using System;
using System.Drawing;

using Conwid.Core;
using Conwid.Core.Messages;
using Conwid.Core.Widgets;

namespace Demo
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var widgets = new Widget[]
            {
                new Frame( new Rectangle(3,3,30,10), "First" ),
                new Frame( new Rectangle(8,5,30,10) ),
                new Frame( new Rectangle(13,6,30,10), "With really long long long name" ),
            };

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
