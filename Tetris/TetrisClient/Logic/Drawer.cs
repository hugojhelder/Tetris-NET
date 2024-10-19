using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TetrisClient.Logic
{
    /// <summary>
    /// Class for handling drawing rectangles in grids.
    /// </summary>
    public static class Drawer
    {
        /// <summary>
        /// Draws a shape in the given grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="shape"></param>
        public static void Draw(Grid grid, Shape shape)
        {
            shape.ToList().ForEach(tuple => DrawRectangle(grid, shape.Color, tuple.Item1, tuple.Item2));
        }

        /// <summary>
        /// Draws the board in the given grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="board"></param>
        public static void Draw(Grid grid, Board board)
        {
            board.ToList().ForEach(tuple => DrawRectangle(grid, tuple.Item1, tuple.Item2, tuple.Item3));
        }

        /// <summary>
        /// Draws a rectangle in the given grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="color"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void DrawRectangle(Grid grid, Color color, int x, int y)
        {
            var rectangle = GenerateRectangle(color);
            grid.Children.Add(rectangle);

            Grid.SetColumn(rectangle, x);
            Grid.SetRow(rectangle, y);
        }

        /// <summary>
        /// Generates a rectangle with color that has been given to it.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Rectangle GenerateRectangle(Color color)
        {
            var colorBrush = new SolidColorBrush {Color = color};

            return new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = colorBrush,
                RadiusX = 2,
                RadiusY = 2,
            };
        }
    }
}
