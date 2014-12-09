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
		private EditorTab CreateEditorTab(TabControl parent)
		{
			var ret = new EditorTab();
			ret.OnActive += ret_OnActive;
			parent.TabPages.Add(ret);
			ret.ImageIndex = 1;
			ret.MakeActive();
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

		private EditorTab CreateEditorTab(string fileName, TabControl parent)
		{
			var ret = new EditorTab(fileName);
			ret.OnActive += ret_OnActive;
			parent.TabPages.Add(ret);
			ret.ImageIndex = 1;
			ret.MakeActive();
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

		public virtual TabControl ActivePane
		{
			get
			{
				var tab = EditorTab.ActiveEditorTab;
				return tab == null ? rootTabControl : (TabControl)tab.Parent;
			}
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
			CreateEditorTab(file, ActivePane);
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
			var ret = new CustomTab();
			ret.ImageList = imageList;
			ret.MouseMove += TabControl_MouseMove;
			ret.MouseClick += TabControl_MouseClick;
			ret.Anchor = Utility.AllAnchors;
			ret.ContextMenuStrip = rootTabControl.ContextMenuStrip;
			ret.MainWindow = true;
			CreateEditorTab(ret);
			ret.HotTrack = true;
			return ret;
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

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
				//MoveTab(movingTab, tabs);
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
			return;
		}

		private void rootTabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			Console.WriteLine(e.Bounds);
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			CreateEditorTab(ActivePane);
		}

		private void TabControl_MouseMove(object sender, MouseEventArgs e)
		{
			var tabs = sender as TabControl;
			if (tabs == null)
				return;
			
		}

		private void TabControl_MouseLeave(object sender, EventArgs e)
		{
			var tabs = sender as TabControl;
			if (tabs == null)
				return;
			
		}

		private void TabControl_MouseClick(object sender, MouseEventArgs e)
		{
			var tabs = sender as TabControl;
			if (tabs == null)
				return;
			
		}

		private void TabControl_MouseDown(object sender, MouseEventArgs e)
		{

		}
	}
}
