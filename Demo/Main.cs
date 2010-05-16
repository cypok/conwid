using System;
using System.Drawing;

using Conwid.Core;
using Conwid.Core.Messages;
using Conwid.Core.Widgets;

namespace Demo
{
    using WidgetGroup = UIManager<Widget>; // it's not a typedef, it's defined only in current file. But it's a bit better :)

    class MainClass
    {
        public static void Main (string[] args)
        {
            var theLoop = MessageLoop.Instance;

            var widGroups = new WidgetGroup[]
            {
                new WidgetGroup(theLoop.WidgetManager, new Rectangle(3,3,30,10), "First"),
                new WidgetGroup(theLoop.WidgetManager, new Rectangle(8,5,30,10)),
                new WidgetGroup(theLoop.WidgetManager, new Rectangle(13,6,30,10), "With really long long long name"),
            };
            
            var widgets = new Widget[]
            {
                new LineEdit( new Point(1,1), 17, "Hello, World!") { Parent = widGroups[0] },
                new LineEdit( new Point(1,3), 15, "") { Parent = widGroups[0] },
                new LineEdit( new Point(10,4), 4, "0123456789") { Parent = widGroups[1] },
                new CheckBox( new Point(3,1), "nsu student") { Parent = widGroups[2] },
                new CheckBox( new Point(3,3), "love to DaftPunk", width: 14) { Parent = widGroups[2] },
                new CheckBox( new Point(3,5), "love to Ruby", value: true) { Parent = widGroups[2] },
            };

            Console.CursorVisible = false;

            theLoop.Run();

            Console.WriteLine( theLoop.WidgetManager.DebugDump() );
        }
    }
}
