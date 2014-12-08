using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	[ToolboxItem(true)]
	public partial class CustomTab : TabControl
	{
		public CustomTab()
		{
			InitializeComponent();
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				cp.ExStyle |= 0x2000000;
				return cp;
			}
		}
		
	}
}
