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
	public partial class MainIDE : IDEForm
	{
		IDETabControl ActivePane
		{
			get
			{
				IDETabControl ret = IDETab.ActivePane as IDETabControl;
				if (ret == null && IDETabControl.AllControls.Count > 0)
					ret = IDETabControl.AllControls[0];
				return ret;
			}
		}

		IDETabControl ActiveEditorPane
		{
			get
			{
				IDETabControl ret = IDETab<Editor>.ActivePane as IDETabControl;
				if (ret == null && IDETabControl.AllControls.Count > 0)
					ret = IDETabControl.AllControls[0];
				return ret;
			}
		}

		private IDETab<Editor> CreateEditorTab(TabControl parent)
		{
			var ret = new IDETab<Editor>();
			ret_OnActive(ret, null);
			ret.OnActive += ret_OnActive;
			parent.TabPages.Add(ret);
			ret.ImageIndex = 1;
			ret.MakeActive();
			ret.Refresh();
			return ret;
		}

		void ret_OnActive(object sender, EventArgs e)
		{
			var tab = (IDETab<Editor>)sender;
			languageMenu.SelectLanguage(tab.Control.Language, true);

		}

		void languageMenu_OnSelectLanguage(object sender, LanguageSelectEventArgs e)
		{
			var tab = IDETab<Editor>.ActiveTab;
			if (tab != null)
				tab.Control.Language = e.Language;
		}

		public MainIDE()
		{
			InitializeComponent();
			DialogCache.SaveFile = saveFileDialog;
			DialogCache.OpenFile = openFileDialog;
			Language.FindAllLanguages();
			languageMenu.OnSelectLanguage += languageMenu_OnSelectLanguage;
			viewMenu.UpdateWindows();
		}

		public MainIDE(string[] arguments) : this()
		{
			bool openedFiles = false;
			foreach (var arg in arguments)
				openedFiles |= TryOpenFile(arg);
			if (!openedFiles)
			{
				CreateEditorTab(ActiveEditorPane);
			}

		}

		bool TryOpenFile(string file)
		{
			var tab = CreateEditorTab(ActiveEditorPane);
			return tab.Control.TryOpen(file);
		}

		void Control_OpenFile(object sender, OpenFileEventArgs e)
		{
			TryOpenFile(e.File);
		}

		protected virtual TabControl CreateTabControl()
		{
			var ret = new IDETabControl();
			ret.ImageList = imageList;
			ret.ContextMenuStrip = tabContextMenu;
			ret.MainWindow = true;
			CreateEditorTab(ret);
			return ret;
		}

		private void close_Click(object sender, EventArgs e)
		{
			var tab = sender.GetSourceControl() as TabControl;
			if (tab == null)
				return;
			var tabPage = tab.SelectedTab as IDETab;
			if (tabPage != null)
				tabPage.Destroy();
		}

		private void closePane_Click(object sender, EventArgs e)
		{
			var tab = sender.GetSourceControl() as IDETabControl;
			if (tab != null)
				tab.Close(true);
		}

		private void saveFile_Click(object sender, EventArgs e)
		{
			var tab = IDETab<Editor>.ActiveTab;
			if (tab != null)
			{
				DialogCache.SaveFile.Filter = Language.GetFilter();
				tab.Control.Save();
			}
		}

		private void openFile_Click(object sender, EventArgs e)
		{
			openFileDialog.Filter = Language.GetFilter();
			if(openFileDialog.ShowDialog() == DialogResult.OK)
			{
				var names = openFileDialog.FileNames;
				for (int i = 0; i < names.Length; i++)
					TryOpenFile(names[i]);
			}
		}

		private void undo_Click(object sender, EventArgs e)
		{
			var tab = IDETab<Editor>.ActiveTab;
			if (tab != null)
				tab.Control.Undo();
		}

		private void redo_Click(object sender, EventArgs e)
		{
			var tab = IDETab<Editor>.ActiveTab;
			if (tab != null)
				tab.Control.Redo();
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			CreateEditorTab(ActivePane);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void saveAllButton_Click(object sender, EventArgs e)
		{
			for(int i = 0; i < IDETab<Editor>.AllTabs.Count; i++)
			{
				var tab = IDETab<Editor>.AllTabs[i];
				if (tab != null)
					tab.Control.Save();
			}
		}

		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			var tab = IDETab<Editor>.ActiveTab;
			if (tab != null)
			{
				DialogCache.SaveFile.Filter = Language.GetFilter();
				tab.Control.Save(true);
			}
		}

		List<IDETab> tabList = new List<IDETab>();

		private void closeAllButThis_Click(object sender, EventArgs e)
		{
			var tabs = sender.GetSourceControl() as TabControl;
			if(tabs == null)
				return;
			tabList.Clear();
			for (int i = 0; i < tabs.TabCount; i++)
				if(tabs.TabPages[i] is IDETab)
					tabList.Add((IDETab)tabs.TabPages[i]);
			for (int i = 0; i < tabList.Count; i++)
				if (tabList[i] != tabs.SelectedTab && !tabList[i].Destroy())
					break;
		}

		private void splitPaneHorizontal_Click(object sender, EventArgs e)
		{
			var tabs = sender.GetSourceControl() as IDETabControl;
			if (tabs == null)
				return;
			tabs.Split(true, false);
		}

		private void splitPaneVertical_Click(object sender, EventArgs e)
		{
			var tabs = sender.GetSourceControl() as IDETabControl;
			if (tabs == null)
				return;
			tabs.Split(true, true);
		}

		private void splitVerticalMenuItem_Click(object sender, EventArgs e)
		{
			ActivePane.Split(true, true);
		}

		private void splitHorizontalMenuItem_Click(object sender, EventArgs e)
		{
			ActivePane.Split(true, false);
		}

	}
}
