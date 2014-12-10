﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// An improved TabPage that nicely changes the index of its parent
	/// when destroyed, and tracks focus.
	/// </summary>
	public partial class IDETab : TabPage
	{
		static List<IDETab> allTabs = new List<IDETab>();

		public static IList<IDETab> AllTabs
		{
			get { return allTabs; }
		}

		static IDETab activeTab;
		public static IDETab ActiveTab
		{
			get 
			{
				if(activeTab == null)
				{
					for(int i = 0; i < allTabs.Count; i++)
					{
						activeTab = allTabs[i];
						if (activeTab != null && !activeTab.IsDisposed)
							break;
					}
				}
				return activeTab;
			}
			private set
			{
				if (value != activeTab)
				{
					if (activeTab != null && activeTab.OnInactive != null)
						activeTab.OnInactive(activeTab, null);
					if (ChangeActive != null)
						ChangeActive(value, null);
					if (value != null && value.OnActive != null)
						value.OnActive(value, null);
					activeTab = value;
				}
			}
		}

		public static TabControl ActivePane
		{
			get { return ActiveTab == null ? null : ActiveTab.Parent as TabControl; }
		}

		public IDETab()
		{
			InitializeComponent();
			if (ActiveTab == null)
				ActiveTab = this;
			allTabs.Add(this);
			ImageIndex = 1;
		}

		protected void SetActiveTab()
		{
			ActiveTab = this;
		}

		public virtual void MakeActive()
		{
			var tabs = Parent as TabControl;
			if (tabs != null)
				tabs.SelectedTab = this;
		}

		public bool Active
		{
			get { return ActiveTab == this; }
		}

		public static event EventHandler ChangeActive;
		public event EventHandler OnActive;
		public event EventHandler OnInactive;
		TabControl oldParent;

		public virtual void Remove()
		{
			var tabs = Parent as TabControl;
			if (tabs != null)
			{
				int newIndex;
				if (tabs.SelectedIndex == 0)
					newIndex = 0;
				else if (tabs.SelectedIndex >= tabs.TabCount - 1)
					newIndex = tabs.TabCount - 2;
				else
					newIndex = tabs.SelectedIndex;
				Parent = null;
				tabs.SelectedIndex = newIndex;
			}
		}

		protected virtual void IDETab_ParentChanged(object sender, EventArgs e)
		{
			if (oldParent != null)
			{
				oldParent.SelectedIndexChanged -= Parent_SelectedIndexChanged;
				oldParent.GotFocus -= Parent_SelectedIndexChanged;
			}
			var newParent = Parent as TabControl;
			if (newParent != null)
			{
				newParent.SelectedIndexChanged += Parent_SelectedIndexChanged;
				newParent.GotFocus += Parent_SelectedIndexChanged;
				if (!allTabs.Contains(this))
					allTabs.Add(this);
			} else
			{
				allTabs.Remove(this);
				if (ActiveTab == this)
					ActiveTab = null;
			}
			oldParent = newParent;
		}

		void Parent_SelectedIndexChanged(object sender, EventArgs e)
		{
			var tabs = sender as TabControl;
			if (tabs == null)
				return;
			var ideTab = tabs.SelectedTab as IDETab;
			if (ideTab != null)
				ActiveTab = ideTab;
		}
	}
}
