using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SLDE
{
	/// <summary>
	/// A non-editor window, i.e. one that can only have one instance
	/// </summary>
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class Window : ToolStripMenuItem
	{
		protected IDETab windowTab;
		protected TabControl lastParent;

		/// <summary>
		/// The TabPage instance that the window refers to
		/// </summary>
		public TabPage WindowTab
		{
			get { return windowTab; }
		}

		/// <summary>
		/// Enables and disables the window
		/// </summary>
		/// <param name="e"></param>
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

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="name">The name of the window</param>
		/// <param name="windowTab">The tab to control</param>
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

	/// <summary>
	/// Applied to a <see cref="Window"/> class to indicate it should be
	/// created dynamically
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class WindowAttribute : Attribute
	{
	}

	/// <summary>
	/// Stores and manages all <see cref="Window"/> instances
	/// </summary>
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class ViewMenu : ToolStripMenuItem
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public ViewMenu() : base()
		{
		}

		/// <summary>
		/// Updates the drop-down menu with the list of all windows
		/// </summary>
		public void UpdateWindows()
		{
			DropDownItems.Clear();
			DropDownItems.AddRange(Utility.CreateListOf<Window, WindowAttribute>(OnFailToAdd).ToArray());
		}

		void OnFailToAdd(Type t, Exception e)
		{
			Utility.ShowError("Unable to add Window " + t + ": " + e.Message);
		}
	}

	/// <summary>
	/// The window for <see cref="ProjectView"/>
	/// </summary>
	[Window]
	public class ProjectWindow : Window
	{
		public ProjectWindow()
			: base("Project", new IDETab<ProjectView>("Project"))
		{
		}
	}

	/// <summary>
	/// The window for <see cref="ErrorList"/>
	/// </summary>
	[Window]
	public class ErrorWindow : Window
	{
		public ErrorWindow()
			: base("Error list", new IDETab<ErrorList>("Error list"))
		{
		}
	}
}
