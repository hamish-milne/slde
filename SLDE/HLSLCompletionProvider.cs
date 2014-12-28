using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;
using System.Windows.Forms;
using SLDE.Properties;

namespace SLDE.HLSL.Completion
{
	using SLDE.Completion;

	public class HLSLKeyword : CompletionData
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

		public virtual IDataList Members
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
			CompletionData dataItem;
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
					if (/*!isFunction &&*/ c == '{')
					{
						var scope = new HLSLScope("", stack.Peek());
						scope.Members.AddRange(Members);
						stack.Push(scope);
						return;
					}
					if (stack.Peek() == null)
						return;
					var dataItem = Create(stack);
					stack.Peek().AddChild(dataItem);
					// 'is HLSLMember' is used to check
					// if we're defining the parameters of a function
					if (c == ',' && !(stack.Peek() is HLSLMember))
						stack.Push(Type);
					else if (c == ')' && stack.Peek() is HLSLMember)
						stack.Peek().Parse(item, stack);
					// Functions are now separate from scopes,
					// so we don't need the following:
					//else if (c == '{' && isFunction)
					//	stack.Push(dataItem);
					break;
				default:
					if (!item.IsOperator())
					{
						if (hasParameters && Parent != null)
						{
							stack.Push(Parent.GetVisibleItems(stack)[item]);
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
			if (!Open || Closed)
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
			if(!Open && !Closed)
			{
				if(item.IsOperator())
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
				var data = GetVisibleItems(stack);
				var push = data == null ? null : data[item];
				if(push != null)
					stack.Push(push);
			} else // Closed
			{
				stack.Pop();
				if(!item.IsOperator())
					stack.Push(new HLSLMember(item, this, stack.Peek()));
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

	public class HLSLPrimitive : HLSLType
	{
		protected IDataList dataItems;

		public virtual IDataList DataItems
		{
			get
			{
				if (dataItems == null)
					dataItems = new DataList();
				return dataItems;
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

	public class HLSLScopeBase : CompletionData
	{
		protected IDataList members;

		public virtual IDataList Members
		{
			get
			{
				if (members == null)
					members = new DataList();
				return members;
			}
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
				var allData = GetVisibleItems(stack);
				var dataItem = allData[item];
				if (dataItem != null)
					stack.Push(dataItem);
			}
		}

		public override void AddChild(CompletionData item)
		{
			if (item is HLSLVariable)
				Members.Add(item);
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
		{
			return base.GetVisibleItems<T>(stack).AddRange(Members);
		}

		public HLSLScopeBase(Substring name, CompletionData parent)
			: base(name, null)
		{
			base.Parent = parent;
		}
	}

	public class HLSLScope : HLSLScopeBase
	{
		public HLSLScope(Substring name, CompletionData parent)
			: base(name, parent)
		{
		}
	}

	public class HLSLFunction : HLSLMember
	{
		IDataList arguments;
		IDataList dataItems;
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

		public virtual IDataList DataItems
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
			var data = GetVisibleItems(stack);
			if (data == null)
				return Arguments;
			data.AddRange(Arguments);
			return data;
		}

		public HLSLFunction(Substring name, HLSLType type, CompletionData parent)
			: base(name, type, parent)
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
					if(c != ',' && c != '>')
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
				else if (item.IsOperator())
					stack.Pop();
			}
		}

		protected abstract void CloseTemplate();

		public HLSLTemplate(Substring name, string description, CompletionData parent)
			: base(name, description, parent)
		{
		}
	}

	public class HLSLVector : HLSLTemplate
	{
		protected override void CloseTemplate()
		{
			if(templateParams.Count < 2)
				return;
			var type = templateParams[0] as HLSLPrimitive;
			if (type == null || templateParams[1] == null)
				return;
			int num;
			if (!Int32.TryParse(templateParams[1].Text.ToString(), out num))
				return;
			if (num < 1 || num > 4)
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
					Members.Add(new HLSLVariable("xz", type, this));
					Members.Add(new HLSLVariable("zx", type, this));
					Members.Add(new HLSLVariable("yz", type, this));
					Members.Add(new HLSLVariable("zy", type, this));

					Members.Add(new HLSLVariable("xyz", type, this));
					Members.Add(new HLSLVariable("yxz", type, this));
					Members.Add(new HLSLVariable("yzx", type, this));
					Members.Add(new HLSLVariable("xzy", type, this));
					Members.Add(new HLSLVariable("zxy", type, this));
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


		public HLSLVector(string description, CompletionData parent)
			: base("vector", description, parent)
		{
		}

		public HLSLVector(HLSLPrimitive type, int number, CompletionData parent)
			: this("", parent)
		{
			templateOpen = true;
			templateClosed = true;
			templateParams.Add(type);
			templateParams.Add(new CompletionData(number.ToString(), ""));
			CloseTemplate();
		}
	}
	
	public class HLSLRootScope : HLSLScope
	{
		protected IDataList dataItems;
		protected HashSet<Substring> typeTypes
			= new HashSet<Substring>() { "struct", "class", "interface" };
		protected IDataList rootDataItems;
		protected IDataList validData;

		public virtual IDataList DataItems
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
			else if (item == "vector")
				stack.Push(new HLSLVector("", this));
			else
				base.Parse(item, stack);
		}

		public override IDataList GetVisibleItems<T>(Stack<CompletionData> stack)
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
				'/', '*', /*'+', '-',  '&', '|', '~', '^',*/ '$', 
				'\\', '?', '!' , ',' };

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

			HLSLType Float4 = Float, Float2 = Float, UInt3 = UInt, Float3 = Float;

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
					if(pos >= caretPos
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
			return data == null ? null : data.ToArray();
		}

		
	}
}
