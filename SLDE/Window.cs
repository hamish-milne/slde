using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SLDE
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class Window : ToolStripMenuItem
	{
		protected TabPage windowTab;
		protected TabControl lastParent;

		public TabPage WindowTab
		{
			get { return windowTab; }
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			Checked = !Checked;
			if (Checked)
			{
				if (lastParent == null)
					lastParent = IDETab.ActivePane;
				if (lastParent == null)
					return;
				windowTab.Parent = lastParent;
				windowTab.Refresh();
			} else
			{
				if (windowTab == null)
					return;
				lastParent = windowTab.Parent as TabControl;
				windowTab.Parent = null;
			}
		}

		public Window(string name, TabPage windowTab)
		{
			Text = name;
			this.windowTab = windowTab;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class WindowAttribute : Attribute
	{
	}

	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class ViewMenu : ToolStripMenuItem
	{
		public ViewMenu() : base()
		{
		}

		public void UpdateWindows()
		{
			DropDownItems.AddRange(Utility.CreateListOf<Window, WindowAttribute>(OnFailToAdd).ToArray());
		}

		void OnFailToAdd(Type t)
		{
			Utility.ShowError("Unable to add Window " + t);
		}
	}

	[Window]
	public class ProjectWindow : Window
	{
		public ProjectWindow()
			: base("Project", new IDETab<ProjectView>())
		{
		}
	}
}
