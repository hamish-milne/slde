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
        public Dictionary<string, string> defines = new Dictionary<string, string>();
        public List<string> includePaths = new List<string>();

        public FlowControlFlag flowControl = FlowControlFlag.NotSet;
        public MatrixPackingFlag matrixPacking = MatrixPackingFlag.NotSet;
        public OptimizationFlag optimization = OptimizationFlag.Level3;
        public bool compatibilityMode = false;
        public bool strictMode = false;
    }
}
