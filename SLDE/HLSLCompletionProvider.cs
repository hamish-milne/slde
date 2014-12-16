using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;
using System.Windows.Forms;

namespace SLDE.HLSL.Completion
{
	public class CompletionDataList : IList<HLSLCompletionData>
	{
		Dictionary<Substring, HLSLCompletionData> cache
			= new Dictionary<Substring, HLSLCompletionData>();
		List<HLSLCompletionData> list
			= new List<HLSLCompletionData>();

		public HLSLCompletionData this[int index]
		{
			get { return list[index]; }
			set { list[index] = value; }
		}

		public HLSLCompletionData this[Substring text]
		{
			get
			{
				HLSLCompletionData ret;
				cache.TryGetValue(text, out ret);
				return ret;
			}
		}

		public void Add(HLSLCompletionData item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			cache[item.Text] = item;
			list.Add(item);
		}

		public bool Remove(HLSLCompletionData item)
		{
			if(list.Remove(item))
			{
				cache.Remove(item.Text);
				return true;
			}
			return false;
		}

		public bool Contains(HLSLCompletionData item)
		{
			return list.Contains(item);
		}

		public int Count
		{
			get { return list.Count; }
		}

		public void CopyTo(HLSLCompletionData[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public int IndexOf(HLSLCompletionData item)
		{
			return list.IndexOf(item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Insert(int index, HLSLCompletionData item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			list.Insert(index, item);
			cache[item.Text] = item;
		}

		public void RemoveAt(int index)
		{
			cache.Remove(list[index].Text);
			list.RemoveAt(index);
		}

		public void Clear()
		{
			cache.Clear();
			list.Clear();
		}

		public void AddRange(IList<HLSLCompletionData> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			for (int i = 0; i < list.Count; i++)
				if(list[i] != null)
					Add(list[i]);
		}

		public IEnumerator<HLSLCompletionData> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

	}

	public struct Substring : IEquatable<Substring>
	{
		string source;
		int start;
		int length;

		public Substring(string source, int start, int length)
		{
			if (source == null || start < 0 || length < 0 
				|| (start + length) > source.Length)
				throw new ArgumentException("Invalid substring arguments");
			this.source = source;
			this.start = start;
			this.length = length;
		}

		public int Length
		{
			get { return length; }
		}

		public override string ToString()
		{
			if (source == null)
				return "";
			return source.Substring(start, length);
		}

		public override int GetHashCode()
		{
			int length = this.length;
			int hash = 5381;
			while (length-- > 0)
				hash = ((hash << 5) + hash) ^ (int)source[length];
			return hash;
		}

		public bool Equals(Substring other)
		{
			if (other.length != this.length)
				return false;
			int length = this.length;
			while (length-- > 0)
				if (source[start + length] != other.source[other.start + length])
					return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Substring))
				return false;
			return Equals((Substring)obj);
		}

		public static implicit operator Substring(string str)
		{
			if (str == null) str = "";
			return new Substring(str, 0, str.Length);
		}

		public static explicit operator string(Substring str)
		{
			return str.ToString();
		}
	}

	public static class Operators
	{
		public static readonly Substring Semicolon = ";";
		public static readonly Substring OpenBrace = "{";
		public static readonly Substring CloseBrace = "}";
	}

	public class HLSLCompletionData : ICompletionData
	{
		protected Substring text;
		protected string description;
		protected int imageIndex;
		protected double priority = 0;
		protected int version;

		public virtual bool IsCompatible
		{
			get { return true; }
		}

		string ICompletionData.Text
		{
			get { return Text.ToString(); }
		}

		public virtual Substring Text
		{
			get { return text; }
		}
		
		public virtual string Description
		{
			get { return description; }
		}

		public virtual double Priority
		{
			get { return priority; }
		}

		public virtual int ImageIndex
		{
			get { return imageIndex; }
		}

		public int Version
		{
			get { return version; }
		}

		public virtual IList<HLSLCompletionData> DataItems
		{
			get { return null; }
		}

		public virtual IList<HLSLCompletionData> GetValidData()
		{
			return DataItems;
		}

		public virtual bool InsertAction(TextArea textArea, char insertChar)
		{
			textArea.InsertString(Text.ToString());
			return false;
		}

		public HLSLCompletionData(Substring text, string description, int imageIndex, int version = 0)
		{
			this.version = version;
			this.text = text;
			this.description = description;
			this.imageIndex = imageIndex;
		}

		protected static string GetPrefix(HLSLCompletionData data, string append)
		{
			return (data == null ? "" : data.Text + append);
		}

		public virtual void Parse(Substring item, out HLSLCompletionData newDefinition)
		{
			newDefinition = null;
		}

		protected static IList<HLSLCompletionData> GetValidDataRecursive(IScope scope, ref bool recursionLock, ref CompletionDataList validData)
		{
			if (recursionLock)
				return null;
			recursionLock = true;
			if (validData == null)
				validData = new CompletionDataList();
			validData.Clear();
			validData.AddRange(scope.Members);
			var parentData = scope.Parent == null ? null : scope.Parent.DataItems;
			if (parentData != null)
				validData.AddRange(parentData);
			recursionLock = false;
			return validData;
		}
	}

	public class HLSLKeyword : HLSLCompletionData
	{
		protected IList<HLSLCompletionData> validData;

		public override IList<HLSLCompletionData> DataItems
		{
			get
			{
				if (validData == null)
					validData = new List<HLSLCompletionData>();
				return validData;
			}
		}

		public HLSLKeyword(Substring name, string description, int imageIndex, int version = 0)
			: base(name, description, imageIndex, version)
		{
		}
	}

	public class HLSLMember : HLSLCompletionData
	{
		protected HLSLType type;
		protected HLSLType parent;

		public virtual Substring Name
		{
			get { return text; }
			set
			{
				text = value;
				description = null;
			}
		}

		public virtual HLSLType Parent
		{
			get { return parent; }
			set
			{
				parent = value;
				description = null;
			}
		}

		public virtual HLSLType Type
		{
			get { return type; }
			set
			{
				type = value;
				description = null;
			}
		}

		public override string Description
		{
			get
			{
				if (description == null)
					description = GetPrefix(Type, " ") + GetPrefix(Parent, ".") + Name;
				return description;
			}
		}

		public HLSLMember(Substring name, HLSLType type, HLSLType parent, int imageIndex, int version = 0)
			: base(name, null, imageIndex, version)
		{
		}
	}

	public class HLSLType : HLSLCompletionData, IScope
	{
		protected IList<HLSLCompletionData> members;
		protected IScope parent;
		bool recursionLock;
		CompletionDataList validData;

		// TODO: Typedefs and templates

		public virtual Substring TypeType
		{
			get { return null; }
		}

		public virtual Substring Name
		{
			get { return text; }
			set
			{
				text = value;
				description = null;
			}
		}

		public virtual IScope Parent
		{
			get { return parent; }
			set
			{
				parent = value;
				description = null;
			}
		}

		/*IScope IScope.Parent
		{
			get { return Parent; }
		}*/

		public virtual IList<HLSLCompletionData> Members
		{
			get
			{
				if(members == null)
					members = new List<HLSLCompletionData>();
				return members;
			}
		}

		public override IList<HLSLCompletionData> GetValidData()
		{
			return GetValidDataRecursive(this, ref recursionLock, ref validData);
		}

		public override string Description
		{
			get
			{
				if (description == null)
					description = (TypeType.Length == 0 ? "" : TypeType + " ") + Name;
				return description;
			}
		}

		public HLSLType(Substring name, IScope parent, int imageIndex, int version = 0)
			: base(name, null, imageIndex, version)
		{
			this.parent = parent;
		}
	}

	public class HLSLPrimitive : HLSLType
	{

		public override string Description
		{
			get
			{
				return description;
			}
		}

		public override Substring Name
		{
			get { return text; }
			set
			{
				text = value;
			}
		}

		public override IScope Parent
		{
			get { return parent; }
			set
			{
				parent = value;
			}
		}

		public HLSLPrimitive(Substring name, string description, IScope parent, int version = 0)
			: base(name, parent, 1, version)
		{
			this.description = description;
		}
	}

	public class HLSLStruct : HLSLType
	{
		public override Substring TypeType
		{
			get { return "struct"; }
		}

		public HLSLStruct(string name, HLSLType parent, int imageIndex, int version = 0)
			: base(name, parent, imageIndex, version)
		{
		}
	}

	public class HLSLClass : HLSLType
	{
		public override Substring TypeType
		{
			get { return "class"; }
		}

		public HLSLClass(string name, HLSLType parent, int imageIndex, int version = 0)
			: base(name, parent, imageIndex, version)
		{
		}
	}

	public class HLSLInterface : HLSLType
	{
		public override Substring TypeType
		{
			get { return "interface"; }
		}

		public HLSLInterface(string name, HLSLType parent, int imageIndex, int version = 0)
			: base(name, parent, imageIndex, version)
		{
		}
	}

	public class HLSLVariable : HLSLMember
	{
		protected HLSLSemantic semantic;

		public virtual HLSLSemantic Semantic
		{
			get { return semantic; }
			set
			{
				semantic = value;
				description = null;
			}
		}

		public override string Description
		{
			get
			{
				if (description == null)
					description = base.Description + HLSLSemantic.GetSuffix(Semantic);
				return description;
			}
		}

		public HLSLVariable(string name, HLSLType type, HLSLType parent, int imageIndex, int version = 0)
			: base(name, type, parent, imageIndex, version)
		{
		}
	}

	public class HLSLLocalVariable : HLSLVariable
	{
		public override string Description
		{
			get
			{
				if (description == null)
					description = Name.ToString();
				return description;
			}
		}

		public HLSLLocalVariable(string name, HLSLType type, HLSLType parent, int imageIndex, int version = 0)
			: base(name, type, parent, imageIndex, version)
		{
		}
	}

	public class HLSLSemantic : HLSLCompletionData
	{
		protected IList<HLSLType> validTypes;

		public virtual IList<HLSLType> ValidTypes
		{
			get
			{
				if (validTypes == null)
					validTypes = new List<HLSLType>();
				return validTypes;
			}
		}

		public HLSLSemantic(string name, int imageIndex, int version = 0)
			: base(name, name, imageIndex, version)
		{
		}

		public static string GetSuffix(HLSLSemantic semantic)
		{
			return (semantic == null ? "" : " : " + semantic.Text);
		}
	}

	public interface IScope
	{
		IList<HLSLCompletionData> Members { get; }
		IList<HLSLCompletionData> DataItems { get; }
		IScope Parent { get; }
	}

	public class HLSLScope : HLSLCompletionData, IScope
	{
		protected IList<HLSLCompletionData> members;
		protected IScope parent;
		protected CompletionDataList validData;
		bool recursionLock;

		public IScope Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public IList<HLSLCompletionData> Members
		{
			get
			{
				if (members == null)
					members = new List<HLSLCompletionData>();
				return members;
			}
		}


		public override IList<HLSLCompletionData> GetValidData()
		{
			return GetValidDataRecursive(this, ref recursionLock, ref validData);
		}

		public HLSLScope(string name, IScope parent, int imageIndex, int version = 0)
			: base(name, null, imageIndex, version)
		{
			this.parent = parent;
		}
	}

	public class HLSLArgument : HLSLLocalVariable
	{
		public override string Description
		{
			get
			{
				if (description == null)
					description = Name + HLSLSemantic.GetSuffix(Semantic);
				return description;
			}
		}

		public HLSLArgument(string name, HLSLType type, HLSLType parent, int imageIndex, int version = 0)
			: base(name, type, parent, imageIndex, version)
		{
		}
	}

	public class HLSLFunction : HLSLScope
	{
		public virtual IEnumerable<HLSLArgument> Arguments
		{
			get { return Members.OfType<HLSLArgument>(); }
		}

		public HLSLFunction(string name, IScope parent, int imageIndex, int version = 0)
			: base(name, parent, imageIndex, version)
		{
		}
	}
	
	public class HLSLRootScope : HLSLScope
	{

		public HLSLRootScope() : base("", null, -1, 0)
		{
		}
	}

	public class HLSLCompletionProvider : AbstractCompletionDataProvider
	{
		static HashSet<char> operators =
			new HashSet<char> { '.', ':', ';', '<', '>', '@',
				'[', ']', '{', '}', '(', ')', '#', '\'', '"',
				'+', '-',  '&', '|', '~', '^', '$', '*', '/', '\\', '?', '!' };

		static Dictionary<char, string> operatorCache
			= new Dictionary<char, string>();

		HLSLRootScope root;
		HLSLPrimitive Bool, Int, UInt, DWord, Half, Float, Double;
		HLSLPrimitive Min16float, Min10float, Min16int, Min12int, Min16uint;
		HLSLKeyword snorm, unorm, Struct, Class, Interface;

		HLSLCompletionData currentScope;
		ImageList imageList;

		public override ImageList ImageList
		{
			get { return imageList; }
		}

		public HLSLCompletionProvider()
		{
			root = new HLSLRootScope();
			Bool        = new HLSLPrimitive("bool", "true or false", root);
			Int         = new HLSLPrimitive("int", "32-bit signed integer", root);
			UInt        = new HLSLPrimitive("uint", "32-bit unsigned integer", root);
			DWord       = new HLSLPrimitive("dword", "32-bit unsigned integer", root);
			Half        = new HLSLPrimitive("half", "16-bit floating point value", root);
			Float       = new HLSLPrimitive("float", "32-bit floating point value", root);
			Double      = new HLSLPrimitive("double", "64-bit floating point value", root);
			Min16float  = new HLSLPrimitive("min16float", "Minimum 16-bit floating point value", root, 112);
			Min10float  = new HLSLPrimitive("min10float", "Minimum 10-bit floating point value", root, 112);
			Min16int    = new HLSLPrimitive("min16int", "Minimum 16-bit signed integer", root, 112);
			Min12int    = new HLSLPrimitive("min12int", "Minimum 12-bit signed integer", root, 112);
			Min16uint   = new HLSLPrimitive("min16uint", "Minimum 16-bit unsigned integer", root, 112);

			snorm       = new HLSLKeyword("snorm", "Normalize float in range -1 to 1", 2, 100);
			unorm       = new HLSLKeyword("unorm", "Normalize float in range 0 to 1", 2, 100);
			Struct      = new HLSLKeyword("struct", "A structure. Groups data together", 2, 0);
			Class       = new HLSLKeyword("class", "Groups data and functions together", 2, 0);
			Interface   = new HLSLKeyword("interface", "An interface to another type", 2, 0);

			snorm.DataItems.Add(Float);
			snorm.DataItems.Add(Min10float);
			snorm.DataItems.Add(Min16float);
			unorm.DataItems.Add(Float);
			unorm.DataItems.Add(Min10float);
			unorm.DataItems.Add(Min16float);

			root.Members.Add(Bool);
			root.Members.Add(Int);
			root.Members.Add(UInt);
			root.Members.Add(DWord);
			root.Members.Add(Half);
			root.Members.Add(Float);
			root.Members.Add(Double);
			root.Members.Add(Min16float);
			root.Members.Add(Min10float);
			root.Members.Add(Min16int);
			root.Members.Add(Min12int);
			root.Members.Add(Min16uint);

			root.Members.Add(snorm);
			root.Members.Add(unorm);

			currentScope = root;

			imageList = new ImageList();

		}


		// Image list:
		// 0: Not compatible
		// 1: Intrinsic type
		// 2: Keyword
		// 3: Semantic
		// 

		void ParseItem(string item)
		{

		}

		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			var text = textArea.Document.TextBufferStrategy;
			for(int pos = 0; pos < text.Length; pos++)
			{
				var c = text.GetCharAt(pos);
				if (Char.IsWhiteSpace(c) || Char.IsControl(c))
					continue;
				if(operators.Contains(c))
				{
					if(c == '/' && pos < (text.Length - 1))
					{
						// Single-line comment;
						if (text.GetCharAt(++pos) == '/')
						{
							do
							{
								if(pos >= (text.Length - 1))
									return null;
								c = text.GetCharAt(++pos);
							} while(c != '\n' && c != '\r');
							if (c == '\r' && pos < text.Length && text.GetCharAt(pos + 1) == '\n')
								pos++;
							continue;
						}
						// Multi-line comment
						if (text.GetCharAt(pos) == '*')
						{
							do
							{
								if (pos >= (text.Length - 1))
									return null;
								c = text.GetCharAt(++pos);
							} while (c != '/' || text.GetCharAt(pos - 1) != '*');
							continue;
						}
					}
					if(c == '"')
					{
						do
						{
							if (pos >= (text.Length - 1))
								return null;
							c = text.GetCharAt(++pos);
							if (c == '\\')
								pos++;
						} while (c != '"');
						continue;
					}
					string item;
					operatorCache.TryGetValue(c, out item);
					if(item == null)
					{
						item = c.ToString();
						operatorCache[c] = item;
					}
					ParseItem(item);
					continue;
				}

			}
			return currentScope == null ? null : currentScope.GetValidData().ToArray();
		}

		
	}
}
