using System;
using System.Drawing;
using System.Threading;
using System.Collections;

namespace TicTacToe
{
    using Cell = TicTacToeField.Cell;
    class BotEngine
    {
        public static Point GenerateMove(Cell turn, Cell[,] field)
        {
            System.Threading.Thread.Sleep(200);
            ArrayList points = new ArrayList();
            for(int i = 0; i < 3; ++i)
                for(int j = 0; j < 3; ++j)
                    if( field[i,j] == Cell.Empty )
                        points.Add( new Point(i,j) );
            if( points.Count == 0)
                throw new InvalidOperationException("Could not generate move for full field");
            Random rnd = new Random();
            return (Point)points[rnd.Next(points.Count)];
        }
    }
}
