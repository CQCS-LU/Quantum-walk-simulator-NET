using System.Collections.Generic;

namespace CQCS.QuantumWalks.Grid2D
{
    /// <summary>
    /// Provides helper methods to mark grid vertices.
    /// </summary>
    public static class MarkedVertices
    {
        /// <summary>
        /// Marks perimiter of k x k square with optional starting point.
        /// </summary>
        public static Vertex[] Perimiter(int k, int x_start = 0, int y_start = 0)
        {
            var markedVertices = new List<Vertex>();
            
            for (int x = x_start; x < x_start + k - 1; x++)
                markedVertices.Add(new Vertex(x, y_start));

            for (int y = y_start; y < y_start + k - 1; y++)
                markedVertices.Add(new Vertex(x_start + k - 1, y));

            for (int x = x_start + k - 1; x > x_start; x--)
                markedVertices.Add(new Vertex(x, y_start + k - 1));

            for (int y = y_start + k - 1; y > y_start; y--)
                markedVertices.Add(new Vertex(x_start, y));

            return markedVertices.ToArray();
        }

        /// <summary>
        /// Marks dashed (i.e. every second point is not marked) perimiter of k x k square with optional starting point.
        /// </summary>
        public static Vertex[] DashedPerimiter(int k, int x_start = 0, int y_start = 0)
        {
            if (k % 2 != 0)
                throw new System.ApplicationException("k must be even");

            var markedVertices = new List<Vertex>();
            var skipNext = false;

            foreach (var vertex in Perimiter(k, x_start, y_start))
            {
                if (!skipNext)
                    markedVertices.Add(vertex);

                skipNext = !skipNext;
            }

            return markedVertices.ToArray();
        }

        /// <summary>
        /// Marks k x k square with optional step and starting point.
        /// </summary>
        public static Vertex[] Square(int k, int x_start = 0, int y_start = 0)
        {
            return Rect(k, k, 1, 1, x_start, y_start);
        }

        
        /// <summary>
        /// Marks x_k x y_k rectangle with optional step and starting point.
        /// </summary>
        public static Vertex[] Rect(int x_k, int y_k, int x_step = 1, int y_step = 1, int x_start = 0, int y_start = 0)
        {
            var markedVertices = new List<Vertex>();

            for (int x_i = 0; x_i < x_k; x_i++)
            for (int y_i = 0; y_i < y_k; y_i++)
            {
                int x = x_start + x_i*x_step;
                int y = y_start + y_i*y_step;
                markedVertices.Add(new Vertex(x, y));
            }

            return markedVertices.ToArray();
        }

        /// <summary>
        /// Returns requested number of marked vertices chosen at random.
        /// </summary>
        public static Vertex[] Random(int width, int height, int count = 1)
        {
            var markedVertices = new List<Vertex>();

            var rand = new System.Random();

            for (int i = 0; i < count; i++)
            {
                int x = rand.Next(width);
                int y = rand.Next(height);
                markedVertices.Add(new Vertex(x, y));
            }

            return markedVertices.ToArray();
        }
    }
}
