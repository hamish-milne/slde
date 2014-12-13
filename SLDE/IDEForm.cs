using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// A form that closes all <see cref="IDETab"/>s before closing itself
	/// </summary>
	public class IDEForm : Form
	{
		List<IDETab> tabs = new List<IDETab>();

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			e.Cancel = false;
			tabs.Clear();
			for(int i = 0; i < IDETab.AllTabs.Count; i++)
			{
				if (IDETab.AllTabs[i].FindForm() == this)
					tabs.Add(IDETab.AllTabs[i]);
			}
			for(int i = 0; i < tabs.Count; i++)
				if(!tabs[i].Destroy())
				{
					e.Cancel = true;
					break;
				}
			tabs.Clear();
		}
		
	}
}
