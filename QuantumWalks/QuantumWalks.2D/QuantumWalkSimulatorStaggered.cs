using System;
using System.Collections.Generic;

using CQCS.QuantumWalks.Common;

namespace CQCS.QuantumWalks.Grid2D
{
    /// <summary>
    /// Implements a staggered quantum walk on two-dimensional grid (according to [Fal13]).
    /// </summary>
	public class QuantumWalkSimulatorStaggered : IQuantumWalkSimulator
	{
        public readonly int Height;
        public readonly int Width;

        public int N { get { return Width * Height; } }

		private readonly double[,] state;

        /// <summary>
        /// Marked vertex list
        /// </summary>
        private readonly List<Vertex> markedVertices = new List<Vertex>();

        /// <summary>
        /// Tesselation region size.
        /// </summary>
        private const int d = 2;

		////////////////////////////////////////////////////////////////////////

		public QuantumWalkSimulatorStaggered (int height, int width)
		{
            if (height % d != 0) throw new ArgumentException("height");
            if (width % d != 0) throw new ArgumentException("width");

            Height = height;
            Width = width;
	
			state = new double [Width, Height];

            // Set initial state
			int stateCount = Height*Width;

            double initialAmplitude = 1.0 / Math.Sqrt (stateCount);

            for (int y = 0; y < Height; y++)	
			for (int x = 0; x < Width; x++)
			{
				state [x, y] = initialAmplitude;
			}

			T = 0;
		}


		////////////////////////////////////////////////////////////////////////

		public void MarkVertex (int x, int y)
		{
			if ((x < 0) || (Width <= x)) throw new ArgumentOutOfRangeException ("x");
			if ((y < 0) || (Height <= y)) throw new ArgumentOutOfRangeException ("y");

            if (!IsVertexMarked(x, y))
            {
                var v = new Vertex(x, y);
                markedVertices.Add(v);
            }
		}

        public bool IsVertexMarked(int x, int y)
        {
            var v = new Vertex(x, y);
            return markedVertices.Contains(v);
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
                Diffusion(d, 0, 0);
                Query();
                Diffusion(d, d / 2, d / 2);
            }
        }


		////////////////////////////////////////////////////////////////////////
        // Transformations

        #region Query transformation

		private void Query ()
        {
		    foreach (var p in markedVertices)
		    {
                state[p.X, p.Y] = - state[p.X, p.Y];
		    }
        }

        #endregion

		#region Coin flip transformation

        /// <summary>
        /// Emulates toroidal space in horisontal direction.
        /// </summary>
        private int TX(int x)
        {
            if (x >= Width) x = x - Width;
            //if (x < 0) x = Width + x;

            return x;
        }

        /// <summary>
        /// Emulates toroidal space in vertical direction.
        /// </summary>
        private int TY(int y)
        {
            if (y >= Height) y = y - Height;
            //if (y < 0) y = Heigth + y;

            return y;
        }


        /// <summary>
        /// Performs Grover's diffusion transformation on specified region.
		/// </summary>
        private void Diffusion(int left, int bottom, int width, int height)
        {
            int top = bottom + height - 1;
            int right = left + width - 1;

            // Calculate the average
            double average = 0;

            for (int y = bottom; y <= top; y++)
            for (int x = left; x <= right; x++)
            {
                average += state[TX(x), TY(y)];
            }

            average /= (width*height);

            // Invert all amplitudes about average
            for (int y = bottom; y <= top; y++)
            for (int x = left; x <= right; x++)
            {
                state[TX(x), TY(y)] = 2*average - state[TX(x), TY(y)];
            }
        }

        /// <summary>
        /// Performs Grover's diffusion transformation on all DxD regions starting from (x_start, y_start).
		/// </summary>
        private void Diffusion(int regionSize, int x_start, int y_start)
        {
            for (int dy = 0; dy < Height; dy += regionSize)
            for (int dx = 0; dx < Width; dx += regionSize)
            {
                Diffusion(x_start + dx, y_start + dy, regionSize, regionSize);
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

			for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
			{
				scalarProduct += state [x, y];
			}

			int stateCount = Height*Width;

			return scalarProduct / Math.Sqrt (stateCount);
		}


        /// <summary>
        /// Returns an amplitude of the vertex.
        /// </summary>
        public double GetVertexAmplitude(int x, int y)
        {
            return state[x, y];
        }


        /// <summary>
        /// Returns a probability to get the vertex as a result of the measurement.
        /// </summary>
        public double GetVertexProbability(int x, int y)
        {
            return Math.Pow(state[x, y], 2);
        }


        /// <summary>
        /// Returns a total probability of all marked vertices.
        /// </summary>
        public double GetMarkedVertexProbability()
        {
            double probability = 0;

            foreach (Vertex v in markedVertices)
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
