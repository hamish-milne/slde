using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	public class IDETab<T> : IDETab where T : Control, new()
	{
		static List<IDETab<T>> allTabs = new List<IDETab<T>>();

		new public static IList<IDETab<T>> AllTabs
		{
			get { return allTabs; }
		}

		static IDETab<T> activeTab;

		new public static IDETab<T> ActiveTab
		{
			get { return activeTab; }
			private set { activeTab = value; }
		}

		new public static TabControl ActivePane
		{
			get { return activeTab == null ? null : activeTab.Parent as TabControl; }
		}

		static IDETab()
		{
			ChangeActive += IDETab_ChangeActive;
		}

		static void IDETab_ChangeActive(object sender, EventArgs e)
		{
			var tab = sender as IDETab<T>;
			if (tab != null)
				ActiveTab = tab;
		}

		protected T control;

		public T Control
		{
			get { return control; }
		}

		public override void MakeActive()
		{
			base.MakeActive();
			control.Focus();
		}

		public IDETab()
			: base()
		{
			control = new T();
			control.Parent = this;
			control.FillParent();
			control.Enter += control_Enter;
			if (ActiveTab == null)
				ActiveTab = this;
			allTabs.Add(this);
		}

		void control_Enter(object sender, EventArgs e)
		{
			SetActiveTab();
		}

		protected override void Dispose(bool disposing)
		{
			allTabs.Remove(this);
			base.Dispose(disposing);
		}
	}
}
