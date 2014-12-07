using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Folding;

namespace SLDE
{
	public class Language
	{
		public virtual IHighlightingStrategy HighlightingStrategy { get; set; }
		public virtual IFoldingStrategy FoldingStrategy { get; set; }
	}

	
}
