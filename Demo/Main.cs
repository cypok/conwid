﻿using System;
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


            new LineEdit( wg[0], wg[0].ClientArea.Location, wg[0].ClientArea.Width, "Hello, World!");
            new LineEdit( wg[0], new Point(1,3), 4, "0123456789");
            new CheckBox( wg[0], new Point(2,5), "nsu student");
            var btTitle = new Button( wg[1], new Point(1,1), "Change title", height: 1);
            new Button( wg[1], new Point(1,3), "Push Me", height: 1, width: 7);
            new Button( wg[1], new Point(1,5), "Push Me", height: 1, width: 13);
            new Button( wg[1], new Point(1,7), "Big Push Me");
            new Button( wg[1], new Point(1,10), "Bigger Push Me", height: 5, width: 22);

            btTitle.OnPressed += (
                _ => (btTitle.Parent as WidgetGroup).Title = "Changed!"
            );

            var exitButton = new Button( wg[2], new Point(1,1), " E X I T ", height: 3);
            exitButton.OnPressed += (
                _ => MessageLoop.Instance.PostMessage(new QuitMessage())
            );

            var label = new Label( wg[2], new Point(2,5), "Edit me", width: 15, centered: true);
            var leLabel = new LineEdit( wg[2], new Point(2,6), 15, "Edit me");
            leLabel.OnTextChanged +=
                (_, text, __) => label.Text = text;
            
            var cbRuby = new CheckBox( wg[2], new Point(3,8), "love to Ruby", state: true);
            var cbCSharp = new CheckBox( wg[2], new Point(3,9), "love to C#", state: false);
            cbRuby.OnStateChanged +=
                (_, state, __) => cbCSharp.State = !state;
            cbCSharp.OnStateChanged +=
                (_, state, __) => cbRuby.State = !state;

            Console.CursorVisible = false;

            theLoop.Run();

            Console.WriteLine( theLoop.WidgetManager.DebugDump() );
        }
    }
}
