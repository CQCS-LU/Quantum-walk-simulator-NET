using System.Linq;
using System.Collections.Generic;

namespace CQCS.QuantumWalks.Graph
{
    /// <summary>
    /// Represents an edge of the graph.
    /// </summary>
    public class Edge
    {
        public Vertex V1;
        public double A1;   // Amplitude corresponding to V1
        public Vertex V2;
        public double A2;   // Amplitude corresponding to V2

        public double GetAmplitude (Vertex v)
        {
            return (v == V1) ? A1 : A2;
        }

        public void SetAmplitude (Vertex v, double a)
        {
            if (v == V1)
                A1 = a;
            else
                A2 = a;
        }

        public Vertex GetOtherVertex (Vertex v)
        {
            return (v == V1) ? V2 : V1;
        }
    }

    /// <summary>
    /// Represents a vertex of the graph.
    /// </summary>
    public class Vertex 
    {
        public int Index;
        public List<Edge> Edges;
        public bool IsMarked;
    }

    /// <summary>
    /// Represents a graph.
    /// </summary>
    public class Graph 
    {
        public readonly List<Vertex> Verteces;
        public readonly List<Edge> Edges;

        public Graph(int numberOfVertices)
        {
            Verteces = new List<Vertex>();
            Edges = new List<Edge>();

            for (int i = 0; i < numberOfVertices; i++)
                Verteces.Add(new Vertex { Index = i, Edges = new List<Edge>() });
        }

        public bool HasEdge(int i, int j)
        {
            return Verteces[i].Edges.Any(e => e.V2.Index == j);
        }

        public void AddEdge(int i, int j)
        {
            if (HasEdge(i, j))
                return;

            var e = new Edge { V1 = Verteces[j], V2 = Verteces[i] };
            Verteces[i].Edges.Add(e);
            Verteces[j].Edges.Add(e);
            Edges.Add(e);
        }
    }
}
