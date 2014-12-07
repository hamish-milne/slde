
using System;
using System.Collections.Generic;

namespace SLDE.ShaderAnalyzer.HLSL {

    public class Assembly : IAssembly {
        private List<string> assemblyLines;
        private List<string> errorLines;

        public Assembly(List<string> assemblyLines, List<string> errorLines) {
            this.assemblyLines = assemblyLines;
            this.errorLines = errorLines;
        }

        public IEnumerable<String> GetAssemblyLinesEnumerable() {
            return assemblyLines;
        }

        public IEnumerable<String> GetErrorLinesEnumerable() {
            return errorLines;
        }
        
    }

}
