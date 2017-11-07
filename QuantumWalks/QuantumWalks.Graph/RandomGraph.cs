using System;
using System.Linq;
using System.Collections.Generic;

namespace CQCS.QuantumWalks.Graph
{
    public static class RandomGraph
    {
        /// <summary>
        /// Generates an Erdős–Rényi random graph with the given number of vertices.
        /// </summary>
        public static Graph GenerateErdosRenyi(int numberOfVertices, double edgeProbability)
        {
            var g = new Graph(numberOfVertices);

            // Add edges
            Random rand = new Random();

            for (int i = 0; i < numberOfVertices; i++)
                for (int j = 0; j < i; j++)
                {
                    if (rand.NextDouble() < edgeProbability)
                    {
                        g.AddEdge(i, j);
                    }
                }

            return g;
        }

        /// <summary>
        /// Generates an Barabási–Albert random graph with the given number of vertices.
        /// </summary>
        public static Graph GenerateBarabasiAlbert(int numberOfVertices, int initialNumberOfVertices = 2, int edgesToAdd = 2)
        {
            var g = new Graph(numberOfVertices);

            // Add initial vertices
            for (int i = 0; i < initialNumberOfVertices; i++)
                for (int j = 0; j < i; j++)
                {
                    g.AddEdge(0, 1);
                }

            var totalDegree = g.Vertices.Sum(v => v.Degree);

            // Add edges
            Random rand = new Random();

            for (int i = initialNumberOfVertices; i < numberOfVertices; i++)
            {
                var newEdges = new List<int>();

                // Choose edges to add
                while (newEdges.Count < edgesToAdd)
                {
                    var j = rand.Next(i);
                    double edgeProbability = ((double)g.Vertices[j].Degree) / totalDegree;

                    if (rand.NextDouble() < edgeProbability)
                    {
                        bool alreadyAdded = newEdges.Contains(j);
                        if (!alreadyAdded)
                        {
                            newEdges.Add(j);
                        }
                    }
                }

                // Add edges
                foreach (int j in newEdges)
                {
                    g.AddEdge(i, j);
                    totalDegree += 2;
                }
            }

            return g;
        }
    }
}
