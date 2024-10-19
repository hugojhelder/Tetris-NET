using System;
using System.Windows.Controls;

namespace TetrisClient.Logic
{
    /// <summary>
    /// Represents the game logic.
    /// </summary>
    public class Game
    {
        #region Public properties
        
        /// <summary>
        /// The current level of the game.
        /// </summary>
        public int Level { get; private set; }
        
        /// <summary>
        /// The current lines removed in the game.
        /// </summary>
        public int Lines { get; private set; }
        
        /// <summary>
        /// The current score in the game.
        /// </summary>
        public int Score { get; private set; }
        
        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState State { get; private set; }

        #endregion
        
        #region Constants
        
        /// <summary>
        /// Points given for clearing a single line.
        /// </summary>
        private const int SingleLineClear = 100;
        
        /// <summary>
        /// Points given for clearing a double line.
        /// </summary>
        private const int DoubleLineClear = 100;
        
        /// <summary>
        /// Points given for clearing a triple line.
        /// </summary>
        private const int TripleLineClear = 300;
        
        /// <summary>
        /// Points given for clearinga tetris line.
        /// </summary>
        private const int TetrisLineClear = 800;
        
        /// <summary>
        /// Indicates how many lines needed per level.
        /// </summary>
        private const int RowsPerLevel = 10;
        
        #endregion

        #region Private properties
        
        /// <summary>
        /// The board that will be used in the game.
        /// </summary>
        private readonly Board _board;
        
        /// <summary>
        /// The playing grid that will be used in the game.
        /// </summary>
        private readonly Grid _grid;
        
        /// <summary>
        /// The preview grid that will be used in the game.
        /// </summary>
        private readonly Grid _preview;
        
        /// <summary>
        /// The random instance that will be used in the game to generate shapes.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// The current shape that will be controlled.
        /// </summary>
        private Shape _currentShape;
        
        /// <summary>
        /// The next shape that is rendered in the preview grid.
        /// </summary>
        private Shape _nextShape;
        
        #endregion

        public Game(Grid grid, Grid preview, int seed = 0)
        {
            (Level, Lines, Score) = (1, 0, 0);
            State = GameState.Waiting;

            _board = new Board(grid.RowDefinitions.Count, grid.ColumnDefinitions.Count);
            _grid = grid;
            _preview = preview;
            _random = new Random(seed == 0 ? Guid.NewGuid().GetHashCode() : seed);

            _currentShape = NextRandomShape();
            _nextShape = NextRandomShape();
        }

        /// <summary>
        /// Method that is used to start the game.
        /// </summary>
        public void StartGame()
        {
            _currentShape = NextRandomShape();
            _nextShape = NextRandomShape();
            _board.Clear();
            _grid.Children.Clear();

            Level = 1; // Level is 1 by default
            Lines = 0;
            Score = 0;

            State = GameState.Started;
        }

        /// <summary>
        /// Method that is used to draw the shapes, both next and current.
        /// </summary>
        public void DrawShapes()
        {
            if (State == GameState.Ended) return;

            if (_currentShape.State == ShapeState.Static)
            {
                _board.AddShape(_currentShape);

                HandleScoring();

                _currentShape = _nextShape;
                CenterShape(_currentShape);
                _nextShape = NextRandomShape();
            }

            if (_board.IsColliding(_currentShape, PositionOffset.From(Direction.None)))
            {
                EndGame();
                return;
            }

            _grid.Children.Clear();

            Drawer.Draw(_grid, _currentShape);
            Drawer.Draw(_grid, _board);

            // There is a scenario when the preview is null, if so return early
            if (_preview == null) return;

            _preview.Children.Clear();
            _nextShape.Offset = new PositionOffset(1, 1);
            Drawer.Draw(_preview, _nextShape);
        }

        /// <summary>
        /// Handles the scoring of the game.
        /// </summary>
        private void HandleScoring()
        {
            var rowsCleared = _board.ClearFullRows();
            Lines += rowsCleared;
            Level = 1 + Lines / RowsPerLevel;

            CalculateScoreFromClearingRows(rowsCleared);
        }

        /// <summary>
        /// Generates the next random shape.
        /// </summary>
        /// <returns>A random shape</returns>
        private Shape NextRandomShape()
        {
            var randomInt = _random.Next(0, Shape.Shapes.Count);
            var shape = Shape.Shapes[randomInt];
            CenterShape(shape);

            return shape;
        }

        /// <summary>
        /// Centers the shape in the playing grid.
        /// </summary>
        /// <param name="shape"></param>
        private void CenterShape(Shape shape) => shape.PositionInMiddle(_grid.ColumnDefinitions.Count);

        /// <summary>
        /// Rotates the current shape in the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public void RotateCurrentShape(Direction direction)
        {
            var beforeRotationPositions = _currentShape.Matrix;
            _currentShape.Rotate(direction, _board.Width - 1);

            if (_board.IsColliding(_currentShape, PositionOffset.From(Direction.None)))
            {
                // Shape collides with other shape, reset before the rotation happened
                _currentShape.Matrix = beforeRotationPositions;
            }

            DrawShapes();
        }

        /// <summary>
        /// Moves the shape in the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public void MoveCurrentShape(Direction direction)
        {
            var toBeAddedOffset = PositionOffset.From(direction);

            // First check has to be the bounds check, otherwise the program will crash
            if (_board.IsOutsideBounds(_currentShape, toBeAddedOffset) ||
                _board.IsColliding(_currentShape, toBeAddedOffset)
            )
            {
                // In this case the shape has reached the bottom, set its state to Static
                if (direction == Direction.Down)
                {
                    _currentShape.State = ShapeState.Static;
                }

                return;
            }

            _currentShape.MoveTo(direction);
            DrawShapes();
        }

        /// <summary>
        /// Hardrops the current shape untill it can't be dropped anymore.
        /// </summary>
        public void HardDrop()
        {
            while (_currentShape.State != ShapeState.Static)
            {
                MoveCurrentShape(Direction.Down);

                // Hard dropping increments score by 2
                // https://tetris.com/play-tetris (rules section)
                Score += 2;
            }
        }

        /// <summary>
        /// Calculates the score according to the modern tetris scoring rules.
        /// </summary>
        /// <param name="rows"></param>
        private void CalculateScoreFromClearingRows(int rows)
        {
            // Based on scoring of the modern tetris version
            // https://tetris.com/play-tetris (score section)
            Score += rows switch
            {
                0 => 0,
                1 => SingleLineClear * Level,
                2 => DoubleLineClear * Level,
                3 => TripleLineClear * Level,
                _ => TetrisLineClear * Level
            };
        }

        /// <summary>
        /// Increments the score.
        /// </summary>
        public void IncrementScore() => Score++;

        /// <summary>
        /// Resets the score.
        /// </summary>
        public void ResetScore() => Score = 0;

        /// <summary>
        /// Ends the game.
        /// </summary>
        public void EndGame()
        {
            State = GameState.Ended;
        }
    }

    /// <summary>
    /// The state of the game.
    /// </summary>
    public enum GameState
    {
        Waiting,
        Started,
        Ended
    }
}
