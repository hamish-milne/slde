using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLDE
{
	/// <summary>
	/// Applied to a <see cref="Locale"/> class to indicate it should
	/// be added dynamically
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class LocaleAttribute : Attribute
	{
	}

	/// <summary>
	/// Stores all the UI strings for a particular locale (language)
	/// </summary>
	public abstract class Locale
	{
		public virtual string Name { get; protected set; }
		public virtual string New { get; protected set; }
		public virtual string NewFile { get; protected set; }
	}

	/// <summary>
	/// English locale
	/// </summary>
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
