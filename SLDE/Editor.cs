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
	public partial class Editor : DigitalRune.Windows.TextEditor.TextEditorControl, ITabNameOverride
	{
		static int newFilesNumber;
		static int newFileID;

		bool changed;
		string tabName;

		public Editor() : base()
		{
			InitializeComponent();

			DocumentChanged += OnTextChange;
			newFilesNumber++;
			newFileID = newFilesNumber;
			tabName = "New file " + newFilesNumber;
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
				tabName = Path.GetFileName(value);
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
				var lastChar = String.IsNullOrEmpty(tabName) ? '\0' : tabName[tabName.Length - 1];
				if (value && lastChar != '*')
					tabName += "*";
				else if (!value && lastChar == '*')
					tabName = tabName.Substring(0, tabName.Length - 1);
			}
		}

		public string TabName
		{
			get { return tabName; }
		}

		public virtual void TrySave()
		{
			if (FileName != null)
			{
				try
				{
					SaveFile(FileName);
					Changed = false;
				}
				catch (Exception e)
				{
					Utility.ShowError(e.Message);
				}
			}
		}

		public virtual void Save(SaveFileDialog saveFileDialog)
		{
			if(!Changed)
				return;
			if(String.IsNullOrEmpty(FileName))
			{
				saveFileDialog.FileOk += SaveCallback;
				saveFileDialog.ShowDialog();
			}
			else
			{
				TrySave();
			}
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

		protected void SaveCallback(object sender, EventArgs e)
		{
			var dialog = (SaveFileDialog)sender;
			dialog.FileOk -= SaveCallback;
			FileName = dialog.FileName;
			TrySave();
		}
	}
}
