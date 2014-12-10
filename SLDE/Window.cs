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
		protected IDETab windowTab;
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
				windowTab.MakeActive();
				windowTab.Refresh();
			} else
			{
				if (windowTab == null)
					return;
				lastParent = windowTab.Parent as TabControl;
				windowTab.Remove();
			}
		}

		public Window(string name, IDETab windowTab)
		{
			Text = name;
			this.windowTab = windowTab;
			if(windowTab != null)
				windowTab.ParentChanged += windowTab_ParentChanged;
		}

		void windowTab_ParentChanged(object sender, EventArgs e)
		{
			if (windowTab.Parent == null)
				Checked = false;
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
			DropDownItems.Clear();
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
			: base("Project", new IDETab<ProjectView>("Project"))
		{
		}
	}

	[Window]
	public class ErrorWindow : Window
	{
		public ErrorWindow()
			: base("Error list", new IDETab<ErrorList>("Error list"))
		{
		}
	}
}
