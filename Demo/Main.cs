using System;
using System.Drawing;
using System.Collections.Generic;

using Conwid.Core;
using Conwid.Core.Messages;
using Conwid.Core.Widgets;

using Color = Conwid.Core.Color;

namespace Demo
{
    using WidgetGroup = UIManager<Widget>; // it's not a typedef, it's defined only in current file. But it's a bit better :)

    class MainClass
    {
        public static void Main (string[] args)
        {
            var theLoop = MessageLoop.Instance;

            #region Button Demo

            var wgButtons = new WidgetGroup(null, new Rectangle(0,5,23,17), "Button Demo");
            new Button(wgButtons, new Point(1,1), "Push Me", height: 1, width: 7);
            new Button(wgButtons, new Point(1,3), "Big Push Me");
            new Button(wgButtons, new Point(1,6), "Bigger Push Me", height: 5, width: 21);
            var btTitle = new Button(wgButtons, new Point(1,11), "Change title", height: 1);
            var openSecret = new Button(wgButtons, new Point(1,13), "Secret!", height: 1, width: 13);
            var secret = new Button(null, new Point(1,15), "Woohoo!", height: 1 );

            btTitle.OnPressed += (
                _ => (btTitle.Parent as WidgetGroup).Title += "!"
            );
            
            openSecret.OnPressed += (
                _ => secret.Parent = openSecret.Parent
            );
            secret.OnPressed += (
                _ => secret.Parent = null
            );

            #endregion //Button Demo
            
            #region LineEdit Demo
            
            var wgEdits = new WidgetGroup(null, new Rectangle(23,5,23,17), "LineEdit Demo");
            new Label(wgEdits, new Point(2,2), "Name");
            new LineEdit(wgEdits, new Point(9,2), 12, "Вася");
            new Label(wgEdits, new Point(2,4), "Age");
            new LineEdit(wgEdits, new Point(9,4), 12, "99");

            var color = new Color(ConsoleColor.Yellow, ConsoleColor.Black);
            new Label(wgEdits, new Point(2,6), "Please donate more", color: color);
            new Label(wgEdits, new Point(2,7), "Donate");
            new LineEdit(wgEdits, new Point(9,7), 7, "100");
            new Label(wgEdits, new Point(17,7), "$");

            new Label(wgEdits, new Point(2, 10), "Write:");
            var write = new LineEdit(wgEdits, new Point(9,10), 12, "");
            new Label(wgEdits, new Point(2, 12), "Wrote:");
            var wrote = new Label(wgEdits, new Point(9,12), "", width: 12);
            write.OnTextChanged +=
                (_, text, __) => wrote.Text = text;
            
            #endregion //LineEdit Demo
            
            #region CheckBox Demo

            var wgChecks1 = new WidgetGroup(null, new Rectangle(46,5,23,9), "CheckBox Demo");
            new CheckBox(wgChecks1, new Point(2,1), "NSU Student", state: true);
            new CheckBox(wgChecks1, new Point(2,3), "From Africa", state: false);
            new CheckBox(wgChecks1, new Point(2,5), "Loves programming", state: true);
            new CheckBox(wgChecks1, new Point(2,7), "Likes jogging", state: false);

            var wgChecks2 = new WidgetGroup(null, new Rectangle(46,15,23,7), "CheckBox-RadioButton");
            var cbFirst = new CheckBox(wgChecks2, new Point(2,2), "Rock!", state: true);
            var cbSecond = new CheckBox(wgChecks2, new Point(2,4), "Chanson...", state: false);
            cbFirst.OnStateChanged +=
                (_, state, __) => cbSecond.State = !state;
            cbSecond.OnStateChanged +=
                (_, state, __) => cbFirst.State = !state;

            #endregion //CheckBox Demo

            #region Moving Demo

            var wgMoving = new WidgetGroup(null, new Rectangle(15,3,26,10), "Moving Demo");
            var green = new Color(ConsoleColor.Green, ConsoleColor.Black);
            new Label(wgMoving, new Point(2,2), "Fell free to move this", color: green);
            new Label(wgMoving, new Point(2,4), "using Ctrl+arrows", color: green);
            new Label(wgMoving, new Point(2,6), "and close it (Ctrl+W)", color: green);

            #endregion //Moving Demo
 
            #region Panel

            var wgPanel = new WidgetGroup(theLoop.WidgetManager, new Rectangle(0,0,80,5), "Conwid - Console Widgets Library Demo");
            
            var pos = new Point(1,1);
            var btButtonDemo = new Button( wgPanel, pos, "Button Demo" );
            pos += new Size(btButtonDemo.Area.Width + 1, 0);
            var btLineEditDemo = new Button( wgPanel, pos, "LineEdit Demo" );
            pos += new Size(btLineEditDemo.Area.Width + 1, 0);
            var btCheckBoxDemo = new Button( wgPanel, pos, "ChechBox Demo" );
            pos += new Size(btCheckBoxDemo.Area.Width + 1, 0);
            var btMoveDemo = new Button( wgPanel, pos, "Moving Demo" );
            pos += new Size(btMoveDemo.Area.Width + 2, 0);
            var btExit = new Button( wgPanel, pos, "Exit" );
            pos += new Size(btExit.Area.Width + 1, 0);
            
            wgPanel.Area = new Rectangle(0,0,pos.X,5);
            
            btButtonDemo.OnPressed += (
                _ => wgButtons.Parent = wgButtons.Parent == null ? theLoop.WidgetManager : null
            );
            btLineEditDemo.OnPressed += (
                _ => wgEdits.Parent = wgEdits.Parent == null ? theLoop.WidgetManager : null
            );
            btCheckBoxDemo.OnPressed += (
                _ => {
                    wgChecks2.Parent = wgChecks2.Parent == null ? theLoop.WidgetManager : null;
                    wgChecks1.Parent = wgChecks1.Parent == null ? theLoop.WidgetManager : null;
                }
            );
            btMoveDemo.OnPressed += (
                _ => wgMoving.Parent = wgMoving.Parent == null ? theLoop.WidgetManager : null
            );

            btExit.OnPressed += (
                _ => theLoop.PostMessage(new QuitMessage())
            );
            
            #endregion //Panel

            theLoop.Run();
        }
    }
}
