
namespace CQCS.QuantumWalks.Grid2D.WinForm
{
	public class QuantumWalkSimulatorForm : System.Windows.Forms.Form
	{
		#region Windows Form Designer variables

		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblStep;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblScalarProduct;
        private QuantumWalkGrid quantumWalkGrid;
        private System.Windows.Forms.Label lblProbability;
        private System.Windows.Forms.Label label4;
		private System.ComponentModel.IContainer components;

		#endregion
		
		#region Windows Form Designer generated code
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lblStep = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblScalarProduct = new System.Windows.Forms.Label();
            this.quantumWalkGrid = new QuantumWalkGrid();
            this.lblProbability = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartStop.Location = new System.Drawing.Point(529, 623);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(80, 24);
            this.btnStartStop.TabIndex = 1;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(422, 627);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Step:";
            // 
            // lblStep
            // 
            this.lblStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStep.Location = new System.Drawing.Point(469, 627);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(40, 19);
            this.lblStep.TabIndex = 2;
            this.lblStep.Text = "0";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Location = new System.Drawing.Point(12, 628);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Overlap:";
            // 
            // lblScalarProduct
            // 
            this.lblScalarProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblScalarProduct.Location = new System.Drawing.Point(76, 628);
            this.lblScalarProduct.Name = "lblScalarProduct";
            this.lblScalarProduct.Size = new System.Drawing.Size(128, 23);
            this.lblScalarProduct.TabIndex = 2;
            this.lblScalarProduct.Text = "1";
            // 
            // quantumWalkGrid
            // 
            this.quantumWalkGrid.HeightInPoints = 120;
            this.quantumWalkGrid.Location = new System.Drawing.Point(9, 12);
            this.quantumWalkGrid.Name = "quantumWalkGrid";
            this.quantumWalkGrid.PointSize = 5;
            this.quantumWalkGrid.Size = new System.Drawing.Size(600, 600);
            this.quantumWalkGrid.TabIndex = 3;
            this.quantumWalkGrid.WidthInPoints = 120;
            // 
            // lblProbability
            // 
            this.lblProbability.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProbability.Location = new System.Drawing.Point(292, 627);
            this.lblProbability.Name = "lblProbability";
            this.lblProbability.Size = new System.Drawing.Size(124, 23);
            this.lblProbability.TabIndex = 5;
            this.lblProbability.Text = "0";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.Location = new System.Drawing.Point(210, 627);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "Probability:";
            // 
            // QuantumWalkSimulatorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 16);
            this.ClientSize = new System.Drawing.Size(624, 660);
            this.Controls.Add(this.lblProbability);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.quantumWalkGrid);
            this.Controls.Add(this.lblScalarProduct);
            this.Controls.Add(this.lblStep);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "QuantumWalkSimulatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quantum walk simulator";
            this.ResumeLayout(false);

		}

		#endregion

		////////////////////////////////////////////////////////////////////////
        
		readonly QuantumWalkSimulatorCoinedRectangle qws;

        public QuantumWalkSimulatorForm()
		{
			InitializeComponent();

            qws = new QuantumWalkSimulatorCoinedRectangle (quantumWalkGrid.HeightInPoints, quantumWalkGrid.WidthInPoints);

            MarkVertices();
        }

        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose (disposing);
		}

        ////////////////////////////////////////////////////////////////////////

		private void MarkVertices()
        {
            qws.MarkVertex(10, 10);

            MarkPoints(MarkedVertices.Square(4, 20, 20));
        }


        private void MarkPoints(params Vertex[] vertices)
        {
            foreach (Vertex v in vertices)
			    qws.MarkVertex (v);
        }

        private void UnMarkPoints(params Vertex[] vertices)
        {
            foreach (Vertex v in vertices)
			    qws.UnMarkVertex(v);
        }

		////////////////////////////////////////////////////////////////////////

		private void btnRun_Click (object sender, System.EventArgs e)
		{
            if (btnStartStop.Text == "Start")
			{
				btnStartStop.Text = "Stop";
				timer.Start();
			}
			else
			{
                btnStartStop.Text = "Start";
				timer.Stop();
			}
		}

		private void timer_Tick (object sender, System.EventArgs e)
		{
			timer.Stop();

			// Do step
			qws.Run();

			lblStep.Text = qws.T.ToString();
            lblScalarProduct.Text = qws.GetScalarProduct().ToString("F10");
            lblProbability.Text = qws.GetMarkedVertexProbability().ToString("F10");

			// Show point probabilities
			for (int y = 0; y < quantumWalkGrid.HeightInPoints; y++)
			for (int x = 0; x < quantumWalkGrid.WidthInPoints; x++)
			{
                quantumWalkGrid.SetPointProbability(x, y, qws.GetVertexProbability(x, y));
			}

			quantumWalkGrid.Refresh();

			timer.Start();
		}
    }
}
