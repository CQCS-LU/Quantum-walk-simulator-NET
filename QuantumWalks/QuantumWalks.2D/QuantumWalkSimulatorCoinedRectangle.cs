using System;
using System.Linq;
using System.Collections.Generic;

namespace CQCS.QuantumWalks.Grid2D
{
    /// <summary>
    /// Implements quantum walk on two-dimensional rectangle grid.
    /// </summary>
	public class QuantumWalkSimulatorCoinedRectangle : IQuantumWalkSimulator2D
    {
        private const int Left  = 0;
        private const int Right = 1;
        private const int Up    = 2;
        private const int Down  = 3;

        private const int DirectionCount = 4;

        public readonly int Height;
        public readonly int Width;

        public int N {  get { return Width * Height; } }

        public readonly Coin Coin;

		private readonly double[,,] state;

		////////////////////////////////////////////////////////////////////////

		public QuantumWalkSimulatorCoinedRectangle (int height, int width, Coin coin = Coin.AKR)
		{
            Height = height;
            Width = width;
	
			state = new double [Width, Height, DirectionCount];

            Coin = coin;

            // Set initial state
            double initialAmplitude = GetInitialAmplitude();

            for (int y = 0; y < Height; y++)	
			for (int x = 0; x < Width; x++)
			{
				state [x, y, Left ] = initialAmplitude;
				state [x, y, Right] = initialAmplitude;
				state [x, y, Up   ] = initialAmplitude;
				state [x, y, Down ] = initialAmplitude;
			}

			T = 0;
		}

        private double GetInitialAmplitude()
        {
            int stateCount = DirectionCount * Height * Width;
            return 1.0 / Math.Sqrt(stateCount);
        }

        ////////////////////////////////////////////////////////////////////////

        private readonly Dictionary<Vertex, Vertex> markedVertexDict = new Dictionary<Vertex, Vertex>();

        /// <summary>
        /// Gets a list of marked vertices.
        /// </summary>
        public List<Vertex> MarkedVertices
        {
            get { return markedVertexDict.Values.ToList(); }
        }

        public void MarkVertex(Vertex v)
        {
            if ((v.X < 0) || (Width <= v.X)) throw new ArgumentOutOfRangeException ("x");
			if ((v.Y < 0) || (Height <= v.Y)) throw new ArgumentOutOfRangeException ("y");

            if (!IsVertexMarked(v))
                markedVertexDict.Add(v, v);
        }

		public void MarkVertex(int x, int y)
		{
			MarkVertex(new Vertex(x, y));
		}

        public void UnMarkVertex(Vertex v)
        {
            if ((v.X < 0) || (Width <= v.X)) throw new ArgumentOutOfRangeException ("x");
			if ((v.Y < 0) || (Height <= v.Y)) throw new ArgumentOutOfRangeException ("y");

            if (IsVertexMarked(v))
                markedVertexDict.Remove(v);
        }

        public void UnMarkVertex(int x, int y)
		{
			UnMarkVertex(new Vertex(x, y)); 
		}

        public bool IsVertexMarked(Vertex v)
        {
            return markedVertexDict.ContainsKey(v);
        }

        public bool IsVertexMarked(int x, int y)
        {
            return IsVertexMarked(new Vertex(x, y));
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
            foreach (var p in markedVertexDict.Values)
            {
                // Change sign of amplitudes
                state[p.X, p.Y, Left]  = -state[p.X, p.Y, Left];
                state[p.X, p.Y, Right] = -state[p.X, p.Y, Right];
                state[p.X, p.Y, Up]    = -state[p.X, p.Y, Up];
                state[p.X, p.Y, Down]  = -state[p.X, p.Y, Down];
            }
        }

        #endregion

        #region Coin flip transformation

        /// <summary>
        /// Performs coin flip transformation on the current register state.
        /// </summary>
        private void CoinFlip()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                if (Coin == Coin.AKR) // D for unmarked, I for marked
                {
                    if (IsVertexMarked(x, y))
                    {
                        IdentityCoin(x, y);
                    }
                    else
                    {
                        GroverCoin(x, y);
                    }
                }
                else if (Coin == Coin.Grover)
                {
                    GroverCoin(x, y);
                }
            }
        }

        private void IdentityCoin(int x, int y)
        {
        }

        private void GroverCoin(int x, int y)
        {
            double sum = state[x, y, Left] + state[x, y, Right] + 
                         state[x, y, Up] + state[x, y, Down];

            double avg = sum / DirectionCount;

            state[x, y, Left]  = 2*avg - state[x, y, Left];
            state[x, y, Right] = 2*avg - state[x, y, Right];
            state[x, y, Up]    = 2*avg - state[x, y, Up];
            state[x, y, Down]  = 2*avg - state[x, y, Down];
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
		private void Shift ()
		{
			for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
			{
                // |x, y, Right> <=> |x+1, y, Left>
                Exchange (ref state[x, y, Right], ref state[Torus.X(x + 1, Width), y, Left]);

                // |x, y, Up> <=> |x, y+1, Down>
                Exchange (ref state[x, y, Up   ], ref state[x, Torus.Y(y + 1, Height), Down]);
			}
		}

		#endregion

		////////////////////////////////////////////////////////////////////////
	
		/// <summary>
		/// Returns a scalar product of the current and the initial state.
		/// </summary>
		public double GetScalarProduct ()
		{
			double amplitudeSum = 0;

			for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
			{
				amplitudeSum += (state[x, y, Left] + state[x, y, Right] + state[x, y, Up] + state[x, y, Down]);
			}

            double initialAmplitude = GetInitialAmplitude();

			return amplitudeSum * initialAmplitude;
		}


        /// <summary>
		/// Returns a probability to find the vertex as a result of the measurement
        /// for the specific direction.
		/// </summary>
		private double GetVertexProbability(int x, int y, int direction)
        {
            return Math.Pow(state[x, y, direction], 2);
        }


        /// <summary>
        /// Returns a probability to get the vertex as a result of the measurement.
        /// </summary>
        public double GetVertexProbability (int x, int y)
		{
            return GetVertexProbability(x, y, Left) +
                   GetVertexProbability(x, y, Right) +
                   GetVertexProbability(x, y, Up) +
                   GetVertexProbability(x, y, Down);
        }


        /// <summary>
        /// Returns a total probability of all marked vertices.
        /// </summary>
        public double GetMarkedVertexProbability()
        {
            double probability = 0;

            foreach (Vertex v in markedVertexDict.Values)
                probability += GetVertexProbability(v.X, v.Y);

            return probability;
        }
        
        /// <summary>
        /// Returns the total probability (for testing purposes).
        /// </summary>
        public double GetTotalProbability()
        {
            double probability = 0;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    probability += GetVertexProbability(x, y);
                }

            return probability;
        }
    }
}
