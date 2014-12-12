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
		TabControl ActivePane
		{
			get
			{
				TabControl ret = IDETab.ActivePane;
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

		private IDETab<Editor> CreateEditorTab(string fileName, TabControl parent)
		{
			var ret = CreateEditorTab(parent);
			ret.Control.FileName = fileName;
			ret.Control.TryOpen();
			return ret;
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
				CreateEditorTab(rootTabControl);
			}

		}

		bool TryOpenFile(string file)
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

		void OpenFile(string file)
		{
			CreateEditorTab(file, ActivePane);
			var tab = new IDETab<ProjectView>();
			rootTabControl.TabPages.Add(tab);
			tab.Control.OpenFile += Control_OpenFile;
			tab.Control.Root = System.IO.Path.GetDirectoryName(file);
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
			/*var tab = sender.GetSourceControl() as TabControl;
			if (tab != null)
				CloseTab(tab.TabPages[tab.SelectedIndex]);*/
		}

		private void closePane_Click(object sender, EventArgs e)
		{
			//ClosePane(sender.GetSourceControl());
		}

		private void splitPane_Click(object sender, EventArgs e)
		{
			var content = sender.GetSourceControl() as IDETabControl;
			if (content == null)
				return;
			content.Split(true, true);
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
			openFileDialog.ShowDialog();
		}

		private void openFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			var dialog = (OpenFileDialog)sender;
			var names = dialog.FileNames;
			for (int i = 0; i < names.Length; i++)
				TryOpenFile(names[i]);
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
			Dispose();
		}

	}
}
