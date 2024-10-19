using System.Linq;
using System.Windows.Media;

namespace TetrisClient.Logic
{
    /// <summary>
    /// Inner representation of the Tetris board.
    /// </summary>
    public class Board
    {
        #region Public properties
        
        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height { get; }
        
        /// <summary>
        /// The width of the grid.
        /// </summary>
        public int Width { get; }
        
        /// <summary>
        /// The value's inside the board.
        /// </summary>
        public Color[,] Value { get; }

        #endregion
        
        public Board(int height, int width)
        {
            Height = height;
            Width = width;
            Value = new Color[height, width];

            // Initial board state
            Clear();
        }

        /// <summary>
        /// Checks if the shape is *going* to collide with the given offset.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool IsColliding(Shape shape, PositionOffset offset)
        {
            return shape.ToList().Any(tuple =>
            {
                var (x, y) = tuple;
                return Value[y + offset.Y, x + offset.X] != Colors.Transparent;
            });
        }

        /// <summary>
        /// Checks if the shape is *going* outside the grid bounds with the given offset.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool IsOutsideBounds(Shape shape, PositionOffset offset)
        {
            return shape.ToList().Any(tuple =>
            {
                var (x, y) = tuple;
                var xWithOffset = x + offset.X;
                var yWithOffset = y + offset.Y;
                return yWithOffset < 0 ||
                       xWithOffset < 0 ||
                       yWithOffset > Height - 1 ||
                       xWithOffset > Width - 1;
            });
        }

        /// <summary>
        /// When a shape is marked as static, it is inserted in the board's values.
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(Shape shape)
        {
            shape.ToList().ForEach(tuple =>
            {
                var (x, y) = tuple;
                Value[y, x] = shape.Color;
            });
        }

        /// <summary>
        /// Resets the board representation to its default state.
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < Height; i++) ClearRow(i);
        }
        
        /// <summary>
        /// Clears a row on the given y position.
        /// </summary>
        /// <param name="row"></param>
        private void ClearRow(int row)
        {
            for (var i = 0; i < Width; i++) Value[row, i] = Colors.Transparent;
        }

        /// <summary>
        /// Removes rows that are filled.
        /// </summary>
        /// <returns>Amount of rows that have been cleared</returns>
        public int ClearFullRows()
        {
            var rowCount = 0;
            for (var row = 0; row < Height; row++)
            {
                var blockCount = 0;
                
                // Count how many rows are full
                for (var column = 0; column < Width; column++)
                {
                    if (Value[row, column] == Colors.Transparent) continue;

                    blockCount++;
                }

                if (blockCount != Width) continue;

                // Clear the give row if it's full
                ClearRow(row);
                rowCount++;

                // After clearing the row, move every block that is above the removed line
                // downwards by swapping the values from top to bottom
                for (var y = Height - 1; y > 0; y--)
                for (var x = 0; x < Width; x++)
                {
                    if (y > row || Value[y, x] == Colors.Transparent) continue;

                    Value[y + 1, x] = Value[y, x];
                    Value[y, x] = Colors.Transparent;
                }
            }

            return rowCount;
        }
    }
}
