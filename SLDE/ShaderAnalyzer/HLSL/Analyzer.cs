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

    public class Analyzer {
        public CompileOptions options;

        public void PreprocessShader(String shader, List<String> includePaths) {
            
        }

        public Assembly Compile(String entryPoint, Profile target) {
            return null;
        }

        public class CompileOptions {
            public Dictionary<String, String> defines = new Dictionary<string, string>();

            public FlowControlFlag flowControl = FlowControlFlag.NotSet;
            public MatrixPackingFlag matrixPacking = MatrixPackingFlag.NotSet;
            public OptimizationFlag optimization = OptimizationFlag.Level1;
            public bool compatibilityMode = false;
            public bool strictMode = false;
        }
    }
}
