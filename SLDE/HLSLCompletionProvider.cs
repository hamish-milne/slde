using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;
using System.Windows.Forms;
using SLDE.Properties;

namespace SLDE.Completion
{
	public interface IDataList : ICollection<CompletionData>
	{
		CompletionData this[Substring name] { get; }
		void AddRange(IDataList list);
		CompletionData[] ToArray();
	}

	public class DataList : IDataList
	{
		class Enumerator : IEnumerator<CompletionData>
		{
			IEnumerator<KeyValuePair<Substring, CompletionData>> internalEnumerator;

			public CompletionData Current
			{
				get { return internalEnumerator.Current.Value; }
			}

			object IEnumerator.Current
			{
				get { return internalEnumerator.Current.Value; }
			}

			public bool MoveNext()
			{
				return internalEnumerator.MoveNext();
			}

			public void Dispose()
			{
				internalEnumerator.Dispose();
			}

			public void Reset()
			{
				internalEnumerator.Reset();
			}

			public Enumerator(IEnumerator<KeyValuePair<Substring, CompletionData>> internalEnumerator)
			{
				this.internalEnumerator = internalEnumerator;
			}
		}

		Dictionary<Substring, CompletionData> dict
			= new Dictionary<Substring, CompletionData>();

		public int Count
		{
			get { return dict.Count; }
		}

		public CompletionData this[Substring name]
		{
			get
			{
				CompletionData ret;
				dict.TryGetValue(name, out ret);
				return ret;
			}
		}

		public void Add(CompletionData item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			dict.Add(item.Text, item);
		}

		public void AddRange(IDataList list)
		{
			if (list == null)
				return;
			foreach (var item in list)
				Add(item);
		}

