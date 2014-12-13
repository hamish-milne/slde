using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// An IDETab that encapsulates a single control
	/// </summary>
	/// <typeparam name="T">The type of control to create</typeparam>
	public class IDETab<T> : IDETab where T : Control, IClosable, new()
	{
		static List<IDETab<T>> allTabs = new List<IDETab<T>>();

		/// <summary>
		/// Lists all instances of this type
		/// </summary>
		new public static IList<IDETab<T>> AllTabs
		{
			get { return allTabs; }
		}

		static IDETab<T> activeTab;

		/// <summary>
		/// Gets the last active tab of this type
		/// </summary>
		new public static IDETab<T> ActiveTab
		{
			get { return activeTab; }
			private set { activeTab = value; }
		}

		/// <summary>
		/// Gets the TabControl associated with the active tab
		/// </summary>
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

		/// <summary>
		/// The control in the tab's body
		/// </summary>
		public T Control
		{
			get { return control; }
		}

		/// <summary>
		/// Makes the tab active and focuses the control
		/// </summary>
		public override void MakeActive()
		{
			base.MakeActive();
			control.Focus();
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
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

		/// <summary>
		/// Creates a new instance with the given text
		/// </summary>
		/// <param name="text">The text to set</param>
		public IDETab(string text)
			: this()
		{
			Text = text;
		}

		void control_Enter(object sender, EventArgs e)
		{
			SetActiveTab();
		}

		/// <summary>
		/// Ensures success on the control's TryClose method before destroying
		/// </summary>
		/// <returns><c>true</c> on success, <c>false</c> on failure or cancel</returns>
		public override bool Destroy()
		{
			if(control.TryClose())
			{
				allTabs.Remove(this);
				return base.Destroy();
			}
			return false;
		}
	}
}
