using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

/////////////////////////////////////////////////////////////////////////////

namespace CQCS.QuantumWalks.Grid2D.WinForm
{
	public class QuantumWalkGrid : UserControl
	{
		#region private void InitializeComponent() ...

		private void InitializeComponent()
		{
			// 
			// BooleanGrid
			// 
			this.Name = "BooleanGrid";

		}

		#endregion
		
		///////////////////////////////////////////////////////////////////////

		public QuantumWalkGrid()
		{
			InitializeComponent();

			InitializeGrid();
		}

		
		/// <summary>
		/// Release the unmanaged resources used by control
		/// </summary>
		public new void Dispose()
		{
			DisposeGridResources();
	
			base.Dispose();
		}

		
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Specifies the grid width in cells.
		/// </summary>
		[Browsable (true)]
		public int WidthInPoints
		{
			get { return widthInPoints; }
			set
			{
				widthInPoints = value;
				InitializeGrid();
			}
		}

		private int widthInPoints = 32;
		

		/// <summary>
		/// Specifies the grid width in points.
		/// </summary>
		[Browsable (true)]
		public int HeightInPoints
		{
			get { return heightInPoints; }
			set
			{
				heightInPoints = value;
				InitializeGrid();
			}
		}

		private int heightInPoints = 32;

		
		/// <summary>
		/// Specifies the grid cell size.
		/// </summary>
		[Browsable (true)]
		public int PointSize
		{
			get { return pointSize; }
			set
			{
				pointSize = value;
				InitializeGrid();
			}
		}

		private int pointSize = 2;

		///////////////////////////////////////////////////////////////////////
		
		// Area map bitmap and drawing surface
		private Bitmap    gridBitmap;	
		private Graphics  gridGraphics;
		private Rectangle gridRect;


		/// <summary>
		/// ...
		/// </summary>
		private void InitializeGrid()
		{
			DisposeGridResources();

			// Set control size
			this.Width  = widthInPoints*pointSize;
			this.Height = heightInPoints*pointSize;

			// Create a bitmap and a drawing surface 
			gridRect = new Rectangle (0, 0, this.Width, this.Height);
			gridBitmap = new Bitmap (gridRect.Width, gridRect.Height);
			gridGraphics = Graphics.FromImage (gridBitmap);

			ClearGrid();
		}

		
		/// <summary>
		/// Sets all point values to 0.
		/// </summary>
		private void ClearGrid()
		{
			gridGraphics.FillRectangle (Brushes.Black, gridRect);
		}


		/// <summary>
		/// Disposes graphic resources used by the grid.
		/// </summary>
		private void DisposeGridResources()
		{
			if (gridGraphics != null)
			{
				gridGraphics.Dispose();
				gridGraphics = null;
			}

			if (gridBitmap != null)
			{
				gridBitmap.Dispose();
				gridBitmap = null;
			}
		}


		///////////////////////////////////////////////////////////////////////

        private int ToColorIntensity(double probability)
        { 
            double scaledValue = Math.Min (200*probability + 0.1, 1);
            return (int) (255*scaledValue);
        }

        private void ColorPoint(int x, int y, Color color)
        {
            Rectangle cellRect = new Rectangle(gridRect.Left + x * pointSize, gridRect.Top + y * pointSize, pointSize, pointSize);
            gridGraphics.FillRectangle(new SolidBrush(color), cellRect);
        }


		/// <summary>
		/// Colors the point corresponding to its probability.
		/// </summary>
		public void SetPointProbability (int x, int y, double probability)
		{
            int intensity = ToColorIntensity(probability);
            Color color = Color.FromArgb (0, intensity, 0);
            ColorPoint (x, y, color);
		}


        /// <summary>
        /// Colors the point corresponding to its amplitude.
        /// </summary>
        public void SetPointAmplitude(int x, int y, double amplitude)
        {
            int intensity = ToColorIntensity(amplitude*amplitude);
            Color color = (amplitude > 0) ? Color.FromArgb(intensity, 0, 0) : Color.FromArgb(0, 0, intensity);
            ColorPoint(x, y, color);
        }


		protected override void OnPaint (PaintEventArgs e)
		{
			// Repaint requested grid area (copies grid area from the local bitmap)
			e.Graphics.DrawImage (gridBitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
		}


		protected override void OnPaintBackground (PaintEventArgs e)
		{}
	}
}
