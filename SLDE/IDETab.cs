using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// An improved TabPage that nicely changes the index of its parent when destroyed
	/// </summary>
	public partial class IDETab : TabPage
	{
		public IDETab()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			var tabs = Parent as TabControl;
			if (tabs != null)
			{
				int newIndex;
				if (tabs.SelectedIndex == 0)
					newIndex = 0;
				else if (tabs.SelectedIndex == tabs.TabCount - 1)
					newIndex = tabs.TabCount - 2;
				else
					newIndex = tabs.SelectedIndex;
				base.Dispose(disposing);
				tabs.SelectedIndex = newIndex;
			}
			else
			{
				base.Dispose(disposing);
			}
		}
	}
}