		public IEnumerator<CompletionData> GetEnumerator()
		{
			return new Enumerator(dict.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public CompletionData[] ToArray()
		{
			var values = dict.Values;
			var array = new CompletionData[values.Count];
			values.CopyTo(array, 0);
			return array;
		}

		public void Clear()
		{
			dict.Clear();
		}

		public bool Contains(CompletionData item)
		{
			if (item == null)
				return false;
			return dict.ContainsKey(item.Text);
		}

		public void CopyTo(CompletionData[] array, int arrayIndex)
		{
			dict.Values.CopyTo(array, arrayIndex);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(CompletionData item)
		{
			if (item == null)
				return false;
			return dict.Remove(item.Text);
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

		public char this[int index]
		{
			get { return source[start + index]; }
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

		public static bool operator ==(Substring a, Substring b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Substring a, Substring b)
		{
			return !a.Equals(b);
		}
	}

	public class CompletionData : ICompletionData
	{
		protected Substring text;
		protected string description;
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
			get { return 0; }
		}

		public int Version
		{
			get { return version; }
		}

		public virtual IDataList DataItems
		{
			get { return null; }
		}

		public virtual IDataList GetValidData(Stack<CompletionData> stack)
		{
			return DataItems;
		}

		public virtual bool InsertAction(TextArea textArea, char insertChar)
		{
			textArea.InsertString(Text.ToString());
			return false;
		}

		public CompletionData(Substring text, string description)
		{
			this.text = text;
			this.description = description;
		}

		protected static string GetPrefix(CompletionData data, string append)
		{
			return (data == null ? "" : data.Text + append);
		}

		public virtual void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (DataItems == null)
				stack.Pop();
			else
			{
				var data = DataItems[item];
				if (data != null)
					stack.Push(data);
			}
		}

		protected CompletionData parent;

		public virtual IDataList Members { get { return null; } }
		public virtual CompletionData Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		protected static IDataList GetValidDataRecursive(Stack<CompletionData> stack, CompletionData scope, ref bool recursionLock, ref IDataList validData)
		{
			if (recursionLock)
				return null;
			recursionLock = true;
			if (validData == null)
				validData = new DataList();
			validData.Clear();
			if(scope.DataItems != null)
				validData.AddRange(scope.DataItems);
			if(scope.Members != null)
				validData.AddRange(scope.Members);
			var parentData = scope.Parent == null ? null : scope.Parent.GetValidData(stack);
			if (parentData != null)
				validData.AddRange(parentData);
			recursionLock = false;
			return validData;
		}
	}
}

namespace SLDE.HLSL.Completion
{
	using SLDE.Completion;

	public class HLSLKeyword : CompletionData
	{
		protected IDataList validData;

		public override IDataList DataItems
		{
			get
			{
				if (validData == null)
					validData = new DataList();
				return validData;
			}
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			return DataItems;
		}

		public HLSLKeyword(Substring name, string description)
			: base(name, description)
		{
		}
	}

	public class HLSLMember : CompletionData
	{
		protected HLSLType type;
		protected List<HLSLKeyword> keywords;
		bool hasSemantic, hasParameters, isFunction;
		HLSLSemantic parsedSemantic;
		IDataList members;

		public override int ImageIndex
		{
			get
			{
				return 4;
			}
		}

		public override IDataList Members
		{
			get
			{
				if (members == null)
					members = new DataList();
				return members;
			}
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

		public override CompletionData Parent
		{
			get
			{
				return base.Parent;
			}
			set
			{
				base.Parent = value;
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

		public virtual IList<HLSLKeyword> Keywords
		{
			get
			{
				if (keywords == null)
					keywords = new List<HLSLKeyword>();
				return keywords;
			}
		}

		public override string Description
		{
			get
			{
				if (description == null)
				{
					if (Parent == null || !(Parent is HLSLFunction))
						description = GetPrefix(Type, " ") + GetPrefix(Parent, ".") + Name;
					else
						description = GetPrefix(Type, " ") + Name;
				}
				return description;
			}
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			if (hasSemantic && Type != null)
				return Type.Semantics;
			if (hasParameters)
				return Parent.GetValidData(stack);
			return null;
		}

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (item.Length < 1)
				return;
			var c = item[0];

			switch(c)
			{
				case ':':
					hasSemantic = true;
					break;
				case '(':
					hasParameters = true;
					isFunction = true;
					break;
				case ')':
				case ',':
				case ';':
				case '{':
					if(c == ')')
					{
						hasParameters = false;
						if (isFunction)
							return;
					}
					if (isFunction && c == ',')
						return;
					stack.Pop();
					if (!isFunction && c == '{')
					{
						stack.Push(new HLSLScope("", stack.Peek()));
						return;
					}
					if (stack.Peek() == null || stack.Peek().Members == null)
						return;
					CompletionData dataItem;
					if(isFunction)
					{
						var func = new HLSLFunction(Name, stack.Peek());
						func.Arguments.AddRange(Members);
						func.ReturnType = Type;
						func.Semantic = parsedSemantic;
						dataItem = func;
					} else
					{
						var variable = new HLSLVariable(Name, Type, stack.Peek());
						variable.Semantic = parsedSemantic;
						dataItem = variable;
					}
					stack.Peek().Members.Add(dataItem);
					if (c == ',' && !(stack.Peek() is HLSLMember))
						stack.Push(Type);
					else if (c == ')' && stack.Peek() is HLSLMember)
						stack.Peek().Parse(item, stack);
					else if (c == '{' && isFunction)
						stack.Push(dataItem);
					break;
				default:
					if (!CompletionUtility.Operators.Contains(c))
					{
						if (hasParameters && Parent != null)
						{
							stack.Push(Parent.GetValidData(stack)[item]);
						}
						else if (hasSemantic)
						{
							hasSemantic = false;
							if (Type != null)
								parsedSemantic = Type.Semantics[item] as HLSLSemantic;
							if (parsedSemantic == null)
								parsedSemantic = new HLSLSemantic(item);
						}
					}
					break;
			}
		}

		public HLSLMember(Substring name, HLSLType type, CompletionData parent)
			: base(name, null)
		{
			this.type = type;
			this.parent = parent;
		}
	}

	/*public class HLSLTypedef : HLSLType
	{
		HLSLType reference;

		public virtual HLSLType Reference
		{
			get { return reference; }
			set { reference = value; }
		}

		public override IDataList DataItems
		{
			get
			{
				return reference == null ? null : reference.DataItems;
			}
		}

		public override IDataList Members
		{
			get
			{
				return reference == null ? null : reference.Members;
			}
		}

		public override Substring Name
		{
			get
			{
				return reference == null ? "" : reference.Name;
			}
			set
			{
				if (reference != null)
					reference.Name = value;
			}
		}
	}*/

	public class HLSLType : CompletionData
	{
		protected IDataList members;
		bool recursionLock;
		IDataList validData, semantics;
		bool open, closed;
		Substring typeType;

		// TODO: Typedefs and templates

		public override int ImageIndex
		{
			get
			{
				return 1;
			}
		}

		public virtual Substring TypeType
		{
			get { return typeType; }
			set { typeType = value; }
		}

		public virtual bool Open
		{
			get { return open; }
		}

		public virtual bool Closed
		{
			get { return closed; }
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

		public virtual IDataList Semantics
		{
			get
			{
				if (semantics == null)
					semantics = new DataList();
				return semantics;
			}
		}

		public override CompletionData Parent
		{
			get { return parent; }
			set
			{
				parent = value;
				description = null;
			}
		}

		public override IDataList Members
		{
			get
			{
				if(members == null)
					members = new DataList();
				return members;
			}
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			if (!Open || Closed)
				return null;
			return GetValidDataRecursive(stack, this, ref recursionLock, ref validData);
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

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if(item.Length == 0)
				return;
			var c = item[0];
			if(!Open && !Closed)
			{
				if(CompletionUtility.Operators.Contains(c))
				{
					if (c == '{')
						open = true;
					else
						stack.Pop();
				} else
					Name = item;
				return;
			} else if(Open && !Closed)
			{
				if(c ==  '}')
				{
					closed = true;
					stack.Pop();
					return;
				}
				var data = GetValidData(stack);
				var push = data == null ? null : data[item];
				if(push != null)
					stack.Push(push);
			} else // Closed
			{
				stack.Pop();
				stack.Push(new HLSLMember(item, this, stack.Peek()));
			}
		}

		public HLSLType(Substring typeType, Substring name, CompletionData parent)
			: base(name, null)
		{
			this.typeType = typeType;
			this.parent = parent;
		}
	}

	public class HLSLPrimitive : HLSLType
	{
		protected IDataList dataItems;

		public override IDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new DataList();
				return dataItems;
			}
		}

		public override string Description
		{
			get
			{
				return description;
			}
		}

		public override bool Open
		{
			get
			{
				return true;
			}
		}

		public override bool Closed
		{
			get
			{
				return true;
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

		public override CompletionData Parent
		{
			get { return parent; }
			set
			{
				parent = value;
			}
		}

		public HLSLPrimitive(Substring name, string description, CompletionData parent)
			: base("", name, parent)
		{
			this.description = description;
			Members.Add(new CompletionData("wxyz", ""));
			Members.Add(new CompletionData("x", ""));
			Members.Add(new CompletionData("y", ""));
			Members.Add(new CompletionData("z", ""));
			Members.Add(new CompletionData("w", ""));
		}
	}

	public class HLSLVariable : HLSLMember
	{
		protected HLSLSemantic semantic;
		protected bool hasDot;

		public virtual HLSLSemantic Semantic
		{
			get { return semantic; }
			set
			{
				semantic = value;
				description = null;
			}
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			if (hasDot && Type != null)
			{
				hasDot = false;
				return Type.Members;
			}
			return base.GetValidData(stack);
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

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (item.Length < 1)
				return;
			if (item[0] == '.')
				hasDot = true;
			else if(hasDot && Type != null && Type.Members != null)
			{
				var member = Type.Members[item];
				stack.Pop();
				if (member != null)
					stack.Push(member);
			} else
				stack.Pop();
		}

		public HLSLVariable(Substring name, HLSLType type, CompletionData parent)
			: base(name, type, parent)
		{
		}
	}

	public class HLSLSemantic : CompletionData
	{
		public HLSLSemantic(Substring name)
			: base(name, name.ToString())
		{
		}

		public static string GetSuffix(HLSLSemantic semantic)
		{
			return (semantic == null ? "" : " : " + semantic.Text);
		}
	}

	public class HLSLScope : CompletionData
	{
		protected IDataList members;
		protected IDataList validData;
		bool recursionLock;

		public override IDataList Members
		{
			get
			{
				if (members == null)
					members = new DataList();
				return members;
			}
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			return GetValidDataRecursive(stack, this, ref recursionLock, ref validData);
		}

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (item.Length < 1)
				return;
			var c = item[0];
			if (c == '{')
				stack.Push(new HLSLScope("", this));
			else if (c == '}')
				stack.Pop();
			else
			{
				var allData = GetValidData(stack);
				var dataItem = allData[item];
				if (dataItem != null)
					stack.Push(dataItem);
			}
		}

		public HLSLScope(Substring name, CompletionData parent)
			: base(name, null)
		{
			this.parent = parent;
		}
	}

	public class HLSLFunction : HLSLScope
	{
		IDataList arguments;
		HLSLType returnType;
		IDataList dataItems;
		bool recursionLock;
		HLSLSemantic semantic;

		public virtual IDataList Arguments
		{
			get
			{
				if (arguments == null)
					arguments = new DataList();
				return arguments;
			}
		}

		public override int ImageIndex
		{
			get
			{
				return 5;
			}
		}

		public override IDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new DataList();
				return dataItems;
			}
		}

		public virtual HLSLSemantic Semantic
		{
			get { return semantic; }
			set { semantic = value; }
		}

		public override string Description
		{
			get
			{
				var ret = ReturnType.Text + " " + Text + "(";
				bool first = true;
				if(Arguments != null)
					foreach(var arg in Arguments)
					{
						if (first)
							first = false;
						else
							ret += ", ";
						ret += arg.Description;
					}
				ret += ")";
				return ret;
			}
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			GetValidDataRecursive(stack, this, ref recursionLock, ref validData);
			validData.AddRange(Arguments);
			return validData;
		}

		public virtual HLSLType ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public HLSLFunction(Substring name, CompletionData parent)
			: base(name, parent)
		{
		}
	}
	
	public class HLSLRootScope : HLSLScope
	{
		protected IDataList dataItems;
		protected HashSet<Substring> typeTypes
			= new HashSet<Substring>() { "struct", "class", "interface" };
		protected IDataList rootDataItems;

		public override IDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new DataList();
				return dataItems;
			}
		}

		public virtual IDataList RootDataItems
		{
			get
			{
				if (rootDataItems == null)
					rootDataItems = new DataList();
				return rootDataItems;
			}
		}

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (typeTypes.Contains(item))
			{
				var newType = new HLSLType(item, null, this);
				Members.Add(newType);
				stack.Push(newType);
			}
			else
				base.Parse(item, stack);
		}

