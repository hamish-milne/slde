using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLDE
{
	[AttributeUsage(AttributeTargets.Class)]
	public class LocaleAttribute : Attribute
	{
	}

	public abstract class Locale
	{
		public virtual string Name { get; protected set; }
		public virtual string New { get; protected set; }
		public virtual string NewFile { get; protected set; }
	}

	[Locale]
	public abstract class EnglishLocale : Locale
	{
		public EnglishLocale() : base()
		{
			Name = "English";
			New = "New";
			NewFile = "New file";
		}
	}
}
