using System;
using System.Drawing;
using System.Collections.Generic;

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

            var wg = new WidgetGroup[]
            {
                new WidgetGroup(theLoop.WidgetManager, new Rectangle(3,2,45,12), "First"),
                new WidgetGroup(theLoop.WidgetManager, new Rectangle(12,6,45,18)),
                new WidgetGroup(theLoop.WidgetManager, new Rectangle(21,10,45,12), "With really long long long name"),
            };
            var hidden = new WidgetGroup(null, new Rectangle(1, 10, 8, 10), "hidden");


            new LineEdit( new Point(1,1), 15, "Hello, World!") { Parent = wg[0] };
            new LineEdit( new Point(1,3), 4, "0123456789") { Parent = wg[0] };
            new CheckBox( new Point(2,5), "nsu student") { Parent = wg[0] };
            var more = new Button(new Point(1,7), "gimme more", height:1) { Parent = wg[0] };
            new Button( new Point(1,1), "Push Me", height: 1) { Parent = wg[1]};
            new Button( new Point(1,3), "Push Me", height: 1, width: 7) { Parent = wg[1]};
            var openSecret = new Button( new Point(1,5), "Secret!", height: 1, width: 13) { Parent = wg[1]};
            new Button( new Point(1,7), "Big Push Me") { Parent = wg[1]};
            new Button( new Point(1,10), "Bigger Push Me", height: 5, width: 22) { Parent = wg[1]};
            var secret = new Button( new Point (1, 16), "Woohoo!", height: 1 );

            var exitButton = new Button(new Point(1,1), " E X I T ", height: 3) { Parent = wg[2] };
            exitButton.OnPressed += (
                _ => MessageLoop.Instance.PostMessage(new QuitMessage())
            );

            var label = new Label( new Point(2,5), "Edit me", width: 15) { Parent = wg[2] };
            var leLabel = new LineEdit( new Point(2,6), 15, "Edit me") { Parent = wg[2] };
            leLabel.OnTextChanged +=
                (_, text, __) => label.Text = text;
            
            var cbRuby = new CheckBox( new Point(3,8), "love to Ruby", state: true) { Parent = wg[2] };
            var cbCSharp = new CheckBox( new Point(3,9), "love to C#", state: false) { Parent = wg[2] };
            cbRuby.OnStateChanged +=
                (_, state, __) => cbCSharp.State = !state;
            cbCSharp.OnStateChanged +=
                (_, state, __) => cbRuby.State = !state;

            var secret_opened = false;
            openSecret.OnPressed += _ =>
            {
                secret.Parent = secret_opened ? null : openSecret.Parent;
                secret_opened = !secret_opened;
            };

            more.OnPressed += _ => hidden.Parent = theLoop.WidgetManager;

            Console.CursorVisible = false;

            theLoop.Run();

            Console.WriteLine( theLoop.WidgetManager.DebugDump() );
        }
    }
}
