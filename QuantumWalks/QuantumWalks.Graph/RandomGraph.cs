using System;

namespace CQCS.QuantumWalks.Graph
{
    public static class RandomGraph
    {
        public static Graph Generate(int numberOfVertices, double edgeProbability)
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
    }
}
