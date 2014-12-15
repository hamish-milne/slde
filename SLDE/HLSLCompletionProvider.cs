using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;

namespace SLDE.HLSL.Completion
{
	public struct Substring
	{
		string source;
		int start;
		int length;

		public Substring(string source, int start, int length)
		{
			if (source == null || start < 0 || start >= source.Length
				|| length < 0 || (start + length) >= source.Length)
				throw new ArgumentException("Invalid substring arguments");
			this.source = source;
			this.start = start;
			this.length = length;
		}
	}

	public class HLSLCompletionData : ICompletionData
	{
		protected string text;
		protected string description;
		protected int imageIndex;
		protected double priority = 0;
		protected int version;

		public virtual bool IsCompatible
		{
			get { return true; }
		}

		public virtual string Text
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

		public virtual IList<HLSLCompletionData> ValidData
		{
			get { return null; }
		}

		public virtual bool InsertAction(TextArea textArea, char insertChar)
		{
			return true;
		}

		public HLSLCompletionData(string text, string description, int imageIndex, int version = 0)
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

		public virtual void Parse(ref int position, TextArea textArea, out HLSLCompletionData newData)
		{
			newData = null;
		}

		protected static IList<HLSLCompletionData> GetValidDataRecursive(IScope scope, ref bool recursionLock, ref List<HLSLCompletionData> validData)
		{
			if (recursionLock)
				return null;
			recursionLock = true;
			if (validData == null)
				validData = new List<HLSLCompletionData>();
			validData.Clear();
			validData.AddRange(scope.Members);
			var parentData = scope.Parent == null ? null : scope.Parent.ValidData;
			if (parentData != null)
				validData.AddRange(parentData);
			recursionLock = false;
			return validData;
		}
	}

	public class HLSLKeyword : HLSLCompletionData
	{
		protected IList<HLSLCompletionData> validData;

		public override IList<HLSLCompletionData> ValidData
		{
			get
			{
				if (validData == null)
					validData = new List<HLSLCompletionData>();
				return validData;
			}
		}

		public HLSLKeyword(string name, string description, int imageIndex, int version = 0)
			: base(name, description, imageIndex, version)
		{
		}
	}

	public class HLSLMember : HLSLCompletionData
	{
		protected HLSLType type;
		protected HLSLType parent;

		public virtual string Name
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

		public HLSLMember(string name, HLSLType type, HLSLType parent, int imageIndex, int version = 0)
			: base(name, null, imageIndex, version)
		{
		}
	}

	public class HLSLType : HLSLCompletionData, IScope
	{
		protected IList<HLSLCompletionData> members;
		protected IScope parent;
		bool recursionLock;
		List<HLSLCompletionData> validData;

		// TODO: Typedefs and templates

		public virtual string TypeType
		{
			get { return null; }
		}

		public virtual string Name
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

		public override IList<HLSLCompletionData> ValidData
		{
			get
			{
				return GetValidDataRecursive(this, ref recursionLock, ref validData);
			}
		}

		public override string Description
		{
			get
			{
				if (description == null)
					description = (TypeType == null ? "" : TypeType + " ") + Name;
				return description;
			}
		}

		public HLSLType(string name, IScope parent, int imageIndex, int version = 0)
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

		public override string Name
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

		public HLSLPrimitive(string name, string description, IScope parent, int version = 0)
			: base(name, parent, 1, version)
		{
			this.description = description;
		}
	}

	public class HLSLStruct : HLSLType
	{
		public override string TypeType
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
		public override string TypeType
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
		public override string TypeType
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
					description = Name;
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
		IList<HLSLCompletionData> ValidData { get; }
		IScope Parent { get; }
	}

	public class HLSLScope : HLSLCompletionData, IScope
	{
		protected IList<HLSLCompletionData> members;
		protected IScope parent;
		protected List<HLSLCompletionData> validData;
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


