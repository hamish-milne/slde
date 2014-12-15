using System;
using System.IO;
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
	/// <summary>
	/// A code language reference that includes syntax highlighting, code
	/// completion, folding, error highlighting and compiling
	/// </summary>
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class Language : ToolStripMenuItem
	{
		static List<Language> allLanguages
			= new List<Language>();
		static ReadOnlyCollection<Language> _allLanguages;

		static Language()
		{
			_allLanguages = allLanguages.AsReadOnly();
		}

		/// <summary>
		/// The list of available languages
		/// </summary>
		public static ReadOnlyCollection<Language> AllLanguages
		{
			get { return _allLanguages; }
		}

		/// <summary>
		/// Called when the list of languages is changed in any way
		/// </summary>
		public static event EventHandler LanguagesChanged;

		/// <summary>
		/// Clears the language list and adds in new instances of all valid classes
		/// </summary>
		/// <remarks>
		/// A valid class derives from <see cref="Language"/> and has the
		/// <see cref="LanguageAttribute"/> applied. An error is shown if
		/// an instance of any type cannot be created.
		/// </remarks>
		public static void FindAllLanguages()
		{
			allLanguages.Clear();
			allLanguages.Add(NoLanguage.Instance);
			allLanguages.AddRange(
				Utility.CreateListOf<Language, LanguageAttribute>(LanguageError)
				);
			if (LanguagesChanged != null)
				LanguagesChanged(null, null);
		}

		static void LanguageError(Type t)
		{
			Utility.ShowError("Unable to create instance of " + t.FullName +
				". Forgot a constructor?");
		}

		/// <summary>
		/// Adds a language to the list
		/// </summary>
		/// <param name="language">The language to add</param>
		/// <exception cref="ArgumentNullException"><paramref name="language"/>
		/// is <c>null</c></exception>
		public static void AddLanguage(Language language)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			allLanguages.Add(language);
			if (LanguagesChanged != null)
				LanguagesChanged(null, null);
		}

		/// <summary>
		/// Gets the appropriate language for the given extension
		/// </summary>
		/// <param name="extension">The file extension, including the dot</param>
		/// <returns>An appropriate language, or <see cref="NoLanguage"/></returns>
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

		/// <summary>
		/// Selects the given language, using it for the current editor tab
		/// </summary>
		/// <param name="language">The language to select</param>
		/// <exception cref="ArgumentNullException"><paramref name="language"/>
		/// is <c>null</c></exception>
		public static void SelectLanguage(Language language)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			for (int i = 0; i < AllLanguages.Count; i++)
				if (AllLanguages[i] != null)
					AllLanguages[i].Checked = false;
			language.Checked = true;
		}

		/// <summary>
		/// Gets an extension filter suitable for Windows Forms file dialog boxes
		/// </summary>
		/// <returns>The full filter as a string</returns>
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
		
		/// <summary>
		/// A list of file extensions that this language is valid for. Includes the dot
		/// </summary>
		public virtual IList<string> Extensions { get; protected set; }

		/// <summary>
		/// The highlighting strategy
		/// </summary>
		public virtual IHighlightingStrategy HighlightingStrategy { get; set; }

		/// <summary>
		/// The formatting strategy
		/// </summary>
		public virtual IFormattingStrategy FormattingStrategy { get; set; }

		/// <summary>
		/// The folding strategy
		/// </summary>
		public virtual IFoldingStrategy FoldingStrategy { get; set; }

		/// <summary>
		/// The completion data
		/// </summary>
		public virtual ICompletionDataProvider CompletionData { get; set; }

		/// <summary>
		/// The compiler
		/// </summary>
		public virtual ICompiler Compiler { get; set; }

		/// <summary>
		/// An extension filter for this language alone
		/// </summary>
		/// <remarks>
		/// Defaults to <c>[Name]|*[Extension],*[Extension 2]...</c>
		/// </remarks>
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

		/// <summary>
		/// Creates a new language with the given name
		/// </summary>
		/// <param name="name"></param>
		public Language(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Returns <see cref="Name"/>
		/// </summary>
		public override string Text
		{
			get { return Name; }
			set { base.Text = value; }
		}
	}

	/// <summary>
	/// Applied to a <see cref="Language"/> class to indicate it should
	/// be added dynamically
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class LanguageAttribute : Attribute
	{
	}

	/// <summary>
	/// The EventArgs for a language selection event
	/// </summary>
	public class LanguageSelectEventArgs : EventArgs
	{
		Language language;

		/// <summary>
		/// The language that was just selected
		/// </summary>
		public Language Language
		{
			get { return language; }
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="language"></param>
		public LanguageSelectEventArgs(Language language)
		{
			this.language = language;
		}
	}

	/// <summary>
	/// The event type for a language selection event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void LanguageSelectEventHandler(object sender, LanguageSelectEventArgs e);

	/// <summary>
	/// Used to select the language for the current editor tab
	/// </summary>
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip)]
	public class LanguageMenu : ToolStripMenuItem
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public LanguageMenu()
			: base()
		{
			Language.LanguagesChanged += LanguagesChanged;
		}

		/// <summary>
		/// Removes outstanding event handlers
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			Language.LanguagesChanged -= LanguagesChanged;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Called when a language is selected
		/// </summary>
		public event LanguageSelectEventHandler OnSelectLanguage;

		/// <summary>
		/// Updates the menu's internal list from the list of all languages
		/// </summary>
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

		/// <summary>
		/// Selects the given language in the menu
		/// </summary>
		/// <param name="language">The language to select</param>
		/// <param name="visualOnly">If <c>true</c>, the change is only
		/// a visual one, and no extra actions are performed</param>
		public void SelectLanguage(Language language, bool visualOnly = false)
		{
			if (language == null)
				language = NoLanguage.Instance;
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

		void LanguagesChanged(object sender, EventArgs e)
		{
			RefreshLanguages();
		}

		private void language_Click(object sender, EventArgs e)
		{
			SelectLanguage(sender as Language);
		}
	}

	/// <summary>
	/// No language; no special strategies
	/// </summary>
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

	/// <summary>
	/// High-level shader language
	/// </summary>
	[Language]
	public class HLSLLanguage : Language
	{
		static string[] HLSLExtensions = new string[] { ".fx", ".fxh", ".hlsl", ".compute", ".cginc" };

		public HLSLLanguage() : base("HLSL")
		{
			Extensions = HLSLExtensions;
			HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(Name);
			FoldingStrategy = new HlslFoldingStrategy();
			FormattingStrategy = new HlslFormattingStrategy();
		}
	}

	/// <summary>
	/// OpenGL shader language
	/// </summary>
	[Language]
	public class GLSL : Language
	{
		public GLSL() : base("GLSL")
		{
			Extensions = new string[] { ".glsl", ".vert", ".frag", ".tesc", ".tese", ".geom", ".comp", ".glslv", ".glslf" };
		}
	}

	/// <summary>
	/// Unity shader language
	/// </summary>
	[Language]
	public class ShaderLab : HLSLLanguage
	{
		public ShaderLab() : base()
		{
			Name = "ShaderLab";
			Extensions = new string[] { ".shader" };
		}
	}
	
}
