using System;

namespace CQCS.QuantumWalks.NandTree
{
    public class QuantumWalkSimulator
    {
        /// <summary>
        /// Number of levels in the tree not including the tail.
        /// </summary>
        public int TreeDepth { get; private set; }

        /// <summary>
        /// Total number of nodes in the tree (including the tail).
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// Current step of the walk.
        /// </summary>
        public int T { get; private set; }

        private double[][] treeAmplitudes;
        private double tailAmplitude;

        private bool[] markedLeaves;

        /// <summary>
        /// Creates a NAND tree of the given depth. 
        /// NOTE: Root of the tree has depth 0, leaves have depth <paramref name="treeDepth"/>.
        /// </summary>
        public QuantumWalkSimulator(int treeDepth)
        {
            TreeDepth = treeDepth;

            // Create the tree
            treeAmplitudes = new double[treeDepth + 1][];

            tailAmplitude = 0;
            N = 1;
            
            for (int d = 0; d <= treeDepth; d++)
            {
                int numberOfVertices = (int)Math.Pow(2, d);
                N += numberOfVertices;

                treeAmplitudes[d] = new double[numberOfVertices];
            }

            int numberOfLeaves = (int)Math.Pow(2, treeDepth + 1);
            markedLeaves = new bool[numberOfLeaves];

            // Set the initial state
            tailAmplitude = 1;
            T = 0;
        }

        /// <summary>
        /// Marks the leaf of the tree. The tree of depth d has 2^<see cref="TreeDepth"/> leaves.
        /// </summary>
        public void MarkLeaf(int i)
        {
            if ((i < 0) || (markedLeaves.Length <= i))
                throw new ArgumentException();

            markedLeaves[i] = true;
        }

        /// <summary>
        /// Returns overlap between the current and the initial state.
        /// </summary>
        public double GetOverlapWithInitialState()
        {
            return tailAmplitude;
        }

        /// <summary>
        /// Returns the total probability to find a tail node as a result of the measurement.
        /// </summary>
        public double GetTailProbability()
        {
            return tailAmplitude* tailAmplitude;
        }

        /// <summary>
        /// Returns the total probability to find one of tree nodes as a result of the measurement.
        /// </summary>
        public double GetTreeProbability()
        {
            double probability = 0;

            for (int i = 0; i < treeAmplitudes.Length; i++)
            for (int j = 0; j < treeAmplitudes[i].Length; j++)
                 probability += treeAmplitudes[i][j]*treeAmplitudes[i][j];

            return probability;
        }


        /// <summary>
        /// Performs a step of the walk.
        /// </summary>
        public void Step()
        {
            Query();
            OddLevelTasselation();
            EvenLevelTasselation();

            T++;
        }


        private void Query()
        {
            double[] leaves = treeAmplitudes[TreeDepth];

            for (int i = 0; i < markedLeaves.Length; i++)
                if (markedLeaves[i])
                    leaves[i] = -leaves[i];
        }

        private void OddLevelTasselation()
        {
            TasselateTail();

            for (int depth = 1; depth < TreeDepth; depth += 2)
            {
                int numberOfNodes = treeAmplitudes[depth].Length;

                for (int node = 0; node < numberOfNodes; node++)
                    Tasselate(depth, node);
            }
        }

        private void EvenLevelTasselation()
        {
            for (int depth = 0; depth < TreeDepth; depth += 2)
            {
                int numberOfNodes = treeAmplitudes[depth].Length;

                for (int node = 0; node < numberOfNodes; node++)
                    Tasselate(depth, node);
            }
        }

        private void TasselateTail()
        {
            // a  b
            // b -a
            // where
            //   a = 2/sqrt(N) - 1
            //   b = 2*sqrt(1/sqrt(N) - 1/N)

            int n = treeAmplitudes[TreeDepth].Length;   // Number of leaves

            double a = 2/Math.Sqrt(n) - 1;
            double b = 2*Math.Sqrt(1.0/Math.Sqrt(n) - 1.0/n);

            double new_tail = a*tailAmplitude + b*treeAmplitudes[0][0]; 
            double new_root = b*tailAmplitude - a*treeAmplitudes[0][0];

            tailAmplitude = new_tail;
            treeAmplitudes[0][0] = new_root;
        }

        private void Tasselate(int depth, int node)
        {
            // -1/3  2/3  2/3
            //  2/3 -1/3  2/3
            //  2/3  2/3 -1/3

            double avg = (treeAmplitudes[depth][node] + treeAmplitudes[depth + 1][2*node] + treeAmplitudes[depth + 1][2*node + 1]) / 3.0;

            treeAmplitudes[depth][node] = 2*avg - treeAmplitudes[depth][node];
            treeAmplitudes[depth + 1][2*node] = 2*avg - treeAmplitudes[depth + 1][2*node];
            treeAmplitudes[depth + 1][2*node + 1] = 2*avg - treeAmplitudes[depth + 1][2*node + 1];
        }
    }
}
