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
		public const AnchorStyles AllAnchors =
			AnchorStyles.Bottom |
			AnchorStyles.Left |
			AnchorStyles.Right |
			AnchorStyles.Top;

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

		protected virtual SplitContainer CreateSplitContainer()
		{
			var ret = new SplitContainer();
			ret.Anchor = AllAnchors;
			ret.DoubleClick += splitContainer_DoubleClick;
			return ret;
		}

		protected virtual TabControl CreateTabControl()
		{
			var ret = new TabControl();
			ret.Anchor = AllAnchors;
			ret.ContextMenuStrip = tabContextMenu;
			return ret;
		}

		static void FillParent(Control content)
		{
			content.Location = default(Point); //new Point(content.Margin.Left, content.Margin.Top);
			content.Size = content.Parent.Size; //- content.Margin.Size;
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
			FillParent(content);
			container.Dispose();
		}

		private void splitContainer_DoubleClick(object sender, EventArgs e)
		{
			var split = sender as SplitContainer;
			if(split != null)
				split.Orientation = split.Orientation == Orientation.Horizontal
					? Orientation.Vertical : Orientation.Horizontal;
		}

		private void splitPane_Click(object sender, EventArgs e)
		{
			var content = GetSourceControl(sender);
			if (content == null)
				return;
			var splitPane = content.Parent as SplitterPanel;
			if (splitPane == null)
				return;
			var oldOrientation = ((SplitContainer)splitPane.Parent).Orientation;
			var newContainer = CreateSplitContainer();
			newContainer.Orientation = oldOrientation == Orientation.Horizontal
				? Orientation.Vertical : Orientation.Horizontal;
			newContainer.Parent = content.Parent;
			FillParent(newContainer);
			content.Parent = newContainer.Panel1;
			FillParent(content);
			var newTabs = CreateTabControl();
			newTabs.Parent = newContainer.Panel2;
			FillParent(newTabs);
		}
	}
}
