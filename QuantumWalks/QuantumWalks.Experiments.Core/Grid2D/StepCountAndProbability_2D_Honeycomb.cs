using System;
using System.Linq;

using CQCS.QuantumWalks.Common;
using CQCS.QuantumWalks.Grid2D;

namespace CQCS.QuantumWalks.Experiments.Grid2D.DataGenerators
{
    /// <summary>
    /// Numerical experiments on honeycomb 2D grid with multiple marked vertices.
    /// </summary>
	public static class StepCountAndProbability_2D_Honeycomb
    {
        public static void Run()
        {
            const int gridSize = 200;

            for (int m = 10; m <= 10; m++)
                OptimalWeight(gridSize, m);
        }

        private static void OptimalWeight(int gridSize, int m)
        {
            int N = gridSize * gridSize;

            var markedVertices = Enumerable.Range(0, m).Select(i => new Vertex(0, 10*i)).ToArray();

            double maxPr = 0;
            int maxPrSteps = 0;
            double maxPrOverlap = 0; 
            decimal a_opt = 0;

            for (decimal a = m-2m; a <= m; a += 0.01m)
            {
                double selfLoopWeight = 3.0 * (double)a / N;

                var qws = new CQCS.QuantumWalks.Grid2D.Lackadaisical.QuantumWalkSimulatorHoneycomb(gridSize, gridSize, selfLoopWeight: selfLoopWeight);
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
                
            Console.WriteLine($"{gridSize}x{gridSize}; {m}; {a_opt}; {maxPrSteps}; {maxPr}; {maxPrOverlap}");
        }

        private enum OutputMode { StepByStep, Final, None }

        private static void Run(IQuantumWalkSimulator qws, OutputMode output = OutputMode.StepByStep, int? maxSteps = null)
        {
            maxSteps = maxSteps ?? 5 * qws.N;

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