		public override IDataList GetValidData(Stack<CompletionData> stack)
		{
			if (validData == null)
				validData = new DataList();
			else
				validData.Clear();
			validData.AddRange(DataItems);
			validData.AddRange(Members);
			if (stack.Peek() == this)
				validData.AddRange(RootDataItems);
			return validData;
		}

		public HLSLRootScope() : base("", null)
		{
		}
	}

	public static class CompletionUtility
	{
		public static readonly HashSet<char> Operators =
			new HashSet<char> { '.', ':', ';', '<', '>', '@',
				'[', ']', '{', '}', '(', ')', '#', '\'', '"',
				'+', '-',  '&', '|', '~', '^', '$', '*', '/',
				'\\', '?', '!' , ',' };
	}

	public class HLSLCompletionProvider : AbstractCompletionDataProvider
	{
		

		static Dictionary<char, string> operatorCache
			= new Dictionary<char, string>();

		HLSLRootScope root;
		HLSLPrimitive Bool, Int, UInt, DWord, Half, Float, Double;
		HLSLPrimitive Min16float, Min10float, Min16int, Min12int, Min16uint;
		HLSLKeyword snorm, unorm, Struct, Class, Interface;

		ImageList imageList;
		Stack<CompletionData> stack = new Stack<CompletionData>();

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
			Min16float  = new HLSLPrimitive("min16float", "Minimum 16-bit floating point value", root);
			Min10float  = new HLSLPrimitive("min10float", "Minimum 10-bit floating point value", root);
			Min16int    = new HLSLPrimitive("min16int", "Minimum 16-bit signed integer", root);
			Min12int    = new HLSLPrimitive("min12int", "Minimum 12-bit signed integer", root);
			Min16uint   = new HLSLPrimitive("min16uint", "Minimum 16-bit unsigned integer", root);

