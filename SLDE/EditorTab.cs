using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor;
using System.IO;

namespace SLDE
{
	public class EditorTab : TabPage
	{
		TextEditorControl textEditor;
		string fileName;

		public virtual TextEditorControl TextEditor
		{
			get { return textEditor; }
			protected set { textEditor = value; }
		}

		public virtual string FileName
		{
			get { return fileName; }
			protected set { fileName = value; }
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

		public virtual void TrySave()
		{
			if(FileName != null)
			{
				try
				{
					File.WriteAllText(FileName, textEditor.Text);
				} catch(Exception e)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		protected void SaveCallback(object sender, EventArgs args)
		{
			var dialog = (SaveFileDialog)sender;
			dialog.FileOk -= SaveCallback;
			FileName = dialog.FileName;
			TrySave();
		}

		public EditorTab() : base()
		{
			textEditor = new TextEditorControl();
			textEditor.Parent = this;
			textEditor.FillParent();
			textEditor.Anchor = Utility.AllAnchors;
			this.Text = "New tab";
		}

		public EditorTab(string fileName) : this()
		{
			FileName = fileName;
			this.Text = Path.GetFileName(fileName);
			textEditor.Text = File.ReadAllText(fileName);
		}
	}
}