		public override IList<HLSLCompletionData> ValidData
		{
			get
			{
				return GetValidDataRecursive(this, ref recursionLock, ref validData);
			}
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

	public abstract class HLSLCompletionProvider : AbstractCompletionDataProvider
	{
		HLSLScope root;
		HLSLPrimitive Bool, Int, UInt, DWord, Half, Float, Double;
		HLSLPrimitive Min16float, Min10float, Min16int, Min12int, Min16uint;


		public HLSLCompletionProvider()
		{
			root = new HLSLScope("", null, -1, 0);
			Bool		= new HLSLPrimitive("bool", "true or false", root);
			Int			= new HLSLPrimitive("int", "32-bit signed integer", root);
			UInt		= new HLSLPrimitive("uint", "32-bit unsigned integer", root);
			DWord		= new HLSLPrimitive("dword", "32-bit unsigned integer", root);
			Half		= new HLSLPrimitive("half", "16-bit floating point value", root);
			Float		= new HLSLPrimitive("float", "32-bit floating point value", root);
			Double		= new HLSLPrimitive("double", "64-bit floating point value", root);
			Min16float	= new HLSLPrimitive("min16float", "Minimum 16-bit floating point value", root, 112);
			Min10float	= new HLSLPrimitive("min10float", "Minimum 10-bit floating point value", root, 112);
			Min16int	= new HLSLPrimitive("min16int", "Minimum 16-bit signed integer", root, 112);
			Min12int	= new HLSLPrimitive("min12int", "Minimum 12-bit signed integer", root, 112);
			Min16uint	= new HLSLPrimitive("min16uint", "Minimum 16-bit unsigned integer", root, 112);

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
		}


		// Image list:
		// 0: Not compatible
		// 1: Intrinsic type
		// 2: Keyword
		// 3: Semantic
		// 

		static readonly HLSLCompletionData[] modifiers = new HLSLCompletionData[]
		{
			new HLSLCompletionData("snorm", "normalize float in range -1 to 1", 2, 100),
			new HLSLCompletionData("unorm", "normalize float in range 0 to 1", 2, 100),
		};

		static readonly HLSLCompletionData[] scalarType = new HLSLCompletionData[]
		{
			new HLSLCompletionData("bool", "true or false", 1),
			new HLSLCompletionData("int", "32-bit signed integer", 1),
			new HLSLCompletionData("uint", "32-bit unsigned integer", 1),
			new HLSLCompletionData("dword", "32-bit unsigned integer", 1),
			new HLSLCompletionData("half", "16-bit floating point value", 1),
			new HLSLCompletionData("float", "32-bit floating point value", 1),
			new HLSLCompletionData("double", "64-bit floating point value", 1),
			new HLSLCompletionData("min16float", "Minimum 16-bit floating point value", 1, 112),
			new HLSLCompletionData("min10float", "Minimum 10-bit floating point value", 1, 112),
			new HLSLCompletionData("min16int", "Minimum 16-bit signed integer", 1, 112),
			new HLSLCompletionData("min12int", "Minimum 12-bit signed integer", 1, 112),
			new HLSLCompletionData("min16uint", "Minimum 16-bit unsigned integer", 1, 112),
		};

		static readonly HLSLCompletionData[] miscTypes = new HLSLCompletionData[]
		{
			new HLSLCompletionData("string", "ASCII string", 1),
		};

		static readonly HLSLCompletionData[] intrinsicTemplates = new HLSLCompletionData[]
		{
			new HLSLCompletionData("vector", "", 1),
			new HLSLCompletionData("Buffer", "", 1),
			new HLSLCompletionData("matrix", "", 1),
		};

		static readonly HLSLCompletionData[] samplers = new HLSLCompletionData[]
		{
			new HLSLCompletionData("sampler", "Direct3D 9 only", 1),
			new HLSLCompletionData("sampler1D", "One dimensional sampler", 1),
			new HLSLCompletionData("sampler1D", "One dimensional sampler", 1),
		};

		
	}
}
