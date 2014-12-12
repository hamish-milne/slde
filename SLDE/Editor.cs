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
	/// <summary>
	/// Extensions to the TextEditorControl
	/// </summary>
	public partial class Editor : TextEditorControl, IClosable
	{
		int newFileNumber;
		string tabName;
		bool changed;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public Editor() : base()
		{
			InitializeComponent();

			DocumentChanged += OnTextChange;
			for (int i = 0; i < IDETab<Editor>.AllTabs.Count; i++)
			{
				var num = IDETab<Editor>.AllTabs[i].Control.newFileNumber;
				if(num > newFileNumber)
					newFileNumber = num;
			}
			newFileNumber++;
			TabName = "New file " + newFileNumber;
		}

		/// <summary>
		/// The current file name
		/// </summary>
		new public string FileName
		{
			get { return base.FileName; }
			set
			{
				base.FileName = value;
				TabName = Path.GetFileName(value);
				Changed = Changed;
				newFileNumber = 0;
			}
		}

		/// <summary>
		/// Indicates whether the document has been modified since the last save
		/// </summary>
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

		/// <summary>
		/// The text on the parent tab
		/// </summary>
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

		/// <summary>
		/// Saves the document, performing any necessary GUI actions
		/// </summary>
		/// <returns><c>true</c> on success, <c>false</c> on cancel or failure</returns>
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

		/// <summary>
		/// Tries to open the file referenced by FileName, showing an error on failure
		/// </summary>
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

		/// <summary>
		/// The current code language
		/// </summary>
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

		/// <summary>
		/// Tries to close this tab, prompting the user to save any changes
		/// </summary>
		/// <returns><c>true</c> on success, <c>false</c> on failure or cancel</returns>
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

	/// <summary>
	/// Stores references to common dialog boxes
	/// </summary>
	public static class DialogCache
	{
		static OpenFileDialog openFile;
		static SaveFileDialog saveFile;

		/// <summary>
		/// An open file dialog box
		/// </summary>
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

		/// <summary>
		/// A save file dialog box
		/// </summary>
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
