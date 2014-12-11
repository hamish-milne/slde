using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	public partial class ErrorList : UserControl, IClosable
	{
		public ErrorList()
		{
			InitializeComponent();
		}

		public virtual bool TryClose()
		{
			return true;
		}

		private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}
	}
}
