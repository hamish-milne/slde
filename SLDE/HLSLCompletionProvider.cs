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

		public HLSLCompletionData[] ToArray()
		{
			return list.ToArray();
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

		public virtual CompletionDataList DataItems
		{
			get { return null; }
		}

		public virtual CompletionDataList GetValidData()
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

		public virtual void Parse(Substring item, Stack<HLSLCompletionData> stack)
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

		protected HLSLCompletionData parent;

		public virtual IList<HLSLCompletionData> Members { get { return null; } }
		public virtual HLSLCompletionData Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		protected static CompletionDataList GetValidDataRecursive(HLSLCompletionData scope, ref bool recursionLock, ref CompletionDataList validData)
		{
			if (recursionLock)
				return null;
			recursionLock = true;
			if (validData == null)
				validData = new CompletionDataList();
			validData.Clear();
			if(scope.DataItems != null)
				validData.AddRange(scope.DataItems);
			if(scope.Members != null)
				validData.AddRange(scope.Members);
			var parentData = scope.Parent == null ? null : scope.Parent.GetValidData();
			if (parentData != null)
				validData.AddRange(parentData);
			recursionLock = false;
			return validData;
		}
	}

	public class HLSLKeyword : HLSLCompletionData
	{
		protected CompletionDataList validData;

		public override CompletionDataList DataItems
		{
			get
			{
				if (validData == null)
					validData = new CompletionDataList();
				return validData;
			}
		}

		public override CompletionDataList GetValidData()
		{
			return DataItems;
		}

		public HLSLKeyword(Substring name, string description, int imageIndex, int version = 0)
			: base(name, description, imageIndex, version)
		{
		}
	}

	public class HLSLMember : HLSLCompletionData
	{
		protected HLSLType type;
		protected List<HLSLKeyword> keywords;

		public virtual Substring Name
		{
			get { return text; }
			set
			{
				text = value;
				description = null;
			}
		}

		public override HLSLCompletionData Parent
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
					description = GetPrefix(Type, " ") + GetPrefix(Parent, ".") + Name;
				return description;
			}
		}

		public override void Parse(Substring item, Stack<HLSLCompletionData> stack)
		{
			if (item.Length < 1)
				return;
			var c = item[0];
			if(c == ';')
			{
				// Variable end
				stack.Pop();
				if (stack.Peek() == null)
					return;
				if (stack.Peek().Members == null)
					return;
				stack.Peek().Members.Add(new HLSLVariable(Name, Type, Parent, ImageIndex, Version));
			}
		}

		public HLSLMember(Substring name, HLSLType type, HLSLCompletionData parent, int imageIndex, int version = 0)
			: base(name, null, imageIndex, version)
		{
			this.type = type;
		}
	}

	public class HLSLType : HLSLCompletionData
	{
		protected IList<HLSLCompletionData> members;
		bool recursionLock;
		CompletionDataList validData;
		bool open, closed;

		// TODO: Typedefs and templates

		public virtual Substring TypeType
		{
			get { return null; }
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

		public override HLSLCompletionData Parent
		{
			get { return parent; }
			set
			{
				parent = value;
				description = null;
			}
		}

		public override IList<HLSLCompletionData> Members
		{
			get
			{
				if(members == null)
					members = new List<HLSLCompletionData>();
				return members;
			}
		}

		public override CompletionDataList GetValidData()
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

		public override void Parse(Substring item, Stack<HLSLCompletionData> stack)
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
				var data = GetValidData();
				var push = data == null ? null : data[item];
				if(push != null)
					stack.Push(push);
			} else // Closed
			{
				stack.Pop();
				stack.Push(new HLSLMember(item, this, stack.Peek() as HLSLType, 0, 0));
			}
		}

		public HLSLType(Substring name, HLSLCompletionData parent, int imageIndex, int version = 0)
			: base(name, null, imageIndex, version)
		{
			this.parent = parent;
		}
	}

	public class HLSLPrimitive : HLSLType
	{
		protected CompletionDataList dataItems;

		public override CompletionDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new CompletionDataList();
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
				return false;
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

		public override HLSLCompletionData Parent
		{
			get { return parent; }
			set
			{
				parent = value;
			}
		}

		public override CompletionDataList GetValidData()
		{
			return (Open && !Closed) ? base.GetValidData() : null;
		}

		public HLSLPrimitive(Substring name, string description, HLSLCompletionData parent, int version = 0)
			: base(name, parent, 1, version)
		{
			this.description = description;
			DataItems.Add(new HLSLCompletionData("wxyz", "", 0, 0));
			DataItems.Add(new HLSLCompletionData("x", "", 0, 0));
			DataItems.Add(new HLSLCompletionData("y", "", 0, 0));
			DataItems.Add(new HLSLCompletionData("z", "", 0, 0));
			DataItems.Add(new HLSLCompletionData("w", "", 0, 0));
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

		public HLSLVariable(Substring name, HLSLType type, HLSLCompletionData parent, int imageIndex, int version = 0)
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

		public HLSLLocalVariable(Substring name, HLSLType type, HLSLCompletionData parent, int imageIndex, int version = 0)
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

	public class HLSLScope : HLSLCompletionData
	{
		protected CompletionDataList members;
		protected CompletionDataList validData;
		bool recursionLock;

		public override IList<HLSLCompletionData> Members
		{
			get
			{
				if (members == null)
					members = new CompletionDataList();
				return members;
			}
		}


		public override CompletionDataList GetValidData()
		{
			return GetValidDataRecursive(this, ref recursionLock, ref validData);
		}

		public override void Parse(Substring item, Stack<HLSLCompletionData> stack)
		{
			if (item.Length < 1)
				return;
			var c = item[0];
			if (c == '{')
				stack.Push(new HLSLScope("", this, -1, 0));
			else if (c == '}')
				stack.Pop();
			else
			{
				var allData = GetValidData();
				var dataItem = allData[item];
				if (dataItem != null)
					stack.Push(dataItem);
			}
		}

		public HLSLScope(string name, HLSLCompletionData parent, int imageIndex, int version = 0)
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

		public HLSLFunction(string name, HLSLCompletionData parent, int imageIndex, int version = 0)
			: base(name, parent, imageIndex, version)
		{
		}
	}
	
	public class HLSLRootScope : HLSLScope
	{
		protected CompletionDataList dataItems;

		public override CompletionDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new CompletionDataList();
				return dataItems;
			}
		}

		public override CompletionDataList GetValidData()
		{
			if (validData == null)
				validData = new CompletionDataList();
			else
				validData.Clear();
			validData.AddRange(DataItems);
			validData.AddRange(Members);
			return validData;
		}

		public HLSLRootScope() : base("", null, -1, 0)
		{
		}
	}

	public static class CompletionUtility
	{
		public static readonly HashSet<char> Operators =
			new HashSet<char> { '.', ':', ';', '<', '>', '@',
				'[', ']', '{', '}', '(', ')', '#', '\'', '"',
				'+', '-',  '&', '|', '~', '^', '$', '*', '/', '\\', '?', '!' };
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
		Stack<HLSLCompletionData> stack = new Stack<HLSLCompletionData>();

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
			var data = stack.Peek().GetValidData();
			return data == null ? null : data.ToArray();
		}

		
	}
}
