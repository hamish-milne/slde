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
		static EditorTab activeTab;

		public static EditorTab ActiveEditorTab
		{
			get { return activeTab; }
			protected set
			{
				if(value != activeTab)
				{
					if (activeTab != null && activeTab.OnInactive != null)
						activeTab.OnInactive(activeTab, null);
					if (value != null && value.OnActive != null)
						value.OnActive(value, null);
					activeTab = value;
				}
			}
		}

		TextEditorControl textEditor;
		bool changed;
		TabControl oldParent;

		public event EventHandler OnActive;
		public event EventHandler OnInactive;

		public virtual TextEditorControl TextEditor
		{
			get { return textEditor; }
			protected set { textEditor = value; }
		}

		public virtual bool Active
		{
			get { return activeTab == this; }
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
				if (value == null || value.HighlightingStrategy == null)
					TextEditor.SetHighlighting("Default");
				else
					TextEditor.Document.HighlightingStrategy = value.HighlightingStrategy;
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

		public EditorTab() : base()
		{
			textEditor = new TextEditorControl();
			textEditor.DocumentChanged += OnTextChange;
			textEditor.Parent = this;
			textEditor.FillParent();
			textEditor.Anchor = Utility.AllAnchors;
			textEditor.Enter += textEditor_Enter;
			this.ParentChanged += EditorTab_ParentChanged;
			this.Text = "New file " + (GetHashCode()&0xF);
		}

		void textEditor_Enter(object sender, EventArgs e)
		{
			ActiveEditorTab = this;
		}

		void EditorTab_ParentChanged(object sender, EventArgs e)
		{
			if(oldParent != null)
			{
				oldParent.SelectedIndexChanged -= oldParent_SelectedIndexChanged;
				oldParent.GotFocus -= oldParent_SelectedIndexChanged;
			}
			oldParent = Parent as TabControl;
			if(oldParent != null)
			{
				oldParent.SelectedIndexChanged += oldParent_SelectedIndexChanged;
				oldParent.GotFocus += oldParent_SelectedIndexChanged;
			}
		}

		void oldParent_SelectedIndexChanged(object sender, EventArgs e)
		{
			var tabs = sender as TabControl;
			if (tabs == null)
				return;
			var editorTab = tabs.SelectedTab as EditorTab;
			if (editorTab != null)
				ActiveEditorTab = editorTab;
		}

		public EditorTab(string fileName) : this()
		{
			FileName = fileName;
			this.Text = Path.GetFileName(fileName);
			textEditor.Text = File.ReadAllText(fileName);
		}
	}
}
