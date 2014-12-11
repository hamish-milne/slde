using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace SLDE
{
	public class IDEForm : Form
	{
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

		}

		void CloseRecursive(Control control)
		{

			
		}
	}
}
