using System;
using System.Linq;
using System.Collections.Generic;

namespace CQCS.QuantumWalks.Grid2D.Lackadaisical
{
    /// <summary>
    /// Implements quantum walk on triangular two-dimensional grid.
    /// </summary>
	public class QuantumWalkSimulatorTriangle : IQuantumWalkSimulator2D
    { 
        // We use equivalence of triangular 2D grid and rectangular 2D grid
        // with six edges going from each vertex, i.e.
        // 
        //    *       * 
        //   / \   =  | \ 
        //  * - *     * - *
        // 
        //  that is the grid looks like
        //
        //  * - * - * - * 
        //  | \ | \ | \ | 
        //  * - * - * - * 
        //  | \ | \ | \ | 
        //  * - * - * - * 
        //
        // The shift transformation works as follows
        //
        // S|x,y,UpRight> => |x,y+1,DownLeft>
        // S|x,y,DownLeft> => |x,y-1,UpRight> 
        // S|x,y,Right> => |x+1,y,Left>
        // S|x,y,Left> => |x+1,y,Left>
        // S|x,y,DownRight> => |x+1,y-1,UpLeft>
        // S|x,y,UpLeft> => |x-1,y+1,DownRight>
        //
        // Coin transformation is Grover's diffusion D_6

		private const int Left = 0;
		private const int Right = 1;
        private const int DownLeft = 2;
        private const int UpRight = 3;
        private const int DownRight = 4;
        private const int UpLeft = 5;
        private const int Self = 6;

        private const int DirectionCount = 6;

        public readonly int Height;
        public readonly int Width;

        public int N {  get { return Width * Height; } }

        public readonly double SelfLoopWeight;

        public readonly Coin Coin;

		private readonly double[,,] state;

		////////////////////////////////////////////////////////////////////////

		public QuantumWalkSimulatorTriangle(int height, int width, Coin coin = Coin.AKR, double selfLoopWeight = 0)
        {
            Height = height;
            Width = width;
            SelfLoopWeight = selfLoopWeight;

            state = new double [Width, Height, DirectionCount + 1];

            Coin = coin;

            // Set initial state
            var denominator = (DirectionCount + SelfLoopWeight) * (Height * Width);
            double initialAmplitude = 1.0 / Math.Sqrt(denominator);

            for (int y = 0; y < Height; y++)	
			for (int x = 0; x < Width; x++)
			{
				state [x, y, Left] = initialAmplitude;
				state [x, y, Right] = initialAmplitude;
				state [x, y, UpLeft] = initialAmplitude;
				state [x, y, UpRight] = initialAmplitude;
				state [x, y, DownLeft] = initialAmplitude;
				state [x, y, DownRight] = initialAmplitude;
                state[x, y, Self] = initialAmplitude * Math.Sqrt(SelfLoopWeight);
            }

            T = 0;
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

        public bool IsVertexMarked(int x, int y)
        {
            var v = new Vertex(x, y);
            return IsVertexMarked(v);
        }

        public bool IsVertexMarked(Vertex v)
        {
            return markedVertexDict.ContainsKey(v);
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
                state[p.X, p.Y, Left] = -state[p.X, p.Y, Left];
                state[p.X, p.Y, Right] = -state[p.X, p.Y, Right];
                state[p.X, p.Y, UpLeft] = -state[p.X, p.Y, UpLeft];
                state[p.X, p.Y, UpRight] = -state[p.X, p.Y, UpRight];
                state[p.X, p.Y, DownLeft] = -state[p.X, p.Y, DownLeft];
                state[p.X, p.Y, DownRight] = -state[p.X, p.Y, DownRight];
                state[p.X, p.Y, Self] = -state[p.X, p.Y, Self];
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
            // C = 2|s_c><s_c| - I
            // |s_c> = 1/sqrt(6+l) (|left> + |right> + |upleft> + |upright> + |downleft> + |downright> + sqrt(l)|self>)

            double sum = state[x, y, Left] + state[x, y, Right] +
                         state[x, y, UpLeft] + state[x, y, UpRight] +
                         state[x, y, DownLeft] + state[x, y, DownRight] +
                         Math.Sqrt(SelfLoopWeight) * state[x, y, Self];

            double scaledSum = 2 * sum / (DirectionCount + SelfLoopWeight);

            state[x, y, Left]  = scaledSum - state[x, y, Left];
            state[x, y, Right] = scaledSum - state[x, y, Right];
            state[x, y, UpLeft]  = scaledSum - state[x, y, UpLeft];
            state[x, y, UpRight] = scaledSum - state[x, y, UpRight];
            state[x, y, DownLeft] = scaledSum - state[x, y, DownLeft];
            state[x, y, DownRight] = scaledSum - state[x, y, DownRight];
            state[x, y, Self] = scaledSum * Math.Sqrt(SelfLoopWeight) - state[x, y, Self];
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
            //    *       * 
            //   / \   =  | \ 
            //  * - *     * - *

            for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
			{
                // |x, y, Right> <=> |x+1, y, Left>
                Exchange(ref state[x, y, Right], ref state[Torus.X(x + 1, Width), y, Left]);

                // |x,y,UpRight> <=> |x,y+1,DownLeft>
                Exchange(ref state[x, y, UpRight], ref state[x, Torus.Y(y + 1, Height), DownLeft]);

                // |x+1,y-1,UpLeft> <=> S|x,y,DownRight>
                Exchange(ref state[x, y, DownRight], ref state[Torus.X(x + 1, Width), Torus.Y(y - 1, Height), UpLeft]);
			}
		}

		#endregion

		////////////////////////////////////////////////////////////////////////
	
		/// <summary>
		/// Returns a scalar product of the current and the initial state.
		/// </summary>
		public double GetScalarProduct()
		{
			double amplitudeSum = 0;

			for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
			{
				amplitudeSum += state[x, y, Left] + state[x, y, Right] + 
                                state[x, y, UpLeft] + state[x, y, UpRight] + 
                                state[x, y, DownLeft] + state[x, y, DownRight] +
                                Math.Sqrt(SelfLoopWeight) * state[x, y, Self];
            }

            var denominator = (DirectionCount + SelfLoopWeight)*(Height*Width);
            double initialAmplitude = 1.0 / Math.Sqrt(denominator);

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
        public double GetVertexProbability(int x, int y)
		{
            return GetVertexProbability(x, y, Left) +
                   GetVertexProbability(x, y, Right) +
                   GetVertexProbability(x, y, UpLeft) +
                   GetVertexProbability(x, y, UpRight) +
                   GetVertexProbability(x, y, DownLeft) +
                   GetVertexProbability(x, y, DownRight) +
                   GetVertexProbability(x, y, Self);
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
