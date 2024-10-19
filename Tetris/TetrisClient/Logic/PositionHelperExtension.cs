using System.Collections.Generic;
using System.Windows.Media;

namespace TetrisClient.Logic
{
    /// <summary>
    /// Extension class that helps with creating lists of coordinates for both the Board and Shape class.
    /// </summary>
    public static class PositionHelperExtension
    {
        /// <summary>
        /// Returns a list with a coordinate tuple (x, y) by looping through a shape's Matrix.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static List<(int, int)> ToList(this Shape shape)
        {
            var offset = shape.Offset;
            var value = shape.Matrix.Value;
            var values = new List<(int, int)>();
            for (var y = 0; y < value.GetLength(0); y++)
            for (var x = 0; x < value.GetLength(0); x++)
            {
                if (value[y, x] == 0) continue;

                values.Add((x + offset.X, y + offset.Y));
            }
            
            return values;
        }
        
        /// <summary>
        /// Returns a list witha coordinate tuple (x, y) by looping through the board's value.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static List<(Color, int, int)> ToList(this Board board)
        {
            var value = board.Value;
            var values = new List<(Color, int, int)>();
            for (var y = 0; y < value.GetLength(0); y++)
            for (var x = 0; x < value.GetLength(1); x++)
            {
                values.Add((value[y, x], x, y));
            }
            
            return values;
        }
    }
}
