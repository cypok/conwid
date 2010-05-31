using System;
using System.Drawing;

using Conwid.Core;
using Conwid.Core.Messages;

namespace TicTacToe
{
    using Color = Conwid.Core.Color;

    public sealed class MakeMoveMessage : IMessage
    {
        public Point Point { get; private set; }
        public MakeMoveMessage(Point point) { Point = point; }
    }

    class TicTacToeField : Widget
    {
        #region Constants
        
        readonly ConsoleKeyInfo GoLeftKeyInfo  = new ConsoleKeyInfo('_', ConsoleKey.LeftArrow, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo GoRightKeyInfo  = new ConsoleKeyInfo('_', ConsoleKey.RightArrow, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo GoUpKeyInfo  = new ConsoleKeyInfo('_', ConsoleKey.UpArrow, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo GoDownKeyInfo  = new ConsoleKeyInfo('_', ConsoleKey.DownArrow, control: false, shift: false, alt: false);
        readonly ConsoleKeyInfo PutMarkKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Spacebar, control: false, shift: false, alt: false);
        
        readonly Color ActiveCrossColor = new Color()
        {
            Foreground = ConsoleColor.Red,
            Background = ConsoleColor.DarkRed
        };
        readonly Color InactiveCrossColor = new Color()
        {
            Foreground = ConsoleColor.DarkRed,
            Background = ConsoleColor.DarkRed
        };
        readonly Color ActiveNoughtColor = new Color()
        {
            Foreground = ConsoleColor.Blue,
            Background = ConsoleColor.DarkBlue
        };
        readonly Color InactiveNoughtColor = new Color()
        {
            Foreground = ConsoleColor.DarkBlue,
            Background = ConsoleColor.DarkBlue
        };
        readonly Color ActiveEmptyColor = new Color()
        {
            Foreground = ConsoleColor.DarkGray,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveEmptyColor = new Color()
        {
            Foreground = ConsoleColor.Black,
            Background = ConsoleColor.Black
        };
        readonly Color ActiveBorderColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };
        readonly Color InactiveBorderColor = new Color()
        {
            Foreground = ConsoleColor.Gray,
            Background = ConsoleColor.Black
        };

        #endregion // Constants
        
        #region Fields & Properties

        public enum Cell
	    {
	        Empty,
            Cross,
            Nought,
            Draw
	    }

        Cell[,] field = new Cell[3,3];
        public Cell[,] Field { get { return field; } }
        Point currentPoint;
        Cell turn;
        public Cell Turn { get { return turn; } }

        #endregion // Fields & Properties

        #region Events
        
        public delegate void TurnChangeHandler(TicTacToeField field, Cell newTurn, Cell oldTurn);
        public event TurnChangeHandler OnTurnChanged;

        public delegate void GameOverHandler(TicTacToeField field, Cell winner);
        public event GameOverHandler OnGameOver;

        #endregion // Events

        public TicTacToeField(UIElement parent, Point pos) :
            base(parent, new Rectangle(pos, new Size( 4 + 3*3, 4 + 2*3 )))
        {
            PrepareField();
        }

        public void PrepareField()
        {
            currentPoint = new Point(1,1);
            for(int i = 0; i < 3; ++i)
                for(int j = 0; j < 3; ++j)
                    field[i,j] = Cell.Empty;
            turn = Cell.Cross;
            Emit(OnTurnChanged, this, turn, Cell.Empty);
        }
        
        // Handles:
        // * KeyPressedMessage
        // * MakeMoveMessage
        public override void Handle(IMessage msg)
        {
            if(msg is KeyPressedMessage)
            {
                var keyInfo = (msg as KeyPressedMessage).KeyInfo;
                if( keyInfo.EqualsTo(GoLeftKeyInfo) )
                {
                    if( currentPoint.X > 0 )
                    {
                        currentPoint.X--;
                        Invalidate();
                    }
                }
                else if( keyInfo.EqualsTo(GoRightKeyInfo) )
                {
                    if( currentPoint.X < 2 )
                    {
                        currentPoint.X++;
                        Invalidate();
                    }
                }
                else if( keyInfo.EqualsTo(GoUpKeyInfo) )
                {
                    if( currentPoint.Y > 0 )
                    {
                        currentPoint.Y--;
                        Invalidate();
                    }
                }
                else if( keyInfo.EqualsTo(GoDownKeyInfo) )
                {
                    if( currentPoint.Y < 2 )
                    {
                        currentPoint.Y++;
                        Invalidate();
                    }
                }
                else if( keyInfo.EqualsTo(PutMarkKeyInfo) )
                {
                    MakeMove();
                }
            }
            else if( msg is MakeMoveMessage )
            {
                MakeMove((msg as MakeMoveMessage).Point);
            }
            return;
        }

        protected void MakeMove(Point? point = null)
        {
            var oldCurrentPoint = currentPoint;
            currentPoint = point.GetValueOrDefault(currentPoint);
            if( field[currentPoint.X, currentPoint.Y] == Cell.Empty )
            {
                field[currentPoint.X, currentPoint.Y] = turn;
                        
                if( ! CheckGameOver() )
                {
                    var oldTurn = turn;
                    turn = (oldTurn == Cell.Cross) ? Cell.Nought : Cell.Cross;
                    Emit(OnTurnChanged, this, turn, oldTurn);
                }
                Invalidate();
            }
            currentPoint = oldCurrentPoint;
        }

        public override void Draw(DrawSpace ds)
        {
            // border
            ds.Color = IsActive ? ActiveBorderColor : InactiveBorderColor;
            // Characters: ┌ ┐ ┘ └ ─ │ ─ │ ┬ ┤ ┴ ├ ┼
            ds.DrawBorder(new Rectangle(0,0,5,4), "┌┬┼├─│─│");
            ds.DrawBorder(new Rectangle(8,0,5,4), "┬┐┤┼─│─│");
            ds.DrawBorder(new Rectangle(8,6,5,4), "┼┤┘┴─│─│");
            ds.DrawBorder(new Rectangle(0,6,5,4), "├┼┴└─│─│");
            
            ds.DrawLine(new Point(4+1,0), new Point(8-1,0), '─');
            ds.DrawLine(new Point(4+1,3), new Point(8-1,3), '─');
            ds.DrawLine(new Point(4+1,6), new Point(8-1,6), '─');
            ds.DrawLine(new Point(4+1,9), new Point(8-1,9), '─');
            
            ds.DrawLine(new Point(0,3+1), new Point(0,6-1), '│');
            ds.DrawLine(new Point(4,3+1), new Point(4,6-1), '│');
            ds.DrawLine(new Point(8,3+1), new Point(8,6-1), '│');
            ds.DrawLine(new Point(12,3+1), new Point(12,6-1), '│');

            // noughts & crosses
            for(int i = 0; i < 3; ++i)
                for(int j = 0; j < 3; ++j)
                {
                    var pos = new Point(1+4*i, 1+3*j);
                    switch (field[i,j])
                    {
                        case Cell.Empty:
                            ds.Color = IsActive ? ActiveEmptyColor : InactiveEmptyColor;
                            break;
                        case Cell.Cross:
                            ds.Color = IsActive ? ActiveCrossColor : InactiveCrossColor;
                            break;
                        case Cell.Nought:
                            ds.Color = IsActive ? ActiveNoughtColor : InactiveNoughtColor;
                            break;
                        default:
                            break;
                    }
                    var ch = (currentPoint == new Point(i,j)) ? '░' : ' ';
                    ds.DrawLine(pos + new Size(0,0), pos + new Size(2,0), ch);
                    ds.DrawLine(pos + new Size(0,1), pos + new Size(2,1), ch);
                }
        }

        private bool CheckGameOver()
        {
            bool draw = true;
            foreach( var f in field )
            {
                draw = draw && (f != Cell.Empty);
            }

            Cell winner;
            if( draw )
            {
                winner = Cell.Draw;
            }
            else
            {
                winner = Cell.Empty;
                for(int i = 0; i < 3; ++i)
                {
                    if(field[i,0] == field[i,1] && field[i,1] == field[i,2] && field[i,1] != Cell.Empty)
                        winner = field[i,1];
                    if(field[0,i] == field[1,i] && field[1,i] == field[2,i] && field[1,i] != Cell.Empty)
                        winner = field[1,i];
                }
                if(field[0,0] == field[1,1] && field[1,1] == field[2,2] && field[1,1] != Cell.Empty)
                    winner = field[1,1];
                if(field[2,0] == field[1,1] && field[1,1] == field[0,2] && field[1,1] != Cell.Empty)
                    winner = field[1,1];
            }

            if(winner != Cell.Empty)
            {
                Emit(OnGameOver, this, winner);
                PrepareField();
                return true;
            }
            return false;
        }
    }
}
