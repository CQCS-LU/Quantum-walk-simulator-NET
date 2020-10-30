using System;
using System.Linq;
using System.Collections.Generic;

using CQCS.QuantumWalks.Common;
using CQCS.QuantumWalks.Grid2D;
using Lackadaisical = CQCS.QuantumWalks.Grid2D.Lackadaisical;

namespace CQCS.QuantumWalks.Experiments.Grid2D.DataGenerators
{
    /// <summary>
    /// Numerical experiments related to [https://arxiv.org/pdf/1808.00672.pdf] paper.
    /// </summary>
	public static class StepCountAndProbability_2D_Rect
    {
        public static void RunCoined()
        {
            const int gridSize = 100;

            var qws = new QuantumWalkSimulatorCoinedRectangle(gridSize, gridSize);
            qws.MarkVertex(0, 0);

            Run(qws);
        }


        public static void RunLackadaisical_CompareWeights()
        {
            int gridSize = 200;
            int N = gridSize * gridSize;

            for (int i = 1; i <= 5; i++)
            {
                // var markedVertices = Enumerable.Range(0, m).Select(i => new Vertex(0, 10*i)).ToArray();

                int m = 20;
                var markedVertices = MarkedVertices.Random(gridSize, gridSize, m);

                var qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: SelfLoopWeightWong(N, m));
                qws.MarkVertices(markedVertices);
                Console.Write("[Wong] ");
                Run(qws, OutputMode.Final, maxSteps: 1000);

                qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: SelfLoopWeightSaha(N, m));
                qws.MarkVertices(markedVertices);
                Console.Write("[Saha] ");
                Run(qws, OutputMode.Final, maxSteps: 1000);

                qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: SelfLoopWeightNN1(N, m));
                qws.MarkVertices(markedVertices);
                Console.Write("[NN01] ");
                Run(qws, OutputMode.Final, maxSteps: 1000);

                qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: SelfLoopWeightNN2(N, m));
                qws.MarkVertices(markedVertices);
                Console.Write("[NN02] ");
                Run(qws, OutputMode.Final, maxSteps: 1000);

                Console.WriteLine();
            }
        }


        #region Self loop weights

        private static double SelfLoopWeightWong(int N, int k)
        {
            return 4.0 / N;
        }

        private static double SelfLoopWeightSaha(int N, int k)
        {
            //return Math.Round(4.0 / (N * (k + Math.Floor(Math.Sqrt(k) / 2.0))), 5);
            return 4.0 / (N * (k + Math.Floor(Math.Sqrt(k) / 2.0)));
        }

        private static double SelfLoopWeightNN1(int N, int k)
        {
            return 4.0 * k / N;
        }

        private static double SelfLoopWeightNN2(int N, int k)
        {
            return Math.Max(4.0 * (k - Math.Sqrt(k)) / N, 0);
        }

        #endregion

        public static void RunLackadaisical()
        {
            int gridSize = 200;
            int N = gridSize * gridSize;

            var rand = new Random();

            var steps = new List<int>();
            var probabilities = new List<double>();

            int m = 10;
            for (int i = 1; i <= 100; i++)
            {
                //var markedVertices = Enumerable.Range(0, m).Select(i => new Vertex(0, 10*i)).ToArray();
                var markedVertices = Enumerable.Range(0, m).Select(_ => new Vertex(rand.Next(gridSize), rand.Next(gridSize))).ToArray();

                double selfLoopWeight = SelfLoopWeightSaha(N, m);

                var qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: selfLoopWeight);
                qws.MarkVertices(markedVertices);
                Run(qws, OutputMode.Final, maxSteps: 1000);

                steps.Add(qws.T);
                probabilities.Add(qws.GetMarkedVertexProbability());
            }

            Console.WriteLine("Steps: " + steps.Average());
            Console.WriteLine("Pr: " + probabilities.Average());
        }

        private static void GetMaxProbability(IQuantumWalkSimulator qws, out double maxPr, out int maxPrStep, int? steps = null)
        {
            const double PrTreshold = 0.1;

            maxPrStep = qws.T;
            maxPr = qws.GetMarkedVertexProbability();

            while (true)
            {
                if (qws.GetScalarProduct() < 0)
                    break;

                if (steps.HasValue && qws.T >= steps)
                    break;

                var pr = qws.GetMarkedVertexProbability();

                if (pr > maxPr)
                {
                    maxPrStep = qws.T;
                    maxPr = qws.GetMarkedVertexProbability();
                }

                if (!steps.HasValue && (pr < maxPr - PrTreshold))
                    break;

                qws.Run(stepCount: 1);
            }
        }


        public static void RunLackadaisical_OptimalWeight()
        {
            int gridSize = 100;
            int N = gridSize * gridSize;

            int ky = 3;
            for (int kx = 1; kx < 20; kx += 2)
            {
                int k = kx*ky;
                var markedVertices = MarkedVertices.Rect(kx, ky);

                double maxPr = 0;
                int maxPrSteps = 0;
                double maxPrOverlap = 0; 
                decimal a_opt = 0;

                for (var a = 0.9m/k; a <= 1.2m/k; a += 0.001m)
                {
                    double selfLoopWeight = 4.0 * (double)a / N;

                    var qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: selfLoopWeight);
                    qws.MarkVertices(markedVertices);
                    Run(qws, OutputMode.None, maxSteps:5000);

                    var pr = qws.GetMarkedVertexProbability();

                    if (pr > maxPr)
                    {
                        a_opt = a;
                        maxPr = pr;
                        maxPrSteps = qws.T;
                        maxPrOverlap = qws.GetScalarProduct();
                    }
                }
                   
                Console.WriteLine($"{kx}x{ky}; {a_opt}; {maxPrSteps}; {maxPr}; {maxPrOverlap}");
            }
        }

        private enum OutputMode { StepByStep, Final, None }

        private static void Run(IQuantumWalkSimulator qws, OutputMode output = OutputMode.StepByStep, int? maxSteps = null)
        {
            maxSteps = maxSteps ?? 5*qws.N;

            if (output == OutputMode.StepByStep)
                Console.WriteLine("Step; Pr; Overlap");

            var prevStepScalarProduct = qws.GetScalarProduct();
            var prevStepProbability = qws.GetMarkedVertexProbability();

            while (true)
            {
                if (qws.GetScalarProduct() < 0)
                    break;

                if (qws.GetScalarProduct() > prevStepScalarProduct && qws.T > 500)
                    break;

                if (qws.T >= maxSteps)
                    break;

                prevStepProbability = qws.GetMarkedVertexProbability();
                prevStepScalarProduct = qws.GetScalarProduct();

                qws.Run(stepCount: 1);

                if (output == OutputMode.StepByStep)
                    Console.WriteLine($"{qws.T}; {qws.GetMarkedVertexProbability()}; {qws.GetScalarProduct()}");
            }

            if (output == OutputMode.Final)
                Console.WriteLine($"{qws.T}; {qws.GetMarkedVertexProbability()}; {qws.GetScalarProduct()}");
        }
    }
}
