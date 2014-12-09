using System;
using System.Collections.Generic;

namespace SLDE.ShaderAnalyzer.HLSL {

    public enum FlowControlFlag {
        NotSet,
        Prefer,
        Avoid
    }

    public enum MatrixPackingFlag {
        NotSet,
        ColumnMajor,
        RowMajor
    }

    public enum OptimizationFlag {
        Disabled,
        Level0,
        Level1,
        Level2,
        Level3,
    }

    public class CompileOptions {
        Dictionary<string, string> defines = new Dictionary<string, string>();
        List<string> includePaths = new List<string>();

        public FlowControlFlag flowControl = FlowControlFlag.NotSet;
        public MatrixPackingFlag matrixPacking = MatrixPackingFlag.NotSet;
        public OptimizationFlag optimization = OptimizationFlag.Level3;
        public bool compatibilityMode = false;
        public bool strictMode = false;

		// This will be set by the editor. The stream can be found in ProjectFiles
		public virtual string MainFile { get; set; }

		public IDictionary<string, string> Defines
		{
			get { return defines; }
		}

		public IList<string> IncludePaths
		{
			get { return includePaths; }
		}

		// This allows the editor to set whatever properties it wants as settings
		// but the properties we're interested in will still be available
		Dictionary<string, int> properties = new Dictionary<string, int>();
		public IDictionary<string, int> Properties
		{
			get { return properties; }
		}

		// Files provided as streams to support compiling while not saved (for real-time analysis/errors)
		public IDictionary<string, System.IO.Stream> ProjectFiles
		{
			get { return null; }
		}
    }
}
