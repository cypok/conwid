using System;
using System.Drawing;
using System.Collections.Generic;

using Conwid.Core;
using Conwid.Core.Messages;
using Conwid.Core.Widgets;

namespace TicTacToe
{
    using WidgetGroup = UIManager<Widget>;
    using Color = Conwid.Core.Color;

    class Program
    {
        static void Main(string[] args)
        {
            var theLoop = MessageLoop.Instance;
            
            var wgExtra = new WidgetGroup(theLoop.WidgetManager, new Rectangle(1,1,21,8), "Score & Settings");
            var wgField = new WidgetGroup(theLoop.WidgetManager, new Rectangle(23,1,30,14), "Tic-Tac-Toe game");
            
            // widgets
            var lePlayerName1 = new LineEdit(wgExtra,
                wgExtra.ClientArea.Location+new Size(1,1),
                wgExtra.ClientArea.Width/2-1,
                "Player");
            var lePlayerName2 = new LineEdit(wgExtra,
                wgExtra.ClientArea.Location+new Size(1+wgExtra.ClientArea.Width/2,1),
                wgExtra.ClientArea.Width/2-1,
                "Bot");
            var cbIsBot1 = new CheckBox(wgExtra,
                wgExtra.ClientArea.Location + new Size(2, 3),
                "bot",
                width: wgExtra.ClientArea.Width/2-1,
                state: false);
            var cbIsBot2 = new CheckBox(wgExtra,
                wgExtra.ClientArea.Location + new Size(2+wgExtra.ClientArea.Width/2, 3),
                "bot",
                width: wgExtra.ClientArea.Width/2-1,
                state: true);
            var playerScore1 = new Label(wgExtra,
                wgExtra.ClientArea.Location + new Size(1, 5),
                "0",
                width: wgExtra.ClientArea.Width/2-1,
                centered: true);
            var playerScore2 = new Label(wgExtra,
                wgExtra.ClientArea.Location + new Size(1+wgExtra.ClientArea.Width/2, 5),
                "0",
                width:  wgExtra.ClientArea.Width/2-1,
                centered: true);
            
            var gameField = new TicTacToeField(wgField,
                wgField.ClientArea.Location + new Size(1, 1));
            
            var playerStatus1 = new Label(wgField,
                wgField.ClientArea.Location + new Size(15, 2),
                "->");
            var playerStatus2 = new Label(wgField,
                wgField.ClientArea.Location + new Size(15, 4),
                "  ");
            var playerStatusName1 = new Label(wgField,
                wgField.ClientArea.Location + new Size(18, 2),
                lePlayerName1.Text,
                width: lePlayerName1.Area.Width,
                color: new Color(ConsoleColor.DarkRed, ConsoleColor.Black));
            var playerStatusName2 = new Label(wgField,
                wgField.ClientArea.Location + new Size(18, 4),
                lePlayerName2.Text,
                width: lePlayerName2.Area.Width,
                color: new Color(ConsoleColor.DarkBlue, ConsoleColor.Black));
            var winnerName = new Label(wgField,
                wgField.ClientArea.Location + new Size(16, 7),
                "",
                width: lePlayerName1.Area.Width,
                color: new Color(ConsoleColor.Yellow, ConsoleColor.Black));
            var winnerMsg = new Label(wgField,
                wgField.ClientArea.Location + new Size(16, 8),
                "",
                width: winnerName.Area.Width,
                color: new Color(ConsoleColor.Yellow, ConsoleColor.Black));

            // connections
            lePlayerName1.OnTextChanged += (
                (_, text, __) => playerStatusName1.Text = text
            );
            lePlayerName2.OnTextChanged += (
                (_, text, __) => playerStatusName2.Text = text
            );

            var firstTurnChange = true;
            TicTacToeField.TurnChangeHandler turnChangedFunc = (_, turn, __) => {
                    var cross = (turn == TicTacToeField.Type.Cross);

                    playerStatus1.Text = cross ? "->" : "  ";
                    playerStatus2.Text = cross ? "  " : "->";

                    if( !firstTurnChange )
                    {
                        winnerName.Text = "";
                        winnerMsg.Text = "";
                    }
                    else
                    {
                        firstTurnChange = false;
                    }
                };
            gameField.OnTurnChanged += turnChangedFunc;
              
            gameField.OnGameOver += (
                (_, winner) => {
                    if( winner == TicTacToeField.Type.Draw )
                    {
                        winnerMsg.Text = "Draw...";
                    }
                    else
                    {
                        var cross = (winner == TicTacToeField.Type.Cross);

                        winnerName.Text = cross ? lePlayerName1.Text : lePlayerName2.Text;
                        winnerMsg.Text = "rocks!";

                        if( cross )
                            playerScore1.Text = (1+int.Parse(playerScore1.Text)).ToString();
                        else
                            playerScore2.Text = (1+int.Parse(playerScore2.Text)).ToString();
                    }
                    firstTurnChange = true;
                }
            );

            turnChangedFunc.Invoke(null, gameField.Turn, gameField.Turn);

            Console.CursorVisible = false;

            theLoop.Run();
        }
    }
}