			snorm       = new HLSLKeyword("snorm", "Normalize float in range -1 to 1");
			unorm       = new HLSLKeyword("unorm", "Normalize float in range 0 to 1");
			Struct      = new HLSLKeyword("struct", "A structure. Groups data together");
			Class       = new HLSLKeyword("class", "Groups data and functions together");
			Interface   = new HLSLKeyword("interface", "An interface to another type");

			snorm.DataItems.Add(Float);
			snorm.DataItems.Add(Min10float);
			snorm.DataItems.Add(Min16float);
			unorm.DataItems.Add(Float);
			unorm.DataItems.Add(Min10float);
			unorm.DataItems.Add(Min16float);

			Float.Semantics.Add(new HLSLSemantic("COLOR0"));
			Float.Semantics.Add(new HLSLSemantic("COLOR1"));
			Int.Semantics.Add(new HLSLSemantic("MySemantic"));

			root.DataItems.Add(Bool);
			root.DataItems.Add(Int);
			root.DataItems.Add(UInt);
			root.DataItems.Add(DWord);
			root.DataItems.Add(Half);
			root.DataItems.Add(Float);
			root.DataItems.Add(Double);
			root.DataItems.Add(Min16float);
			root.DataItems.Add(Min10float);
			root.DataItems.Add(Min16int);
			root.DataItems.Add(Min12int);
			root.DataItems.Add(Min16uint);

