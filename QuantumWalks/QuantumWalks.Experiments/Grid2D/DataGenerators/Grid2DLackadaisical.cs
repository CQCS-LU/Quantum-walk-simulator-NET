using System;

using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Grid2D.DataGenerators
{
    /// <summary>
    /// Numerical experiments on lackadaisical QW on 2D - rectangle, triangle and honeycomb - grids.
    /// Base for the arxiv.org paper http://arxiv.org/abs/2007.13564
    /// </summary>
	public class Grid2DLackadaisical
	{
        public static void Run ()
        {
            // Compare non-lackadiasical QW on tri, rect and hex 2D grid
            //StepByStep_TriangleRectangleHoneycomb();

            // Show that the running time of the walk depends on a weight of the self-loop
            //LackadaisicalTriangle_StepByStep_DifferentWeights();
            //LackadaisicalRectangle_StepByStep_DifferentWeights();
            //LackadaisicalHoneycomb_StepByStep_DifferentWeights();

            // Find optimal weights of the self-loop
            //LackadaisicalTriangle_FindOptimalWeight();
            //LackadaisicalRectangle_FindOptimalWeight();
            //LackadaisicalHoneycomb_FindOptimalWeight();

            // Check optimal weights for different grid sizes
            //LackadaisicalTriangle_CheckOptimalWeight();
            //LackadaisicalRectangle_CheckOptimalWeight();
            //LackadaisicalHoneycomb_CheckOptimalWeight();

            // Rutime for different n
            //LackadaisicalTriangle_PrintRuntime();
            //LackadaisicalRectangle_PrintRuntime();
            LackadaisicalHoneycomb_PrintRuntime();
        }


        #region private static void TestImplementation() { ... }

        private static void TestImplementation()
        {
            TestLackadaisicalTriangle(100, new Vertex(0, 0));
            TestLackadaisicalRectangle(100, new Vertex(0, 0));
            TestLackadaisicalHoneycomb(100, new Vertex(0, 0));
        }

        private static void TestLackadaisicalRectangle(int gridSize, params Vertex[] markedVertices)
        {
            var qws1 = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight:0);
            qws1.MarkVertices(markedVertices);

            var qws2 = new QuantumWalkSimulatorCoinedRectangle(gridSize, gridSize);
            qws2.MarkVertices(markedVertices);

            CompareSimulators(qws1, qws2);
        }

        private static void TestLackadaisicalTriangle(int gridSize, params Vertex[] markedVertices)
        {
            var qws1 = new Lackadaisical.QuantumWalkSimulatorTriangle(gridSize, gridSize, selfLoopWeight: 0);
            qws1.MarkVertices(markedVertices);

            var qws2 = new QuantumWalkSimulatorCoinedTriangle(gridSize, gridSize);
            qws2.MarkVertices(markedVertices);

            CompareSimulators(qws1, qws2);
        }

        private static void TestLackadaisicalHoneycomb(int gridSize, params Vertex[] markedVertices)
        {
            var qws1 = new Lackadaisical.QuantumWalkSimulatorHoneycomb(gridSize, gridSize, selfLoopWeight: 0);
            qws1.MarkVertices(markedVertices);

            var qws2 = new QuantumWalkSimulatorCoinedHoneycomb(gridSize, gridSize);
            qws2.MarkVertices(markedVertices);

            CompareSimulators(qws1, qws2);
        }

        private static void CompareSimulators(IQuantumWalkSimulator2D qws1, IQuantumWalkSimulator2D qws2)
        {
            while (qws1.GetScalarProduct() >= 0)
            {
                qws1.Run();
                Console.WriteLine("{0}; {1}", qws1.T, qws1.GetMarkedVertexProbability());

                qws2.Run();
                Console.WriteLine("{0}; {1}", qws2.T, qws2.GetMarkedVertexProbability());

                if (Math.Round(qws1.GetTotalProbability(), 10) != 1 ||
                    Math.Round(qws2.GetTotalProbability(), 10) != 1)
                    return;
            }
        }

        #endregion

        private static void StepByStep_TriangleRectangleHoneycomb()
        {
            int gridSize = 100;

            PrintStepByStep(new QuantumWalkSimulatorCoinedRectangle(gridSize, gridSize));
            PrintStepByStep(new QuantumWalkSimulatorCoinedTriangle(gridSize, gridSize));
            PrintStepByStep(new QuantumWalkSimulatorCoinedHoneycomb(gridSize, gridSize));
        }

        private static void PrintStepByStep(IQuantumWalkSimulator2D qws)
        {
            qws.MarkVertex(0, 0);

            Run(qws, OutputMode.StepByStep, 500);
        }


        private static void LackadaisicalTriangle_StepByStep_DifferentWeights()
        {
            LackadaisicalTriangle_StepByStep(16, 0.0, 100);
            LackadaisicalTriangle_StepByStep(16, 0.1, 100);
            LackadaisicalTriangle_StepByStep(16, 0.2, 100);
            LackadaisicalTriangle_StepByStep(16, 0.0234, 100);
            LackadaisicalTriangle_StepByStep(16, 0.005, 100);
        }

        private static void LackadaisicalTriangle_FindOptimalWeight()
        {
            int gridSize = 100;
            Lackadaisical_FindOptimalWeight(gridSize, (n, l) => new Lackadaisical.QuantumWalkSimulatorTriangle(n, n, selfLoopWeight: l));
        }

        private static void LackadaisicalTriangle_StepByStep(int gridSize, double selfLoopWeight, int? steps = null)
        {
            Console.WriteLine($"n = {gridSize}; l = {selfLoopWeight}");

            var qws = new Lackadaisical.QuantumWalkSimulatorTriangle(gridSize, gridSize, selfLoopWeight: selfLoopWeight);
            qws.MarkVertex(0, 0);

            Run(qws, OutputMode.StepByStep, maxSteps: steps);

            Console.WriteLine();
        }

        private static void LackadaisicalTriangle_CheckOptimalWeight()
        {
            LackadaisicalTriangle_CheckOptimalWeight(16);
            LackadaisicalTriangle_CheckOptimalWeight(32);
            LackadaisicalTriangle_CheckOptimalWeight(64);
        }

        private static void LackadaisicalTriangle_CheckOptimalWeight(int gridSize)
        {
            var selfLoopWeight = 6.0 / (gridSize*gridSize);
            LackadaisicalTriangle_StepByStep(gridSize, selfLoopWeight, 300);
        }


        /// <summary>
        /// Experiment from [Won2018]
        /// </summary>
        private static void LackadaisicalRectangle_StepByStep_DifferentWeights()
        {
            LackadaisicalRectangle_StepByStep(16, 0.0, 100);
            LackadaisicalRectangle_StepByStep(16, 0.1, 100);
            LackadaisicalRectangle_StepByStep(16, 0.2, 100);
            LackadaisicalRectangle_StepByStep(16, 0.015, 100);
            LackadaisicalRectangle_StepByStep(16, 0.005, 100);
        }

        private static void LackadaisicalRectangle_StepByStep(int gridSize, double selfLoopWeight, int? steps = null)
        {
            Console.WriteLine($"n = {gridSize}; l = {selfLoopWeight}");

            var qws = new Lackadaisical.QuantumWalkSimulatorRectangle(gridSize, gridSize, selfLoopWeight: selfLoopWeight);
            qws.MarkVertex(0, 0);

            Run(qws, OutputMode.StepByStep, maxSteps: steps);

            Console.WriteLine();
        }

        private static void LackadaisicalRectangle_FindOptimalWeight()
        {
            int gridSize = 100;
            Lackadaisical_FindOptimalWeight(gridSize, (n, l) => new Lackadaisical.QuantumWalkSimulatorRectangle(n, n, selfLoopWeight: l));
        }

        private static void LackadaisicalRectangle_CheckOptimalWeight()
        {
            LackadaisicalRectangle_CheckOptimalWeight(16);
            LackadaisicalRectangle_CheckOptimalWeight(32);
            LackadaisicalRectangle_CheckOptimalWeight(64);
        }


        private static void LackadaisicalRectangle_CheckOptimalWeight(int gridSize)
        {
            var selfLoopWeight = 4.0 / (gridSize * gridSize);
            LackadaisicalRectangle_StepByStep(gridSize, selfLoopWeight, 300);
        }


        private static void LackadaisicalHoneycomb_StepByStep_DifferentWeights()
        {
            LackadaisicalHoneycomb_StepByStep(16, 0.0, 100);
            LackadaisicalHoneycomb_StepByStep(16, 0.1, 100);
            LackadaisicalHoneycomb_StepByStep(16, 0.2, 100);
            LackadaisicalHoneycomb_StepByStep(16, 0.012, 100);
            LackadaisicalHoneycomb_StepByStep(16, 0.005, 100);
        }

        private static void LackadaisicalHoneycomb_FindOptimalWeight()
        {
            int gridSize = 100;
            Lackadaisical_FindOptimalWeight(gridSize, (n, l) => new Lackadaisical.QuantumWalkSimulatorHoneycomb(n, n, selfLoopWeight:l));
        }

        private static void LackadaisicalHoneycomb_StepByStep(int gridSize, double selfLoopWeight, int? steps = null)
        {
            Console.WriteLine($"n = {gridSize}; l = {selfLoopWeight}");

            var qws = new Lackadaisical.QuantumWalkSimulatorHoneycomb(gridSize, gridSize, selfLoopWeight: selfLoopWeight);
            qws.MarkVertex(0, 0);

            Run(qws, OutputMode.StepByStep, maxSteps: steps);

            Console.WriteLine();
        }

        private static void LackadaisicalHoneycomb_CheckOptimalWeight()
        {
            LackadaisicalHoneycomb_CheckOptimalWeight(16);
            LackadaisicalHoneycomb_CheckOptimalWeight(32);
            LackadaisicalHoneycomb_CheckOptimalWeight(64);
        }

        private static void LackadaisicalHoneycomb_CheckOptimalWeight(int gridSize)
        {
            var selfLoopWeight = 3.0 / (gridSize * gridSize);
            LackadaisicalHoneycomb_StepByStep(gridSize, selfLoopWeight, 300);
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
                Console.WriteLine($"{qws.N}; {qws.T}; {qws.GetMarkedVertexProbability()}; {qws.GetScalarProduct()}");
        }

        private static void Lackadaisical_FindOptimalWeight(int gridSize, Func<int,double,IQuantumWalkSimulator2D> CreateSimulator)
        {
            int N = gridSize * gridSize;

            Console.WriteLine($"l; T; Pr; Overlap");

            for (var a = 0.0m; a <= 10m; a += 0.1m)
            {
                double selfLoopWeight = (double)a / N;

                var qws = CreateSimulator(gridSize, selfLoopWeight);
                qws.MarkVertex(0, 0);

                Console.Write($"{selfLoopWeight}; ");

                var maxPr = qws.GetMarkedVertexProbability();
                var maxPrStep = 0;
                var maxPrOverlap = 0.0;

                while (true)
                {
                    qws.Run(1);

                    var prob = qws.GetMarkedVertexProbability();

                    if (prob > maxPr)
                    {
                        maxPr = prob;
                        maxPrStep = qws.T;
                        maxPrOverlap = qws.GetScalarProduct();
                    }

                    if (maxPr - prob > 0.05)
                        break;
                }

                Console.WriteLine($"{maxPrStep}; {maxPr}; {maxPrOverlap}");
            }
        }


        private static void LackadaisicalTriangle_PrintRuntime()
        {
            PrintRuntime(n => new Lackadaisical.QuantumWalkSimulatorTriangle(n, n, selfLoopWeight: 6.0 / (n * n)));
        }

        private static void LackadaisicalRectangle_PrintRuntime()
        {
            PrintRuntime(n => new Lackadaisical.QuantumWalkSimulatorRectangle(n, n, selfLoopWeight: 4.0 / (n * n)));
        }

        private static void LackadaisicalHoneycomb_PrintRuntime()
        {
            PrintRuntime(n => new Lackadaisical.QuantumWalkSimulatorHoneycomb(n, n, selfLoopWeight: 3.0 / (n * n)));
        }

        private static void PrintRuntime(Func<int, IQuantumWalkSimulator2D> CreateSimulator)
        {
            Console.WriteLine($"n; T; Pr; Overlap");

            for (var gridSize = 10; gridSize <= 200; gridSize += 2)
            {
                var qws = CreateSimulator(gridSize);
                qws.MarkVertex(0, 0);

                Console.Write($"{gridSize}; ");

                var maxPr = qws.GetMarkedVertexProbability();
                var maxPrStep = 0;
                var maxPrOverlap = 0.0;

                while (true)
                {
                    qws.Run(1);

                    var prob = qws.GetMarkedVertexProbability();

                    if (prob > maxPr)
                    {
                        maxPr = prob;
                        maxPrStep = qws.T;
                        maxPrOverlap = qws.GetScalarProduct();
                    }

                    if (maxPr - prob > 0.05)
                        break;
                }

                Console.WriteLine($"{maxPrStep}; {maxPr}; {maxPrOverlap}");
            }
        }
    }
}
