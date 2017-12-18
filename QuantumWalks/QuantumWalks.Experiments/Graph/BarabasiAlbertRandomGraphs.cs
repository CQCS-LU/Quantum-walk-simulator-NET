using System;

using CQCS.QuantumWalks.Graph;
using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Experiments.Graph
{
    internal static class BarabasiAlbertRandomGraphs
    {
        public static void Run()
        {
            try
            {
                const int N = 1000;
                const int Iterations = 10;

                Console.WriteLine("{Max degree}; {Min Overlap}; {Max Pr}");

                for (int i = 0; i < Iterations; i++)
                {
                    // Generate a Barabasi-Albert random graph
                    var g = RandomGraph.GenerateBarabasiAlbert(N, 2);

                    // Find a pair of connected vertices of highest possible degree
                    Vertex v1 = null;
                    Vertex v2 = null;
                    int maxDegree = 0;

                    foreach (var e in g.Edges)
                    {
                        if (e.V1.Degree == e.V2.Degree)
                        {
                            var degree = e.V1.Degree;

                            if (degree > maxDegree)
                            {
                                maxDegree = degree;
                                v1 = e.V1;
                                v2 = e.V2;
                            }
                        }
                    }

                    Console.Write($"{maxDegree}; ");

                    // Mark the pair and run the simulation.
                    // Find the highest probability of finding a marked vertex
                    var qws = new QuantumWalkSimulator(g);

                    qws.MarkVertex(v1.Index);
                    qws.MarkVertex(v2.Index);

                    WriteMinOverlapMaxProbability(qws, 2*N);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void WriteMinOverlapMaxProbability(IQuantumWalkSimulator qws, int? steps = null)
        {
            double minOverlap = 1;
            double maxProbability = 0;

            while (true)
            {
                if (steps.HasValue && (qws.T > steps.Value))
                    break;

                if (qws.GetScalarProduct() < 0)
                    break;

                qws.Run();

                if (qws.T % 50 == 0)
                    Console.Title = "T = " + qws.T;

                var curOverlap = qws.GetScalarProduct();
                minOverlap = Math.Min(curOverlap, minOverlap);

                var curProbability = qws.GetMarkedVertexProbability();
                maxProbability = Math.Max(curProbability, maxProbability);
            }

            Console.WriteLine($"{minOverlap}; {maxProbability}");
        }
    }
}