			root.DataItems.Add(snorm);
			root.DataItems.Add(unorm);
			root.RootDataItems.Add(Struct);
			root.RootDataItems.Add(Class);
			root.RootDataItems.Add(Interface);

			imageList = new ImageList();
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			imageList.Images.Add(Resources.CodeLines);
			imageList.Images.Add(Resources.Structure);
			imageList.Images.Add(Resources.Class);
			imageList.Images.Add(Resources.Interface);
			imageList.Images.Add(Resources.Field);
			imageList.Images.Add(Resources.Method);
		}

		void ParseItem(string item)
		{
			if (stack.Count < 1)
				stack.Push(root);
			else if(stack.Contains(null))
			{
				var array = stack.ToArray();
				stack.Clear();
				for (int i = 0; i < array.Length; i++)
					if (array[i] != null)
						stack.Push(array[i]);
			}
			stack.Peek().Parse(item, stack);
		}

		public override ICompletionData[] GenerateCompletionData(string fileName,
			TextArea textArea, char charTyped)
		{
			stack.Clear();
			stack.Push(root);
			root.Members.Clear();
			var text = textArea.Document.TextBufferStrategy;
			var caretPos = textArea.Caret.Offset;
			for(int pos = 0; pos < caretPos; pos++)
			{
				var c = text.GetCharAt(pos);
				if (Char.IsWhiteSpace(c) || Char.IsControl(c))
					continue;
				if(CompletionUtility.Operators.Contains(c))
				{
					if (c == '/' && pos < (caretPos - 1))
					{
						// Single-line comment;
						if (text.GetCharAt(++pos) == '/')
						{
							do
							{
								if (pos >= (caretPos - 1))
									return null;
								c = text.GetCharAt(++pos);
							} while(c != '\n' && c != '\r');
							if (c == '\r' && pos < caretPos
								&& text.GetCharAt(pos + 1) == '\n')
								pos++;
							continue;
						}
						// Multi-line comment
						if (text.GetCharAt(pos) == '*')
						{
							do
							{
								if (pos >= (caretPos - 1))
									return null;
								c = text.GetCharAt(++pos);
							} while (c != '/' || text.GetCharAt(pos - 1) != '*');
							continue;
						}
					}
					// Strings (including escape characters)
					if(c == '"')
					{
						do
						{
							if (pos >= (caretPos - 1))
								return null;
							c = text.GetCharAt(++pos);
							if (c == '\\')
								pos++;
						} while (c != '"');
						continue;
					}
					// Now get the operator itself
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
				// If it's not an operator, it's some sort of identifier
				int start = pos;
				while (++pos < caretPos)
				{
					c = text.GetCharAt(pos);
					if(Char.IsWhiteSpace(c) || Char.IsControl(c)
						|| CompletionUtility.Operators.Contains(c))
					{
						var length = pos - start;
						ParseItem(text.GetText(start, length));
						pos--;
						break;
					}
				}
			}
			if (stack.Count < 1 || stack.Peek() == null)
				return null;
			var data = stack.Peek().GetValidData(stack);
			return data == null ? null : data.ToArray();
		}

		
	}
}
