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
		private EditorTab CreateEditorTab()
		{
			var ret = new EditorTab();
			ret.OnActive += ret_OnActive;
			return ret;
		}

		void ret_OnActive(object sender, EventArgs e)
		{
			Console.WriteLine("OnActive " + sender);
			var tab = (EditorTab)sender;
			languageMenu.SelectLanguage(tab.Language, true);

		}

		void languageMenu_OnSelectLanguage(object sender, LanguageSelectEventArgs e)
		{
			var tab = EditorTab.ActiveEditorTab;
			if (tab != null)
				tab.Language = e.Language;
		}

		private EditorTab CreateEditorTab(string fileName)
		{
			var ret = new EditorTab(fileName);
			return ret;
		}

		public MainIDE()
		{
			InitializeComponent();
			Language.FindAllLanguages();
			languageMenu.OnSelectLanguage += languageMenu_OnSelectLanguage;
		}

		public MainIDE(string[] arguments) : this()
		{
			bool openedFiles = false;
			foreach (var arg in arguments)
				openedFiles |= TryOpenFile(arg);
			if (!openedFiles)
			{
				rootTabControl.TabPages.Add(CreateEditorTab());
				rootTabControl.TabPages.Add(CreateEditorTab());
				rootTabControl.TabPages.Add(CreateEditorTab());
			}

		}

		public TabControl GetActivePane()
		{
			return (TabControl)EditorTab.ActiveEditorTab.Parent;
		}


		public bool TryOpenFile(string file)
		{
			try
			{
				OpenFile(file);
				return true;
			} catch(Exception e)
			{
				Utility.ShowError(e.Message);
				return false;
			}
		}

		public void OpenFile(string file)
		{
			var tab = CreateEditorTab(file);
			GetActivePane().TabPages.Add(tab);
		}

		protected virtual SplitContainer CreateSplitContainer()
		{
			var ret = new SplitContainer();
			ret.Anchor = Utility.AllAnchors;
			ret.DoubleClick += splitContainer_DoubleClick;
			return ret;
		}

		protected virtual TabControl CreateTabControl()
		{
			var ret = new TabControl();
			ret.Anchor = Utility.AllAnchors;
			ret.ContextMenuStrip = tabContextMenu;
			ret.TabPages.Add(CreateEditorTab());
			ret.HotTrack = true;
			return ret;
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void close_Click(object sender, EventArgs e)
		{
			var tab = sender.GetSourceControl() as TabControl;
			if (tab != null)
				CloseTab(tab.TabPages[tab.SelectedIndex]);
		}

		void CloseTab(TabPage tab)
		{
			if (tab == null)
				return;
			var tabs = (TabControl)tab.Parent;
			if (tabs.TabPages.Count <= 1)
				ClosePane(tabs);
			else
				tabs.TabPages.Remove(tab);
		}

		void MoveTab(TabPage tab, TabControl newLocation)
		{
			if (tab == null || newLocation == null)
				return;
			var otherTabs = (TabControl)tab.Parent;
			newLocation.TabPages.Insert(newLocation.SelectedIndex + 1, tab);
			if (otherTabs.TabPages.Count == 0)
				ClosePane(otherTabs);
		}

		void ClosePane(Control content)
		{
			if (content == null)
				return;
			var splitPane = content.Parent as SplitterPanel;
			if (splitPane == null)
				return;
			var container = (SplitContainer)splitPane.Parent;
			var otherPane = splitPane == container.Panel1 ? container.Panel2 : container.Panel1;
			if (otherPane.Controls.Count < 1)
				return;
			content = otherPane.Controls[0];
			content.Parent = container.Parent;
			content.FillParent();
			container.Dispose();
		}

		private void closePane_Click(object sender, EventArgs e)
		{
			ClosePane(sender.GetSourceControl());
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
			var content = sender.GetSourceControl();
			if (content == null)
				return;
			var parent = content.Parent;
			if (parent == null)
				return;
			var newContainer = CreateSplitContainer();
			if(parent is SplitterPanel)
			{
				var oldOrientation = ((SplitContainer)parent.Parent).Orientation;
				newContainer.Orientation = oldOrientation == Orientation.Horizontal
					? Orientation.Vertical : Orientation.Horizontal;
			}
			newContainer.Parent = content.Parent;
			newContainer.FillParent();
			content.Parent = newContainer.Panel1;
			content.FillParent();
			var newTabs = CreateTabControl();
			newTabs.Parent = newContainer.Panel2;
			newTabs.FillParent();
			if (newTabs.SelectedTab is EditorTab)
				((EditorTab)newTabs.SelectedTab).TextEditor.Focus();
			else
				newTabs.SelectedTab.Focus();
		}

		TabPage movingTab;

		private void moveTab_Click(object sender, EventArgs e)
		{
			var tabs = sender.GetSourceControl() as TabControl;
			if(tabs == null)
				return;
			if(movingTab == null)
			{
				movingTab = tabs.TabPages[tabs.SelectedIndex];
				statusLabel.Text = "Moving tab";
			}
			else
			{
				MoveTab(movingTab, tabs);
				movingTab = null;
				statusLabel.Text = "Ready";
			}
		}

		private void saveFile_Click(object sender, EventArgs e)
		{
			var tab = EditorTab.ActiveEditorTab;
			if (tab != null)
			{
				saveFileDialog.Filter = Language.GetFilter();
				tab.Save(saveFileDialog);
			}

		}
	}
}
