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
			//ret.Refresh();
			return ret;
		}

		void ret_OnActive(object sender, EventArgs e)
		{
			Console.WriteLine("OnActive " + sender);
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

		protected virtual SplitContainer CreateSplitContainer()
		{
			var ret = new SplitContainer();
			ret.Anchor = Utility.AllAnchors;
			ret.DoubleClick += splitContainer_DoubleClick;
			return ret;
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
			if (newTabs.SelectedTab is IDETab<Editor>)
				((IDETab<Editor>)newTabs.SelectedTab).Control.Focus();
			else
				newTabs.SelectedTab.Focus();
		}

		private void saveFile_Click(object sender, EventArgs e)
		{
			var tab = IDETab<Editor>.ActiveTab;
			if (tab != null)
			{
				saveFileDialog.Filter = Language.GetFilter();
				tab.Control.Save(saveFileDialog);
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
			IDETab<ProjectView>.ActiveTab.Refresh();
			IDETab.ActiveTab.Refresh();
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			CreateEditorTab(ActivePane);
		}

	}
}
