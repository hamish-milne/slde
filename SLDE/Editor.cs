using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Document;

namespace SLDE
{
	public partial class Editor : TextEditorControl, IClosable
	{
		static int newFilesNumber;
		static int newFileID;
		string tabName;
		bool changed;

		public Editor() : base()
		{
			InitializeComponent();

			DocumentChanged += OnTextChange;
			newFilesNumber++;
			newFileID = newFilesNumber;
			TabName = "New file " + newFilesNumber;
		}

		protected override void Dispose(bool disposing)
		{
			if (String.IsNullOrEmpty(FileName) && newFileID >= newFilesNumber)
				newFilesNumber--;
			base.Dispose(disposing);
		}

		new public string FileName
		{
			get { return base.FileName; }
			set
			{
				if (String.IsNullOrEmpty(base.FileName))
					newFilesNumber--;
				base.FileName = value;
				TabName = Path.GetFileName(value);
				Changed = Changed;
				if (String.IsNullOrEmpty(value))
					newFilesNumber++;
			}
		}

		public virtual bool Changed
		{
			get { return changed; }
			set
			{
				changed = value;
				var lastChar = String.IsNullOrEmpty(TabName) ?
					'\0' : TabName[TabName.Length - 1];
				if (value && lastChar != '*')
					TabName += "*";
				else if (!value && lastChar == '*')
					TabName = TabName.Substring(0, TabName.Length - 1);
			}
		}

		public string TabName
		{
			get
			{
				var tab = Parent as TabPage;
				if (tab == null)
					return tabName;
				return tab.Text;
			}
			set
			{
				var tab = Parent as TabPage;
				if (tab != null)
					tab.Text = value;
				tabName = value;
			}
		}

		public virtual bool Save()
		{
			if(!Changed)
				return true;
			if(String.IsNullOrEmpty(FileName))
			{
				if (DialogCache.SaveFile.ShowDialog() == DialogResult.OK)
					FileName = DialogCache.SaveFile.FileName;
				else
					return false;
			}
			try
			{
				SaveFile(FileName);
				Changed = false;
				return true;
			}
			catch (Exception e)
			{
				Utility.ShowError(e.Message);
			}
			return false;
		}

		public virtual void TryOpen()
		{
			if(FileName != null)
			{
				try
				{
					Text = File.ReadAllText(FileName);
					Changed = false;
					Language = Language.GetByExtension(Path.GetExtension(FileName));
				} catch(Exception e)
				{
					Utility.ShowError(e.Message);
				}
			}
		}

		Language language;

		public virtual Language Language
		{
			get
			{
				if (language == null)
					language = NoLanguage.Instance;
				return language;
			}
			set
			{
				language = value;
				if (value == null)
					return;
				if (value.HighlightingStrategy == null)
					SetHighlighting("Default");
				else
					Document.HighlightingStrategy = value.HighlightingStrategy;
				if (value.FormattingStrategy == null)
					Document.FormattingStrategy = new DefaultFormattingStrategy();
				else
					Document.FormattingStrategy = value.FormattingStrategy;
				if (value.FoldingStrategy == null)
					Document.FoldingManager.FoldingStrategy = null;
				else
				{
					Document.FoldingManager.FoldingStrategy = value.FoldingStrategy;
					Document.FoldingManager.UpdateFolds();
				}

			}
		}

		void OnTextChange(object sender, DocumentEventArgs e)
		{
			Changed = true;
		}

		private void Editor_ParentChanged(object sender, EventArgs e)
		{
			var tab = Parent as TabPage;
			if (tab != null)
				tab.Text = tabName;
		}

		public virtual bool TryClose()
		{
			if(Changed)
			{
				var result = MessageBox.Show("Save changes to " +
					TabName.Substring(0, TabName.Length - 1) + "?", "Save",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				switch(result)
				{
					case DialogResult.Yes:
						return Save();
					case DialogResult.No:
						return true;
					default:
						return false;
				}
			}
			return true;
		}
	}

	public static class DialogCache
	{
		static OpenFileDialog openFile;
		static SaveFileDialog saveFile;

		public static OpenFileDialog OpenFile
		{
			get
			{
				if (openFile == null)
					openFile = new OpenFileDialog();
				return openFile;
			}
			set
			{
				openFile = value;
			}
		}

		public static SaveFileDialog SaveFile
		{
			get
			{
				if (saveFile == null)
					saveFile = new SaveFileDialog();
				return saveFile;
			}
			set
			{
				saveFile = value;
			}
		}
	}
}
