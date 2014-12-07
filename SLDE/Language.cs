using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Completion;

namespace SLDE
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class LanguageMenu : ToolStripMenuItem
	{
		public LanguageMenu() : base()
		{
			DropDownItems.Add(Language.NoLanguage);
			Language.NoLanguage.Checked = true;
			Language.OnAddLanguage += (Language l) => { DropDownItems.Add(l); l.Click += language_Click; };
		}

		private void language_Click(object sender, EventArgs e)
		{
			SelectLanguage(sender as Language);
		}

		public virtual void SelectLanguage(Language l)
		{
			if (l == null)
				return;
			for (int i = 0; i < DropDownItems.Count; i++)
				if (DropDownItems[i] is ToolStripMenuItem)
					((ToolStripMenuItem)DropDownItems[i]).Checked = false;
			l.Checked = true;
		}
	}

	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class Language : ToolStripMenuItem
	{
		public static readonly Language NoLanguage = new Language("None");

		public static event Action<Language> OnAddLanguage;

		public static void AddLanguage(Language language)
		{
			if (OnAddLanguage != null)
				OnAddLanguage(language);
		}
		
		public virtual IList<string> Extensions { get; protected set; }
		public virtual IHighlightingStrategy HighlightingStrategy { get; set; }
		public virtual IFoldingStrategy FoldingStrategy { get; set; }
		public virtual ICompletionDataProvider CompletionData { get; set; }

		public Language(string name)
		{
			Name = name;
		}

		public override string Text
		{
			get { return Name; }
			set { base.Text = value; }
		}
		
	}

	
}
