
namespace CQCS.QuantumWalks.Grid2D
{
    /// <summary>
    /// Provides helper methods to calculate coordinates on the torus.
    /// </summary>
    public class Torus
    {
        /// <summary>
		/// Calculates X coordinate in torus of given width.
		/// </summary>
		public static int X(int x, int width)
		{
            if (x >= width) x = x - width;
			if (x < 0) x = width + x;

			return x;
		}

        /// <summary>
        /// Calculates Y coordinate in torus of given height.
        /// </summary>
        public static int Y(int y, int height)
        {
            if (y >= height) y = y - height;
            if (y < 0) y = height + y;

            return y;
        }
    }
}
