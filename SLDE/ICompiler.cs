using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLDE
{

	public enum ErrorColor
	{
		Red, Green, Blue
	}

	public struct TextMarker
	{
		public int StartLine;
		public int StartColumn;
		public int EndLine;
		public int EndColumn;
	}

	public struct ErrorMarker
	{
		public ErrorColor Color;
		public TextMarker Location;
		public string Message;
	}

	public struct AssemblyLine
	{
		public TextMarker Location;
		public string Instruction;
	}

	public interface ICompilerOutput
	{
		IList<AssemblyLine> AssemblyLines { get; }
		IList<ErrorMarker> Errors { get; }
	}

	public interface ICompiler
	{
		IDictionary<string, Stream> ProjectFiles { get; }
		string MainFile { get; set; }
		string EntryPoint { get; set; }
		bool HasEntryPoint { get; }
		IDictionary<string, string> Defines { get; }
		IList<string> IncludePaths { get; }
		IDictionary<string, int> Properties { get; }
		IList<PropertyName> PropertyNames { get; }
		ICompilerOutput Compile();
	}
}
