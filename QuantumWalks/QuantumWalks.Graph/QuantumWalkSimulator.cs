using System;

using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Graph
{
    /// <summary>
    /// Implements a quantum walk process on a graph.
    /// </summary>
	public class QuantumWalkSimulator : IQuantumWalkSimulator
	{
        public int N { get; private set; }

        /// <summary>
        /// Graph to perform the walk on.
        /// </summary>
        private readonly Graph g;

        ////////////////////////////////////////////////////////////////////////

        public QuantumWalkSimulator(Graph g)
        {
            this.g = g;
            this.N = g.Vertices.Count;
            
            // Set initial amplitudes
            int stateCount = 2*g.Edges.Count;
			
            double initialAmplitude = 1 / Math.Sqrt (stateCount);

            foreach (Vertex v in g.Vertices)
                foreach (Edge e in v.Edges)
			    {
                    e.SetAmplitude (v, initialAmplitude);
			    }

			T = 0;
		}


		////////////////////////////////////////////////////////////////////////

		public void MarkVertex (int i)
		{
            g.Vertices[i].IsMarked = true;
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
            foreach (Vertex v in g.Vertices)
                if (v.IsMarked)
                {
                    foreach (Edge e in v.Edges)
                        e.SetAmplitude (v, -e.GetAmplitude(v));  // Flip the amplitude
                }
        }

        #endregion

		#region Coin flip transformation

        /// <summary>
		/// Performs coin flip transformation on the current state.
		/// </summary>
		private void CoinFlip ()
		{
            foreach (Vertex v in g.Vertices)
            { 
                // Calculate an average
                double avg = 0;

                foreach (Edge e in v.Edges)
                    avg += e.GetAmplitude(v);

                avg /= v.Edges.Count;

                // Perform the reflection above the average
                foreach (Edge e in v.Edges)
                    e.SetAmplitude (v, 2*avg - e.GetAmplitude(v));
            }
		}

		#endregion

		#region Shift transformation

		/// <summary>
		/// Performs a shift transform on the current state.
		/// </summary>
        private void Shift()
        {
            foreach (Vertex v in g.Vertices)
                foreach (Edge e in v.Edges)
                    if (v.Index < e.GetOtherVertex(v).Index)    // Do it only once
                    {
                        // Exchange the amplitudes
                        double tmp = e.A2;
                        e.A2 = e.A1;
                        e.A1 = tmp;
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

            foreach (Vertex v in g.Vertices)
                foreach (Edge e in v.Edges)
			    {
				    scalarProduct += e.GetAmplitude(v);
			    }

			int stateCount = 2*g.Edges.Count;

			return scalarProduct / Math.Sqrt (stateCount);
		}
        

		/// <summary>
		/// Returns a probability to get the vertex as a result of the measurement.
		/// </summary>
		public double GetVertexProbability (int i)
		{
            double probability = 0;

            Vertex v = g.Vertices[i];

            foreach (Edge e in v.Edges)
            {
                double a = e.GetAmplitude(v);
                probability += (a*a);
            }

            return probability;
        }

        /// <summary>
        /// Returns the total probability of finding a marked vertex as a result of the measurement.
        /// </summary>
        public double GetMarkedVertexProbability()
        {
            double probability = 0;

            foreach (Vertex v in g.Vertices)
            {
                if (v.IsMarked)
                {
                    probability += GetVertexProbability(v.Index);
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

            foreach (Vertex v in g.Vertices)
            {
                probability += GetVertexProbability(v.Index);
            }

            return probability;
        }
    }
}
