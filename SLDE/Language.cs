using System;
using System.IO;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Completion;

namespace SLDE
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class Language : ToolStripMenuItem
	{
		public static ObservableCollection<Language> AllLanguages
			= new ObservableCollection<Language>();

		public static void FindAllLanguages()
		{
			AllLanguages.Clear();
			var types = Utility.AllTypes;
			for(int i = 0; i < types.Count; i++)
			{
				var t = types[i];
				if(t.IsSubclassOf(typeof(Language)) &&
					t.GetCustomAttributes(typeof(LanguageAttribute), false).Length > 0)
				{
					try
					{
						AllLanguages.Add((Language)Activator.CreateInstance(t));
					}
					catch
					{
						Utility.ShowError("Unable to create instance of " + t.FullName + ". Forgot a constructor?");
					}
				}
			}
		}

		public static Language GetByExtension(string extension)
		{
			for(int i = 0; i < AllLanguages.Count; i++)
			{
				var l = AllLanguages[i];
				if (l == null || l.Extensions == null)
					continue;
				if (l.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
					return AllLanguages[i];
			}
			return NoLanguage.Instance;
		}

		public static void SelectLanguage(Language l)
		{
			for (int i = 0; i < AllLanguages.Count; i++)
				if (AllLanguages[i] != null)
					AllLanguages[i].Checked = false;
			l.Checked = true;
		}

		public static string GetFilter()
		{
			string filter = "";
			for(int i = 0; i < AllLanguages.Count; i++)
			{
				var l = AllLanguages[i];
				if (l == null)
					continue;
				filter += l.Filter;
				if (i < AllLanguages.Count - 1)
					filter += "|";
			}
			return filter;
		}
		
		public virtual IList<string> Extensions { get; protected set; }
		public virtual IHighlightingStrategy HighlightingStrategy { get; set; }
		public virtual IFormattingStrategy FormattingStrategy { get; set; }
		public virtual IFoldingStrategy FoldingStrategy { get; set; }
		public virtual ICompletionDataProvider CompletionData { get; set; }
		public virtual ICompiler Compiler { get; set; }
		public virtual string Filter
		{
			get
			{
				var ret = Name + "|";
				if (Extensions != null)
					for (int i = 0; i < Extensions.Count; i++)
					{
						ret += "*" + Extensions[i];
						if (i < Extensions.Count - 1)
							ret += ",";
					}
				return ret;
			}
		}

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

	[AttributeUsage(AttributeTargets.Class)]
	public class LanguageAttribute : Attribute
	{
	}

	public class LanguageSelectEventArgs : EventArgs
	{
		public Language Language;
		public LanguageSelectEventArgs(Language language)
		{
			Language = language;
		}
	}

	public delegate void LanguageSelectEventHandler(object sender, LanguageSelectEventArgs e);

	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class LanguageMenu : ToolStripMenuItem
	{
		public LanguageMenu()
			: base()
		{
			Language.AllLanguages.CollectionChanged += AllLanguages_CollectionChanged;
		}

		protected override void Dispose(bool disposing)
		{
			Language.AllLanguages.CollectionChanged -= AllLanguages_CollectionChanged;
			base.Dispose(disposing);
		}

		public event LanguageSelectEventHandler OnSelectLanguage;

		public void RefreshLanguages()
		{
			for (int i = DropDownItems.Count - 1; i >= 0; i--)
				if (DropDownItems[i] is Language)
					DropDownItems.RemoveAt(i);
			for (int i = 0; i < Language.AllLanguages.Count; i++)
			{
				var l = Language.AllLanguages[i];
				if (l == null)
					continue;
				l.Click += language_Click;
				DropDownItems.Add(l);
			}
			for (int i = 0; i < DropDownItems.Count; i++)
			{
				var l = DropDownItems[i] as Language;
				if (l != null)
				{
					SelectLanguage(l);
					break;
				}
			}
		}

		public void SelectLanguage(Language language, bool visualOnly = false)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			for (int i = 0; i < DropDownItems.Count; i++)
			{
				var l = DropDownItems[i] as Language;
				if (l != null)
					l.Checked = false;
			}
			language.Checked = true;
			if (OnSelectLanguage != null && !visualOnly)
				OnSelectLanguage(this, new LanguageSelectEventArgs(language));
		}

		void AllLanguages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshLanguages();
		}

		private void language_Click(object sender, EventArgs e)
		{
			SelectLanguage(sender as Language);
		}
	}

	public struct PropertyName
	{
		public string Name;
		public Type Type;

		public PropertyName(string name, Type type)
		{
			Name = name;
			Type = type;
		}
	}

	[Language]
	public class NoLanguage : Language
	{
		public static NoLanguage Instance { get; private set; }

		public NoLanguage() : base("None")
		{
			Instance = this;
		}

		public override string Filter
		{
			get { return "All files|*.*"; }
		}
	}

	[Language]
	public class HLSL : Language
	{
		public HLSL() : base("HLSL")
		{
			Extensions = new string[] { ".fx", ".fxh", ".hlsl", ".compute", ".cginc" };
			HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(Name);
			FoldingStrategy = new HlslFoldingStrategy();
			FormattingStrategy = new HlslFormattingStrategy();
		}
	}

	[Language]
	public class GLSL : Language
	{
		public GLSL() : base("GLSL")
		{
			Extensions = new string[] { ".glsl", ".vert", ".frag", ".tesc", ".tese", ".geom", ".comp", ".glslv", ".glslf" };
		}
	}
	
}
