using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Grid2D
{
    public interface IQuantumWalkSimulator2D : IQuantumWalkSimulator
    {
        void MarkVertex(Vertex v);

        void MarkVertex(int x, int y);

        void UnMarkVertex(Vertex v);

        void UnMarkVertex(int x, int y);

        bool IsVertexMarked(Vertex v);

        bool IsVertexMarked(int x, int y);
    }
}
