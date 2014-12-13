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
	/// <summary>
	/// The error list window
	/// </summary>
	public partial class ErrorList : UserControl, IClosable
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public ErrorList()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Closes the tab
		/// </summary>
		/// <returns><c>true</c></returns>
		public virtual bool TryClose()
		{
			return true;
		}

		private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}
	}
}
