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
                new Frame( new Rectangle(2,2,10,5) ),
                new Frame( new Rectangle(2,2,10,5) ),
            };

            foreach (var w in widgets)
            {
                WidgetManager.Instance.PostMessage(new AddWidgetMessage(w));
            }
            MessageLoop.Instance.PostMessage(new QuitMessage());
            
            MessageLoop.Instance.Run();
            Console.WriteLine( WidgetManager.Instance.DebugDump() );
        }
    }
}
