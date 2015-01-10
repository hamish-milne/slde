using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;
using DigitalRune.Windows.TextEditor.TextBuffer;
using System.Windows.Forms;
using SLDE.Properties;
using System.Text;

namespace SLDE.HLSL.Completion
{
	using SLDE.Completion;

	public class HLSLModifier : CompletionData
	{
		protected IDataList validData;

		public virtual IDataList DataItems
		{
			get
			{
				if (validData == null)
					validData = new DataList();
				return validData;
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			return DataItems;
		}

		public override void AddChild(CompletionData item)
		{
			if(item != null)
				DataItems.Add(item);
		}

		public HLSLModifier(Substring name, string description)
			: base(name, description)
		{
		}
	}

	public class HLSLMember : CompletionData
	{
		protected HLSLType type;
		protected List<HLSLModifier> keywords;
		bool hasSemantic, hasParameters, isFunction;
		HLSLSemantic parsedSemantic;
		IDataList members;
		IDataList modifiers;

		public override int ImageIndex
		{
			get
			{
				return 4;
			}
		}

		public virtual IDataList Members
		{
			get
			{
				if (members == null)
					members = new DataList();
				return members;
			}
		}

		public virtual IDataList Modifiers
		{
			get
			{
				if (modifiers == null)
					modifiers = new DataList();
				return modifiers;
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

		public virtual IList<HLSLModifier> Keywords
		{
			get
			{
				if (keywords == null)
					keywords = new List<HLSLModifier>();
				return keywords;
			}
		}

		public override string Description
		{
			get
			{
				if (description == null)
				{
					description = "";
					foreach (var item in Modifiers)
						description += item + " ";
					if (Parent != null && Parent is HLSLType)
						description += GetPrefix(Type, " ") + GetPrefix(Parent, ".") + Name;
					else
						description += GetPrefix(Type, " ") + Name;
				}
				return description;
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			if (hasSemantic && Type != null)
				return Type.Semantics;
			if (hasParameters)
				return base.GetVisibleItems<T>(stack);
			return null;
		}

		CompletionData Create(Stack<CompletionData> stack)
		{
			HLSLMember dataItem;
			if (isFunction)
			{
				var func = new HLSLFunction(Name, Type, stack.Peek());
				func.Arguments.AddRange(Members);
				func.Semantic = parsedSemantic;
				dataItem = func;
			}
			else
			{
				var variable = new HLSLVariable(Name, Type, stack.Peek());
				variable.Semantic = parsedSemantic;
				dataItem = variable;
			}
			dataItem.Modifiers.AddRange(Modifiers);
			return dataItem;
		}

		public override void AddChild(CompletionData item)
		{
			Members.Add(item);
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
						// If this is a function, don't
						// create the member yet
						if (isFunction)
							return;
					}
					if (isFunction && c == ',')
						return;
					stack.Pop();
					if (c == '{')
					{
						var scope = new HLSLScope(stack.Peek());
						scope.Members.AddRange(Members);
						scope.Select(stack);
						return;
					}
					if (stack.Peek() == null)
						return;
					var dataItem = Create(stack);
					stack.Peek().AddChild(dataItem);
					// 'is HLSLMember' is used to check
					// if we're defining the parameters of a function
					if (c == ',' && !(stack.Peek() is HLSLMember))
						Type.Select(stack);
					else if (c == ')' && stack.Peek() is HLSLMember)
						stack.Peek().Parse(item, stack);
					break;
				default:
					if (!item.IsOperator())
					{
						if (hasParameters && Parent != null)
						{
							Parent.GetVisibleItems(stack)[item].Select(stack);
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

		public override void Select(Stack<CompletionData> stack)
		{
			while (stack.Peek() is HLSLModifier)
				Modifiers.Add(stack.Pop());
			description = null;
			base.Select(stack);
		}

		public HLSLMember(Substring name, HLSLType type, CompletionData parent)
			: base(name, null)
		{
			this.type = type;
			base.Parent = parent;
		}
	}

	public class HLSLTypedef : HLSLType
	{
		HLSLType reference;

		public virtual HLSLType Reference
		{
			get { return reference; }
			set { reference = value; }
		}

		public override CompletionData Parent
		{
			get
			{
				return reference == null ? base.Parent : reference.Parent;
			}

			set
			{
				base.Parent = value;
				if (reference != null)
					reference.Parent = value;
			}
		}

		public override Substring TypeType
		{
			get
			{
				return reference == null ? base.TypeType : reference.TypeType;
			}
			set
			{
				base.TypeType = value;
				if (reference != null)
					reference.TypeType = value;
			}
		}

		public override IDataList Members
		{
			get
			{
				return reference == null ? null : reference.Members;
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			return reference == null ? null : reference.GetVisibleItems<T>(stack);
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

		public HLSLTypedef(Substring name, HLSLType reference)
			: base("", name, null)
		{
			this.reference = reference;
		}
	}

	public class HLSLType : CompletionData
	{
		IDataList members;
		IDataList semantics;
		bool closed;
		Substring typeType;

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
			get { return base.Parent; }
			set
			{
				base.Parent = value;
				description = null;
			}
		}

		public virtual IDataList Members
		{
			get
			{
				if(members == null)
					members = new DataList();
				return members;
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			if (Closed)
				return new DataList();
			return base.GetVisibleItems<T>(stack).AddRange(Members);
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
			if(!Closed)
			{
				if(c ==  '}')
				{
					closed = true;
					stack.Pop();
					return;
				}
				var data = GetVisibleItems(stack);
				var push = data == null ? null : data[item];
				if (push != null)
					push.Select(stack);
			} else
			{
				stack.Pop();
				if(!item.IsOperator())
				new HLSLMember(item, this, stack.Peek()).Select(stack);
			}
		}

		public override void AddChild(CompletionData item)
		{
			if(item is HLSLMember)
				Members.Add(item);
		}

		public HLSLType(Substring typeType, Substring name, CompletionData parent)
			: base(name, null)
		{
			this.typeType = typeType;
			base.Parent = parent;
		}
	}

	public class HLSLVoid : HLSLType
	{
		public override int ImageIndex
		{
			get
			{
				return 0;
			}
		}

		public override bool Closed
		{
			get
			{
				return true;
			}
		}

		public HLSLVoid(Substring name, string description, CompletionData parent)
			: base("", name, parent)
		{
			this.description = description;
		}
	}

	public class HLSLPrimitive : HLSLType
	{
		public override bool Closed
		{
			get
			{
				return true;
			}
		}

		public HLSLPrimitive(Substring name, string description, CompletionData parent)
			: base("", name, parent)
		{
			this.description = description;
		}
	}

	public class HLSLVariable : HLSLMember
	{
		HLSLSemantic semantic;
		bool hasDot;

		public virtual HLSLSemantic Semantic
		{
			get { return semantic; }
			set
			{
				semantic = value;
				description = null;
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			if (hasDot && Type != null)
			{
				hasDot = false;
				return Type.Members;
			}
			return base.GetVisibleItems<T>(stack);
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
					member.Select(stack);
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

	public class HLSLScopeBase : CompletionData
	{
		protected IDataList members;
		protected IDataList keywords;

		public virtual IDataList Members
		{
			get
			{
				if (members == null)
					members = new DataList();
				return members;
			}
		}

		public virtual IDataList Keywords
		{
			get
			{
				if (keywords == null)
					keywords = new DataList();
				return keywords;
			}
		}

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (item.Length < 1)
				return;
			var c = item[0];
			if (c == '{')
				new HLSLScope(this).Select(stack);
			else if (c == '}')
				stack.Pop();
			else
			{
				var allData = GetVisibleItems(stack);
				var dataItem = allData[item];
				if (dataItem != null)
					dataItem.Select(stack);
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			return base.GetVisibleItems<T>(stack).AddRange(Members).AddRange(Keywords);
		}

		public override void AddChild(CompletionData item)
		{
			Keywords.Add(item);
		}

		public HLSLScopeBase(Substring name, CompletionData parent)
			: base(name, null)
		{
			base.Parent = parent;
		}
	}

	public class HLSLNamespace : HLSLScopeBase
	{
		DataList types;

		public virtual IDataList Types
		{
			get
			{
				if (types == null)
					types = new DataList();
				return types;
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			var ret = base.GetVisibleItems<T>(stack);
			if (!(stack.Peek() is HLSLNamespace))
				ret.AddRange(Members);
			return ret;
		}

		public override void AddChild(CompletionData item)
		{
			if (item is HLSLType)
				Types.Add(item);
			else if (item is HLSLMember)
				Members.Add(item);
			else
				base.AddChild(item);
		}

		public HLSLNamespace(Substring name, CompletionData parent)
			: base(name, parent)
		{
		}
	}

	public class HLSLScope : HLSLScopeBase
	{
		public override void AddChild(CompletionData item)
		{
			if (item is HLSLVariable)
				Members.Add(item);
			else
				base.AddChild(item);
		}

		public HLSLScope(CompletionData parent)
			: base("", parent)
		{
		}
	}

	public class HLSLFunction : HLSLMember
	{
		IDataList arguments;
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

		public virtual HLSLSemantic Semantic
		{
			get { return semantic; }
			set { semantic = value; }
		}

		public override string Description
		{
			get
			{
				var ret = Type.Text + " " + Text + "(";
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

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			return GetVisibleItems(stack).AddRange(Arguments);
		}

		public HLSLFunction(Substring name, HLSLType type, CompletionData parent)
			: base(name, type, parent)
		{
		}
	}

	public class HLSLKeyword : CompletionData
	{
		public override void Select(Stack<CompletionData> stack)
		{
		}

		public HLSLKeyword(Substring text, string description)
			: base(text, description)
		{
		}
	}

	public abstract class HLSLTemplate : HLSLPrimitive
	{
		protected bool templateOpen;
		protected bool templateClosed;
		protected List<CompletionData> templateParams = new List<CompletionData>();

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (item.Length < 1)
				return;
			var c = item[0];
			if (templateOpen && templateClosed)
				base.Parse(item, stack);
			else if (templateOpen && !templateClosed)
			{
				if (item.IsOperator())
				{
					if(/*c != ',' && c != '>'*/ c == '{')
						stack.Pop();
				} else
				{
					var validData = Parent == null ?
						GetVisibleItems(stack) : Parent.GetVisibleItems(stack);
					CompletionData data = null;
					if (validData != null)
						data = validData[item];
					if (data == null)
						data = new CompletionData(item, "");
					templateParams.Add(data);
				}
				if (c == '>')
				{
					templateClosed = true;
					CloseTemplate();
				}
			} else
			{
				// !templateOpen
				if (c == '<')
					templateOpen = true;
				else
				{
					Default();
					base.Parse(item, stack);
				}
			}
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			var list = new DataList();
			if (Parent == null || !templateOpen || templateClosed || templateParams.Count > 0)
				return list;
			foreach (var item in Parent.GetVisibleItems<T>(stack))
				if (item != null && item is HLSLPrimitive)
					list.Add(item);
			return list;
		}

		protected abstract void CloseTemplate();
		protected abstract void Default();

		public HLSLTemplate(Substring name, string description, CompletionData parent)
			: base(name, description, parent)
		{
		}
	}

	public class HLSLVector : HLSLTemplate
	{
		public static HLSLType DefaultType;

		protected override void CloseTemplate()
		{
			if(templateParams.Count < 2)
				return;
			var type = templateParams[0] as HLSLPrimitive;
			if (type == null || templateParams[1] == null)
				return;
			double num;
			if (!templateParams[1].AsNumber(out num))
				return;
			if (num < 1)
				return;
			Members.Add(new HLSLVariable("x", type, this));
			if(num > 1)
			{
				Members.Add(new HLSLVariable("y", type, this));
				Members.Add(new HLSLVariable("xy", type, this));
				Members.Add(new HLSLVariable("yx", type, this));
				if(num > 2)
				{
					Members.Add(new HLSLVariable("z", type, this));
					//Members.Add(new HLSLVariable("xz", type, this));
					//Members.Add(new HLSLVariable("zx", type, this));
					//Members.Add(new HLSLVariable("yz", type, this));
					//Members.Add(new HLSLVariable("zy", type, this));

					Members.Add(new HLSLVariable("xyz", type, this));
					//Members.Add(new HLSLVariable("yxz", type, this));
					//Members.Add(new HLSLVariable("yzx", type, this));
					//Members.Add(new HLSLVariable("xzy", type, this));
					//Members.Add(new HLSLVariable("zxy", type, this));
					Members.Add(new HLSLVariable("zyx", type, this));
				}
				if(num > 3)
				{
					Members.Add(new HLSLVariable("w", type, this));

					Members.Add(new HLSLVariable("xyzw", type, this));
					Members.Add(new HLSLVariable("yzxw", type, this));
					Members.Add(new HLSLVariable("zyxw", type, this));
					Members.Add(new HLSLVariable("wzyx", type, this));
				}
			}
		}

		protected override void Default()
		{
			templateOpen = true;
			templateClosed = true;
			templateParams.Add(DefaultType);
			templateParams.Add(new NumberItem(4));
			CloseTemplate();
		}

		public HLSLVector(string description, CompletionData parent)
			: base("vector", description, parent)
		{
		}

		public HLSLVector()
			: this("", null)
		{
		}

		public HLSLVector(Substring name, HLSLPrimitive type, int number, CompletionData parent)
			: base(name, "", parent)
		{
			templateOpen = true;
			templateClosed = true;
			templateParams.Add(type);
			templateParams.Add(new NumberItem(number));
			CloseTemplate();
		}

		public override string ToString()
		{
			if (templateParams.Count < 1)
				return base.ToString();
			var list = new object[(templateParams.Count * 2) + 2];
			list[0] = base.ToString();
			list[1] = "<";
			for (int i = 0; i < templateParams.Count; i++)
			{
				list[(i * 2) + 2] = templateParams[i];
				if (i < (templateParams.Count - 1))
					list[(i * 2) + 3] = ", ";
			}
			list[(templateParams.Count * 2) + 1] = ">";
			return String.Concat(list);
		}
	}

	public class HLSLMatrix : HLSLTemplate
	{
		public static HLSLType DefaultType;

		protected override void CloseTemplate()
		{
			if (templateParams.Count < 2)
				return;
			var type = templateParams[0] as HLSLPrimitive;
			if (type == null || templateParams[1] == null)
				return;
			double rows, cols;
			if (!templateParams[1].AsNumber(out rows))
				return;
			if (templateParams.Count < 3 || templateParams[2] == null)
				cols = rows;
			else if (!templateParams[2].AsNumber(out cols))
				return;
			if (rows < 1 || cols < 1)
				return;
			if (rows > 4)
				rows = 4;
			if (cols > 4)
				cols = 4;
			Members.Add(new HLSLVariable("_m00", type, this));
			Members.Add(new HLSLVariable("_11", type, this));
			if (rows > 1 || cols > 1)
			{
				Members.Add(new HLSLVariable("_m" + ((int)rows-1).ToString() + ((int)cols-1).ToString(), type, this));
				Members.Add(new HLSLVariable("_" + ((int)rows).ToString() + ((int)cols).ToString(), type, this));
			}
		}

		protected override void Default()
		{
			templateOpen = true;
			templateClosed = true;
			templateParams.Add(DefaultType);
			templateParams.Add(new NumberItem(4));
			templateParams.Add(new NumberItem(4));
			CloseTemplate();
		}

		public HLSLMatrix(string description, CompletionData parent)
			: base("matrix", description, parent)
		{
		}

		public HLSLMatrix()
			: this("", null)
		{
		}

		public HLSLMatrix(Substring name, HLSLPrimitive type, int rows, int cols, CompletionData parent)
			: base(name, "", parent)
		{
			templateOpen = true;
			templateClosed = true;
			templateParams.Add(type);
			templateParams.Add(new NumberItem(rows));
			templateParams.Add(new NumberItem(cols));
			CloseTemplate();
		}
	}

	public class HLSLTypeKeyword : CompletionData
	{
		Substring parsedName;

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			return null;
		}

		public override void Parse(Substring item, Stack<CompletionData> stack)
		{
			if (item.Length < 1)
				return;
			if (item[0] == '{')
			{
				stack.Pop();
				var type = new HLSLType(Text, parsedName, stack.Peek());
				if (parsedName.Length > 0)
				{
					stack.Peek().AddChild(type);
					type.Select(stack);
				}
			}
			else if (!item.IsOperator())
				parsedName = item;
		}

		public HLSLTypeKeyword(Substring name, string description)
			: base(name, description)
		{
		}
	}
	
	public class HLSLRootScope : HLSLNamespace
	{
		protected IDataList dataItems;
		protected IDataList rootDataItems;
		protected IDataList validData;
		protected IDataList functionItems;

		public virtual IDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new DataList();
				return dataItems;
			}
		}

		public virtual IDataList FunctionItems
		{
			get
			{
				if (functionItems == null)
					functionItems = new DataList();
				return functionItems;
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

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			if (validData == null)
				validData = new DataList();
			else
				validData.Clear();
			validData.AddRange(Types)
				.AddRange((stack.Peek() is HLSLNamespace)
					? RootDataItems : Members.AddRange(FunctionItems))
				.AddRange(DataItems);
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
				'/', '*', '+', '-',  '&', '|', '~', '^', '$', 
				'\\', '?', '!' , ',', '=' };

		public static bool IsOperator(this Substring item)
		{
			if (item.Length < 1)
				return false;
			return Operators.Contains(item[0]);
		}

	}

	public class HLSLCompletionProvider : AbstractCompletionDataProvider
	{
		

		static Dictionary<char, string> operatorCache
			= new Dictionary<char, string>();

		HLSLRootScope root;
		HLSLVoid VoidType;
		HLSLPrimitive Bool, Int, UInt, DWord, Half, Float, Double;
		HLSLPrimitive Min16float, Min10float, Min16int, Min12int, Min16uint;
		HLSLModifier snorm, unorm;
		CompletionData vector, matrix;
		HLSLTypeKeyword Struct, Class, Interface;
		HLSLVector[] floatVectors, uintVectors;

		ImageList imageList;
		Stack<CompletionData> stack = new Stack<CompletionData>();

		public override ImageList ImageList
		{
			get { return imageList; }
		}

		HLSLVector[] MakeVectors(HLSLPrimitive primitive, HLSLRootScope root)
		{
			var ret = new HLSLVector[4];
			for (int i = 0; i < 4; i++)
			{
				ret[i] = new HLSLVector(primitive.Name.ToString() + (i + 1), primitive, i + 1, root);
				ret[i].Hidden = true;
				root.RootDataItems.Add(ret[i]);
			}
			return ret;
		}

		public HLSLCompletionProvider()
		{
			root = new HLSLRootScope();
			VoidType    = new HLSLVoid("void", "No data", root);
			// Primitives
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

			// Keywords
			snorm       = new HLSLModifier("snorm", "Normalize float in range -1 to 1");
			unorm       = new HLSLModifier("unorm", "Normalize float in range 0 to 1");
			Struct      = new HLSLTypeKeyword("struct", "A structure. Groups data together");
			Class       = new HLSLTypeKeyword("class", "Groups data and functions together");
			Interface   = new HLSLTypeKeyword("interface", "An interface to another type");
			vector		= new CreatorKeyword<HLSLVector>("vector", "The vector template type");
			matrix      = new CreatorKeyword<HLSLMatrix>("matrix", "The matrix template type");

			snorm.DataItems.Add(Float);
			snorm.DataItems.Add(Min10float);
			snorm.DataItems.Add(Min16float);
			unorm.DataItems.Add(Float);
			unorm.DataItems.Add(Min10float);
			unorm.DataItems.Add(Min16float);

			// Predefined vectors
			floatVectors = MakeVectors(Float, root);
			uintVectors = MakeVectors(UInt, root);
			MakeVectors(Bool, root);
			MakeVectors(Int, root);
			MakeVectors(DWord, root);
			MakeVectors(Half, root);
			MakeVectors(Double, root);

			var Float2 = floatVectors[1];
			var Float3 = floatVectors[2];
			var Float4 = floatVectors[3];
			var UInt3 = uintVectors[2];

			// DX9 semantics
			Float4.Semantics.Add(new HLSLSemantic("BINORMAL"));
			UInt.Semantics.Add(new HLSLSemantic("BLENDINDICES"));
			Float.Semantics.Add(new HLSLSemantic("BLENDWEIGHT"));
			Float4.Semantics.Add(new HLSLSemantic("COLOR"));
			Float4.Semantics.Add(new HLSLSemantic("NORMAL"));
			Float4.Semantics.Add(new HLSLSemantic("POSITION"));
			Float4.Semantics.Add(new HLSLSemantic("POSITIONT"));
			Float.Semantics.Add(new HLSLSemantic("PSIZE"));
			Float4.Semantics.Add(new HLSLSemantic("TANGENT"));
			Float4.Semantics.Add(new HLSLSemantic("TEXCOORD"));
			Float.Semantics.Add(new HLSLSemantic("FOG"));
			Float.Semantics.Add(new HLSLSemantic("TESSFACTOR"));
			Float.Semantics.Add(new HLSLSemantic("VFACE"));
			Float2.Semantics.Add(new HLSLSemantic("VPOS"));
			Float.Semantics.Add(new HLSLSemantic("DEPTH"));

			// DX10 semantics
			Float.Semantics.Add(new HLSLSemantic("SV_ClipDistance"));
			Float.Semantics.Add(new HLSLSemantic("SV_CullDistance"));
			UInt.Semantics.Add(new HLSLSemantic("SV_Coverage"));
			Float.Semantics.Add(new HLSLSemantic("SV_Depth"));
			UInt3.Semantics.Add(new HLSLSemantic("SV_DispatchThreadID"));
			Float2.Semantics.Add(new HLSLSemantic("SV_DomainLocation"));
			Float3.Semantics.Add(Float2.Semantics["SV_DomainLocation"]);
			UInt3.Semantics.Add(new HLSLSemantic("SV_GroupID"));
			UInt.Semantics.Add(new HLSLSemantic("SV_GroupIndex"));
			UInt3.Semantics.Add(new HLSLSemantic("SV_GroupThreadID"));
			UInt.Semantics.Add(new HLSLSemantic("SV_GSInstanceID"));
			Float.Semantics.Add(new HLSLSemantic("SV_InsideTessFactor"));
			Float2.Semantics.Add(Float.Semantics["SV_InsideTessFactor"]);
			Bool.Semantics.Add(new HLSLSemantic("SV_IsFrontFace"));
			UInt.Semantics.Add(new HLSLSemantic("SV_OutputControlPointID"));
			Float4.Semantics.Add(new HLSLSemantic("SV_Position"));
			UInt.Semantics.Add(new HLSLSemantic("SV_RenderTargetArrayIndex"));
			UInt.Semantics.Add(new HLSLSemantic("SV_SampleIndex"));
			Float.Semantics.Add(new HLSLSemantic("SV_Target"));
			Float2.Semantics.Add(new HLSLSemantic("SV_TessFactor"));
			Float3.Semantics.Add(Float2.Semantics["SV_TessFactor"]);
			Float4.Semantics.Add(Float2.Semantics["SV_TessFactor"]);
			UInt.Semantics.Add(new HLSLSemantic("SV_ViewportArrayIndex"));
			UInt.Semantics.Add(new HLSLSemantic("SV_InstanceID"));
			UInt.Semantics.Add(new HLSLSemantic("SV_PrimitiveID"));
			UInt.Semantics.Add(new HLSLSemantic("SV_VertexID"));

			// Add all the items
			root.DataItems.Add(VoidType);
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

			// Variable modifiers
			root.RootDataItems.Add(new HLSLModifier("extern", "Marks a global variable as external input to the shader"));
			root.DataItems.Add(new HLSLModifier("static", "Marks a local variable as persistent across function calls"));
			root.DataItems.Add(new HLSLModifier("nointerpolation", "Do not interpolate the outputs of a vertex shader"));
			root.DataItems.Add(new HLSLModifier("precise", "Prevent the compiler from making IEEE unsafe optimizations"));
			root.DataItems.Add(new HLSLModifier("shared", "Mark a variable for sharing between effects"));
			root.DataItems.Add(new HLSLModifier("groupshared", "Mark a variable for thread-group-shared memory"));
			root.DataItems.Add(new HLSLModifier("uniform", "Mark a variable whose data is consistent throughout execution"));
			root.DataItems.Add(new HLSLModifier("volatile", "Mark a variable that changes frequently"));

			root.DataItems.Add(new HLSLModifier("const", "Mark a variable that cannot be changed"));
			root.DataItems.Add(new HLSLModifier("row_major", "Mark a variable that stores four components in a single row"));
			root.DataItems.Add(new HLSLModifier("column_major", "Mark a variable that stores four components in a single column"));

			root.FunctionItems.Add(new HLSLKeyword("while", null));
			root.FunctionItems.Add(new HLSLKeyword("for", null));
			root.FunctionItems.Add(new HLSLKeyword("do", null));
			root.FunctionItems.Add(new HLSLKeyword("continue", null));
			root.FunctionItems.Add(new HLSLKeyword("break", null));
			root.FunctionItems.Add(new HLSLKeyword("return", null));
			root.FunctionItems.Add(new HLSLKeyword("for", null));
			root.FunctionItems.Add(new HLSLKeyword("true", null));
			root.FunctionItems.Add(new HLSLKeyword("false", null));
			
			HLSLVector.DefaultType = Float;
			HLSLMatrix.DefaultType = Float;

			root.DataItems.Add(snorm);
			root.DataItems.Add(unorm);
			root.RootDataItems.Add(Struct);
			root.RootDataItems.Add(Class);
			root.RootDataItems.Add(Interface);
			root.RootDataItems.Add(vector);
			root.RootDataItems.Add(matrix);

			imageList = new ImageList();
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			imageList.Images.Add(Resources.CodeLines);
			imageList.Images.Add(Resources.Structure);
			imageList.Images.Add(Resources.Class);
			imageList.Images.Add(Resources.Interface);
			imageList.Images.Add(Resources.Field);
			imageList.Images.Add(Resources.Method);
		}

		// This simply cleans the array before parsing the given item
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

		const int blockSize = 4096;
		char[] textBuffer = new char[blockSize];

		void EnsureBufferCapacity(int size, bool keepData)
		{
			if(size > textBuffer.Length)
			{
				var newSize = ((size % blockSize) + 1) * blockSize;
				if (keepData)
					Array.Resize(ref textBuffer, newSize);
				else
					textBuffer = new char[newSize];
			}
		}

		static bool IsNewline(char c)
		{
			return (c == '\n' || c == '\r');
		}

		static Func<char, bool> isNewLine = IsNewline;
		static Func<char, bool> isWhitespace = Char.IsWhiteSpace;

		IDictionary<string, string> defines = new Dictionary<string, string>();

		struct PPStack
		{
			public bool active;
			public bool wasActive;

			public PPStack(bool active, bool wasActive)
			{
				this.active = active;
				this.wasActive = wasActive;
			}
		}

		void Preprocess(ITextBufferStrategy text)
		{
			EnsureBufferCapacity(text.Length, false);
			bool validStart = true;
			var stack = new Stack<PPStack>();
			for(int i = 0; i < text.Length; i++)
			{
				var c = text.GetCharAt(i);
				if (IsNewline(c))
					validStart = true;
				else if(validStart)
				{
					if(c == '#')
					{
						string identifier;
						bool reverse = false;
						switch(ReadPPLine(text, ref i, isWhitespace))
						{
							case "include":
								break;
							case "define":
								if (stack.Count == 0 || stack.Peek().active)
									break;
								identifier = ReadPPLine(text, ref i, isWhitespace);
								if (identifier == null)
									break;
								defines[identifier] = ReadPPLine(text, ref i, isNewLine);
								break;
							case "undef":
								if (stack.Count == 0 || stack.Peek().active)
									break;
								identifier = ReadPPLine(text, ref i, isWhitespace);
								if (identifier != null)
									defines.Remove(identifier);
								break;
							case "if":
								// Not supported yet. Neither are logical operations etc.
								stack.Push(new PPStack(false, true));
								break;
							case "ifndef":
								reverse = true;
								goto case "ifdef";
							case "ifdef":
								if(!stack.Peek().active)
									stack.Push(new PPStack(false, true));
								else
								{
									var contains = defines.ContainsKey(ReadPPLine(text, ref i, isWhitespace));
									contains ^= reverse;
									stack.Push(new PPStack(contains, contains));
								}
								break;
							case "else":
								stack.Push(new PPStack(!stack.Pop().wasActive, true));
								break;
							case "elif":
								var item = stack.Pop();
								if (item.wasActive)
									stack.Push(new PPStack(false, true));
								else
								{
									var contains = defines.ContainsKey(ReadPPLine(text, ref i, isWhitespace));
									stack.Push(new PPStack(contains, contains));
								}
								break;
							case "endif":
								stack.Pop();
								break;
							case null:
								break;
							default: // line, pragma, error
								while (!IsNewline(text.GetCharAt(++i))) ;
								break;
						}
					} else if(!Char.IsWhiteSpace(c))
					{
						validStart = false;
					}
				}
			}
		}

		string ReadPPLine(ITextBufferStrategy text, ref int i, Func<char, bool> test)
		{
			char c;
			do
			{
				c = text.GetCharAt(++i);
				if (IsNewline(c))
					return null;
			} while (Char.IsWhiteSpace(c));
			int start = i++;
			int length = -1;
			while (!test(text.GetCharAt(i + ++length))) ;
			return text.GetText(start, length); 
		}

		public override ICompletionData[] GenerateCompletionData(string fileName,
			TextArea textArea, char charTyped)
		{
			stack.Clear();
			stack.Push(root);
			root.Members.Clear();
			var text = textArea.Document.TextBufferStrategy;
			var caretPos = textArea.Caret.Offset;
			for(int pos = 0; pos <= caretPos; pos++)
			{
				var c = pos >= caretPos ? charTyped : text.GetCharAt(pos);
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
				while (++pos <= caretPos)
				{
					if(pos < caretPos)
						c = text.GetCharAt(pos);
					if((pos >= caretPos && charTyped == '.')
						|| Char.IsWhiteSpace(c) || Char.IsControl(c)
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
			var data = stack.Peek().GetVisibleItems(stack);
			if (data == null) return null;
			var list = new List<ICompletionData>(data.Count);
			foreach (var item in data)
				if (item != null && !item.Hidden)
					list.Add(item);
			return list.ToArray();
		}

		
	}
}
