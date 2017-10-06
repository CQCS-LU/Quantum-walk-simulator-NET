using System;
using System.Collections.Generic;

using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Hybercube
{
    /// <summary>
    /// Implements quantum walk on hypercube.
    /// </summary>
    public class QuantumWalkSimulator : IQuantumWalkSimulator
    {
        public readonly int n;
        public int N { get; private set; }

        private double[,] state;

        private readonly List<int> markedVerteces = new List<int>();

        /// <summary>
        /// Gets marked points.
        /// </summary>
        public List<int> MarkedVertices
        {
            get { return markedVerteces; }
        }

        ////////////////////////////////////////////////////////////////////////

        public QuantumWalkSimulator(int n)
        {
            this.n = n;
            N = (int) Math.Pow(2, n);

            state = new double[N, n];

            // Set initial state
            int stateCount = N * n;

            double initialAmplitude = 1.0 / Math.Sqrt (stateCount);

            for (int i = 0; i < N; i++)
                for (int j = 0; j < n; j++)
                {
                    state[i, j] = initialAmplitude;
                }

            T = 0;
        }


        ////////////////////////////////////////////////////////////////////////

        public void MarkVertex(int i)
        {
            if ((i < 0) || (N <= i)) throw new ArgumentOutOfRangeException("i");

            if (!IsVertexMarked(i))
                markedVerteces.Add(i);
        }

        public void UnMarkVertex(int i)
        {
            if ((i < 0) || (N <= i)) throw new ArgumentOutOfRangeException("i");

            if (IsVertexMarked(i))
                markedVerteces.Remove(i);
        }

        public bool IsVertexMarked(int i)
        {
            if ((i < 0) || (N <= i)) throw new ArgumentOutOfRangeException("i");

            return markedVerteces.Contains(i);
        }

        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets current step.
        /// </summary>
        public int T { get; private set; }


        /// <summary>
        /// Runs the walk for a given number of steps.
        /// </summary>
        public void Run(int stepCount = 1)
        {
            for (int i = 0; i < stepCount; i++, T++)
            {
                Query();
                CoinFlip();
                Shift();
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Transformations

        #region Query transformation

        private void Query()
        {
            foreach (var i in markedVerteces)
                for (int j = 0; j < n; j++)
                {
                    state[i, j] = -state[i, j];
                }
        }

        #endregion

        #region Coin flip transformation

        /// <summary>
        /// Performs coin flip transformation on the current register state.
        /// </summary>
        private void CoinFlip()
        {
            for (int i = 0; i < N; i++)
            {
                double average = 0;

                for (int j = 0; j < n; j++)
                {
                    average += state[i, j];
                }

                average = average / n;

                // Apply Grover's diffusion
                for (int j = 0; j < n; j++)
                {
                    state[i, j] = 2*average - state[i, j];
                }
            }
        }

        #endregion

        #region Shift transformation

        /// <summary>
        /// Exchanges two amplitudes.
        /// </summary>
        private static void Exchange(ref double a1, ref double a2)
        {
            double tmp = a1;
            a1 = a2;
            a2 = tmp;
        }

        /// <summary>
        /// Performs a flip-flop shift transform on the current register state.
        /// </summary>
        private void Shift()
        {
            var newState = new double[N, n];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < n; j++)
                {
                    int k = (0x01 << j) ^ i;
                    newState[i, j] = state[k, j];
                    newState[k, j] = state[i, j];
                }

            state = newState;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns a scalar product of the current and the initial state.
        /// </summary>
        public double GetScalarProduct()
        {
            double scalarProduct = 0;

            for (int i = 0; i < N; i++)
                for (int j = 0; j < n; j++)
                {
                    scalarProduct += state[i, j];
                }

            int stateCount = N * n;

            return scalarProduct / Math.Sqrt(stateCount);
        }


        /// <summary>
        /// Returns the amplitude of the vertex.
        /// </summary>
        public double GetVertexAmplitude(int i, int j)
        {
            return state[i, j];
        }


        /// <summary>
        /// Returns a probability to get the vertex as a result of the measurement.
        /// </summary>
        public double GetVertexProbability(int i)
        {
            double probability = 0;

            for (int j = 0; j < n; j++)
                probability += Math.Pow(state[i, j], 2);

            return probability;
        }


        /// <summary>
        /// Returns a total probability of all marked vertices.
        /// </summary>
        public double GetMarkedVertexProbability()
        {
            double probability = 0;

            foreach (int i in markedVerteces)
                probability += GetVertexProbability(i);

            return probability;
        }

        /// <summary>
        /// Returns a total probability of all vertices.
        /// </summary>
        public double GetTotalProbability()
        {
            double probability = 0;

            for (int i = 0; i < N; i++)
                probability += GetVertexProbability(i);

            return probability;
        }
    }
}

