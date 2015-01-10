using System;
using System.Collections;
using System.Collections.Generic;

using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;

namespace SLDE.Completion
{
	public interface IDataList : ICollection<CompletionData>
	{
		CompletionData this[Substring name] { get; }
		IDataList AddRange(IDataList list);
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
			// TODO: Add priority combining
			// For now, new > old
			//if (!dict.ContainsKey(item.Text))
			//	dict.Add(item.Text, item);
			dict[item.Text] = item;
		}

		public IDataList AddRange(IDataList list)
		{
			if (list == null)
				return this;
			foreach (var item in list)
				Add(item);
			return this;
		}

		public IEnumerator<CompletionData> GetEnumerator()
		{
			return new Enumerator(dict.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

	public interface IPreprocessorData
	{
		public IDictionary<Substring, Substring> PreprocessorData { get; }
	}

	public struct Substring : IEquatable<Substring>
	{
		static Dictionary<string, char[]> charArrayCache
			= new Dictionary<string, char[]>();

		char[] source;
		int start;
		int length;

		static char[] GetCharArray(string source)
		{
			char[] ret;
			if(!charArrayCache.TryGetValue(source, out ret))
			{
				ret = source.ToCharArray();
				charArrayCache.Add(source, ret);
			}
			return ret;
		}

		public Substring(char[] source, int start, int length)
		{
			this.source = source;
			this.start = start;
			this.length = length;
		}

		public Substring(string source, int start, int length)
		{
			if (source == null || start < 0 || length < 0
				|| (start + length) > source.Length)
				throw new ArgumentException("Invalid substring arguments");
			this.source = GetCharArray(source);
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
			return new string(source, start, length);
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
		protected bool hidden;

		public virtual bool Hidden
		{
			get { return hidden; }
			set { hidden = value; }
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
			get { return description == null ? ToString() : description; }
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

		/*public virtual IDataList DataItems
		{
			get { return null; }
		}

		public virtual IDataList GetValidData(Stack<CompletionData> stack)
		{
			return DataItems;
		}*/

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
			var items = GetVisibleItems(stack);
			if (items == null)
				stack.Pop();
			else
			{
				var dataItem = items[item];
				if (dataItem == null)
					stack.Pop();
				else
					dataItem.Select(stack);
			}
		}

		CompletionData parent;
		bool recursionLock;

		public virtual CompletionData Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public virtual void AddChild(CompletionData item)
		{
		}

		public IDataList GetVisibleItems(Stack<CompletionData> stack)
		{
			return GetVisibleItems<CompletionData>(stack);
		}

		public virtual IDataList GetVisibleItems<T>(Stack<CompletionData> stack) where T : CompletionData
		{
			if (recursionLock || Parent == null)
				return new DataList();
			recursionLock = true;
			var ret = Parent.GetVisibleItems<T>(stack);
			recursionLock = false;
			return ret;
		}

		public virtual void Select(Stack<CompletionData> stack)
		{
			stack.Push(this);
		}

		public virtual bool AsNumber(out double number)
		{
			return Double.TryParse(Text.ToString(), out number);
		}

		public override string ToString()
		{
			return Text.ToString();
		}
	}

	public class NumberItem : CompletionData
	{
		double number;

		public virtual double Number
		{
			get { return number; }
			set { number = value; }
		}

		public override Substring Text
		{
			get { return number.ToString(); }
		}

		public override bool AsNumber(out double number)
		{
			number = this.number;
			return true;
		}

		public NumberItem(double number)
			: base("", "")
		{
			this.number = number;
			base.Hidden = true;
		}
	}

	public class CreatorKeyword<T> : CompletionData where T : CompletionData, new()
	{
		public override void Select(Stack<CompletionData> stack)
		{
			var obj = new T();
			obj.Parent = stack.Peek();
			obj.Select(stack);
		}

		public CreatorKeyword(Substring text, string description)
			: base(text, description)
		{
		}
	}
}
