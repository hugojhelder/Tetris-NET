using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace TetrisClient.Logic
{
    /// <summary>
    /// This class is used to represent all the different types of Tetronomino
    ///
    /// The following link has been used to collect most of these shapes: https://tetris.fandom.com/wiki/Tetromino
    /// The following link has been used to determine how the shape should be rotated: https://tetris.fandom.com/wiki/SRS
    /// </summary>
    public class Shape
    {
        #region Shape definitions

        /// <summary>
        /// The I Tetronomino.
        /// </summary>
        private static Shape TetronominoI =>
            new(
                new[,]
                {
                    {0, 0, 0, 0},
                    {1, 1, 1, 1},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0},
                },
                Color.FromRgb(28, 181, 201)
            );

        /// <summary>
        /// The J Tetronomino.
        /// </summary>
        private static Shape TetronominoJ =>
            new(
                new[,]
                {
                    {1, 0, 0},
                    {1, 1, 1},
                    {0, 0, 0},
                },
                Color.FromRgb(28, 51, 201)
            );

        /// <summary>
        /// The L Tetronomino.
        /// </summary>
        private static Shape TetronominoL =>
            new(
                new[,]
                {
                    {0, 0, 1},
                    {1, 1, 1},
                    {0, 0, 0},
                },
                Colors.Orange
            );

        /// <summary>
        /// The O Tetronomino.
        /// </summary>
        private static Shape TetronominoO =>
            new(
                new[,]
                {
                    {1, 1},
                    {1, 1}
                },
                Color.FromRgb(199, 209, 18)
            );

        /// <summary>
        /// The S Tetronomino.
        /// </summary>
        private static Shape TetronominoS =>
            new(
                new[,]
                {
                    {0, 1, 1},
                    {1, 1, 0},
                    {0, 0, 0}
                },
                Colors.Green
            );

        /// <summary>
        /// The T Tetronomino.
        /// </summary>
        private static Shape TetronominoT =>
            new(
                new[,]
                {
                    {0, 1, 0},
                    {1, 1, 1},
                    {0, 0, 0}
                },
                Colors.Purple
            );

        /// <summary>
        /// The Z Tetronomino.
        /// </summary>
        private static Shape TetronominoZ =>
            new(
                new[,]
                {
                    {1, 1, 0},
                    {0, 1, 1},
                    {0, 0, 0},
                },
                Colors.Red
            );

        /// <summary>
        /// All types of Tetronomino in a list.
        /// </summary>
        public static List<Shape> Shapes =>
            new()
            {
                TetronominoI,
                TetronominoJ,
                TetronominoL,
                TetronominoO,
                TetronominoS,
                TetronominoZ,
                TetronominoT
            };

        #endregion

        #region Public properties
        
        /// <summary>
        /// Represents the color of the shape.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Represents the shape in a matrix.
        /// </summary>
        public Matrix Matrix { get; set; }

        /// <summary>
        /// Represents the positional offset for the individual items in the <see cref="Matrix"/>
        /// </summary>
        public PositionOffset Offset { get; set; }

        /// <summary>
        /// Represents the state that the shape is currently in.
        /// </summary>
        public ShapeState State { get; set; }
        
        #endregion

        private Shape(int[,] initialValue, Color color)
        {
            Color = color;
            Matrix = new Matrix(initialValue);
            Offset = new PositionOffset(0, 0);
            State = ShapeState.Moving;
        }

        /// <summary>
        /// Positions the shape in the middle by giving the column width.
        /// </summary>
        /// <param name="columnWidth"></param>
        public void PositionInMiddle(int columnWidth)
        {
            Offset = new PositionOffset((columnWidth - Matrix.Value.GetLength(0)) / 2, 0);
        }

        /// <summary>
        /// Rotates a shape and checks if it needs to be 'kicked' back.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="boardWidth"></param>
        public void Rotate(Direction direction, int boardWidth)
        {
            // Determine how to rotate the matrix
            Matrix = direction == Direction.Left
                ? Matrix.Rotate90CounterClockwise()
                : Matrix.Rotate90();
            
            // Get the offset from the position that is exceeding the board width
            var offset = this.ToList()
                .Select(tuple => tuple.Item1)
                .Where(x => x < 0 || x > boardWidth)
                .Select(x => new PositionOffset(x, 0))
                .FirstOrDefault();

            if (offset == null) return;
            
            var isNegative = offset.X < 0;
            Offset.X += isNegative 
                // Negative means the Tetromino is exceeding the board at the left side
                // Number is already negative, but needs to be positive by using a '-' sign
                ? -offset.X 
                // Positive means that the Tetromino is exceeding the board the right side
                // Subtract the amount necessary by using a '-' sign
                : -(offset.X - boardWidth);
        }
        
        /// <summary>
        /// Moves the shape to the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public void MoveTo(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    Offset.IncrementX();
                    break;
                case Direction.Left:
                    Offset.DecrementX();
                    break;
                case Direction.Down:
                    Offset.IncrementY();
                    break;
            }
        }
    }

    /// <summary>
    /// The state of the shape.
    /// </summary>
    public enum ShapeState
    {
        Moving,
        Static
    }
}
