using System;
using System.Linq;
using System.Collections.Generic;

using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Graph
{
    /// <summary>
    /// Implements a quantum walk process on a graph (alternative implementations).
    /// </summary>
	public class QuantumWalkSimulatorEx : IQuantumWalkSimulator
	{
        public int N { get; private set; }

        /// <summary>
        /// Defines a current state of the quantum walk.
        /// state[i][j] = directional amplitude of i-th vertex pointing towards j-th vertex.
        /// </summary>
        private readonly Dictionary<int,double>[] state;

		/// <summary>
		/// Specifies if a graph point is "marked".
		/// </summary>
		private readonly bool[] isMarked;

        /// <summary>
        /// Indicates whether the 
        /// </summary>
        private static bool initialized;

        ////////////////////////////////////////////////////////////////////////

        public QuantumWalkSimulatorEx(int numberOfVertices)
        {
            N = numberOfVertices;

            state = new Dictionary<int, double>[N];

            for (int i = 0; i < state.Length; i++)
                state[i] = new Dictionary<int, double>();

            isMarked = new bool[N];

            initialized = false;
        }

        /// <summary>
        /// Sets the initial state
        /// </summary>
        public void Initialize()
        {
            int stateCount = state.Sum(s => s.Count);

            if (stateCount == 0)
                throw new ArgumentException("The graph has no edges");
			
            double initialAmplitude = 1 / Math.Sqrt (stateCount);

            for (int i = 0; i < N; i++)
                foreach (int j in state[i].Keys.ToList())
                {
                    state[i][j] = initialAmplitude;
                }

			T = 0;

            initialized = true;
		}

        ////////////////////////////////////////////////////////////////////////

        public void AddEdge (int i, int j)
        {
            if (HasEdge(i, j)) return;

            initialized = false;

            state[i].Add(j, 0.0);    
            state[j].Add(i, 0.0);    
        }

        public void RemoveEdge(int i, int j)
        {
            if (!HasEdge(i, j)) return;

            initialized = false;

            state[i].Remove(j);
            state[j].Remove(i);
        }

        public bool HasEdge(int i, int j)
        {
            if (i >= N) throw new ArgumentOutOfRangeException(nameof(i));
            if (j >= N) throw new ArgumentOutOfRangeException(nameof(j));

            return state[i].ContainsKey(j);
        }

        ////////////////////////////////////////////////////////////////////////

        public void MarkVertex (int i)
		{
            if (i >= N) throw new ArgumentOutOfRangeException(nameof(i));

            isMarked[i] = true;
		}

        public void UnmarkVertex(int i)
        {
            if (i >= N) throw new ArgumentOutOfRangeException(nameof(i));

            isMarked[i] = false;
        }

        public bool IsVertexMarked(int i)
        {
            if (i >= N) throw new ArgumentOutOfRangeException(nameof(i));

            return isMarked[i];
        }

        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets current step.
        /// </summary>
        public int T { get; private set; }


		/// <summary>
		/// Performs the specified number of quantum walk steps.
		/// </summary>
		public void Run (int stepCount)
		{
            if (!initialized)
                Initialize();

			for (int i = 0; i < stepCount; i++, T++)
			{
                Query ();
				CoinFlip ();
				Shift ();
			}
		}


		////////////////////////////////////////////////////////////////////////
        // Transformations

        #region Query transformation

        private void Query ()
        {
            for (int i = 0; i < N; i++)
            {
                if (isMarked[i])
                {
                    foreach (int j in state[i].Keys.ToList())
                        state[i][j] = -state[i][j];
                }
            }
        }

        #endregion

		#region Coin flip transformation

        /// <summary>
		/// Performs coin flip transformation on the current state.
		/// </summary>
		private void CoinFlip ()
		{
            for (int i = 0; i < N; i++)
            {
                if (state[i].Count == 0)
                    continue;

                // Calculate the average
                double avg = state[i].Average(kv => kv.Value);

                // Do reflection above the average
                foreach (int j in state[i].Keys.ToList())
                    state[i][j] = 2*avg - state[i][j];
            }
		}

		#endregion

		#region Shift transformation

		/// <summary>
		/// Performs a shift transform on the current state.
		/// </summary>
		private void Shift ()
		{
			for (int i = 0; i < N; i++)
                foreach (int j in state[i].Keys.ToList())
                {
                    if (i < j)
                    {
                        // Exchange (i,j) and (j,i) amplitudes
                        double tmp = state[i][j];
                        state[i][j] = state[j][i];
                        state[j][i] = tmp;
                    }
                }
		}

		#endregion

		////////////////////////////////////////////////////////////////////////
	
		/// <summary>
		/// Returns a scalar product of the current and the initial state.
		/// </summary>
		public double GetScalarProduct ()
		{
			double scalarProduct = 0;

            for (int i = 0; i < N; i++)
                foreach (int j in state[i].Keys)
                {
                    scalarProduct += state[i][j];
                }
            
			int stateCount = state.Sum(s => s.Count);

			return scalarProduct / Math.Sqrt (stateCount);
		}
        

		/// <summary>
		/// Returns a probability to get the vertex as a result of the measurement.
		/// </summary>
		public double GetVertexProbability (int i)
		{
            double probability = 0;

            foreach (int j in state[i].Keys)
            {
                probability += (state[i][j] * state[i][j]);
            }

            return probability;
        }

        /// <summary>
		/// Returns the total probability of finding a marked vertex as a result of the measurement.
		/// </summary>
		public double GetMarkedVertexProbability()
        {
            double probability = 0;

            for (int i = 0; i < N; i++)
            {
                if (isMarked[i])
                {
                    probability += GetVertexProbability(i);
                }
            }

            return probability;
        }

        /// <summary>
		/// Returns the total probability (for testing purposes).
		/// </summary>
		public double GetTotalProbability()
        {
            double probability = 0;

            for (int i = 0; i < N; i++)
            {
                probability += GetVertexProbability(i);
            }

            return probability;
        }
    }
}
