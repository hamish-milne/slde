using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Document;
using System.IO;

namespace SLDE
{
	public class EditorTab : TabPage
	{
		TextEditorControl textEditor;
		bool changed;

		public virtual TextEditorControl TextEditor
		{
			get { return textEditor; }
			protected set { textEditor = value; }
		}

		public virtual string FileName
		{
			get { return textEditor.FileName; }
			protected set
			{
				textEditor.FileName = value;
				this.Text = Path.GetFileName(value);
				Changed = Changed;
			}
		}

		public virtual void Save(SaveFileDialog saveFileDialog)
		{
			if(FileName == null)
			{
				saveFileDialog.FileOk += SaveCallback;
				saveFileDialog.ShowDialog();
			} else
			{
				TrySave();
			}
		}

		public virtual bool Changed
		{
			get { return changed; }
			set
			{
				changed = value;
				var lastChar = String.IsNullOrEmpty(Text) ? '\0' : Text[Text.Length - 1];
				if (value && lastChar != '*')
					Text += '*';
				else if (!value && lastChar == '*')
					Text = Text.Substring(0, Text.Length - 1);
			}
		}

		public virtual void TrySave()
		{
			if(FileName != null)
			{
				try
				{
					textEditor.SaveFile(FileName);
					Changed = false;
				} catch(Exception e)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		public virtual void OnTextChange(object sender, DocumentEventArgs e)
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

		public EditorTab() : base()
		{
			textEditor = new TextEditorControl();
			textEditor.DocumentChanged += OnTextChange;
			textEditor.Parent = this;
			textEditor.FillParent();
			textEditor.Anchor = Utility.AllAnchors;
			this.Text = "New file";
		}

		public EditorTab(string fileName) : this()
		{
			FileName = fileName;
			this.Text = Path.GetFileName(fileName);
			textEditor.Text = File.ReadAllText(fileName);
		}
	}
}
