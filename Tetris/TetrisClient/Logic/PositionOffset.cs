namespace TetrisClient.Logic
{
    /// <summary>
    /// Represents the offset used int the shapes.
    ///
    /// Used for correctly positioning the shape in the grid.
    /// </summary>
    public class PositionOffset
    {
        /// <summary>
        /// Gives the offset relative to the direction given.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static PositionOffset From(Direction direction)
        {
            return direction switch
            {
                Direction.Left => new PositionOffset(-1, 0),
                Direction.Right => new PositionOffset(1, 0),
                Direction.Down => new PositionOffset(0, 1),
                _ => new PositionOffset(0, 0),
            };
        }

        /// <summary>
        /// The X offset.
        /// </summary>
        public int X { get; set; }
        
        /// <summary>
        /// The Y offset.
        /// </summary>
        public int Y { get; private set; }

        public PositionOffset(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Increments the X offset.
        /// </summary>
        public void IncrementX() => X++;

        /// <summary>
        /// Decrements the Y Offset.
        /// </summary>
        public void DecrementX() => X--;

        /// <summary>
        /// Increments the Y offset.
        /// </summary>
        public void IncrementY() => Y++;

        public override string ToString()
        {
            return $"x: {X} y: {Y}";
        }
    }
}
