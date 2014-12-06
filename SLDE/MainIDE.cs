using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	public partial class MainIDE : Form
	{
		public MainIDE()
		{
			InitializeComponent();
		}

		static Control GetSourceControl(object sender)
		{
			Control ret = null;
			try
			{
				ret = ((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl;
			}
			catch
			{
			}
			return ret;
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void selectConfig_Click(object sender, EventArgs e)
		{

		}

		private void close_Click(object sender, EventArgs e)
		{
			var tab = GetSourceControl(sender) as TabControl;
			if (tab != null)
				tab.TabPages.RemoveAt(tab.SelectedIndex);
		}

		private void closePane_Click(object sender, EventArgs e)
		{
			var content = GetSourceControl(sender);
			if (content == null)
				return;
			var splitPane = content.Parent as SplitterPanel;
			if (splitPane == null)
				return;
			var container = (SplitContainer)splitPane.Parent;
			var otherPane = splitPane == container.Panel1 ? container.Panel2 : container.Panel1;
			Console.WriteLine(otherPane);
			if (otherPane.Controls.Count < 1)
				return;
			content = otherPane.Controls[0];
			content.Parent = container.Parent;
			//content.Dock = DockStyle.Fill;
			content.Location = new Point(content.Margin.Left, content.Margin.Top);
			content.Size = content.Parent.Size - content.Margin.Size;
			container.Dispose();
		}
	}
}
