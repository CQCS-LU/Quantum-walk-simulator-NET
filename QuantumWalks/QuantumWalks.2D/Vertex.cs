using System.Collections.Generic;

namespace CQCS.QuantumWalks.Grid2D
{
    /// <summary>
    /// Represents a vertex of 2D grid.
    /// </summary>
    public struct Vertex
    {
        public int X;
        public int Y;

        public Vertex(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() 
        {
            return $"({X}, {Y})";
        }
    }

    /// <summary>
    /// Provides static helper methods to mark/unmark vertices.
    /// </summary>
    public static class MarkedVertexHelper
    {
        public static void MarkVertices(this IQuantumWalkSimulator2D qws, params Vertex[] vertices)
        {
            foreach (var v in vertices)
                qws.MarkVertex(v);
        }

        public static void UnMarkVertices(this IQuantumWalkSimulator2D qws, params Vertex[] vertices)
        {
            foreach (var v in vertices)
                qws.UnMarkVertex(v);
        }
    }
}
