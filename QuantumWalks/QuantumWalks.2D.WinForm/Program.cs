using System;
using System.Windows.Forms;

namespace CQCS.QuantumWalks.Grid2D.WinForm
{
	public class Program
	{
		[STAThread]
		static void Main() 
		{
            try
            {
                Application.Run(new QuantumWalkSimulatorForm());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
		}
	}
}
