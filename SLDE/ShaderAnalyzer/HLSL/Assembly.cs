
using System;
using System.Collections.Generic;

namespace SLDE.ShaderAnalyzer.HLSL {

    public class Assembly : IAssembly {
        private string[] assemblyLines;
        private string[] errorLines;

        public Assembly(string[] assemblyLines, string[] errorLines) {
            this.assemblyLines = assemblyLines;
            this.errorLines = errorLines;
        }

        public bool CompiledSuccessfully() {
            return true;
        }

        public string GetRawCompilerOutput() {
            return String.Join("\n", assemblyLines);
        }

        public string GetRawCompilerErrors() {
            return String.Join("\n", errorLines);
        }

        public IEnumerable<Instruction> GetInstructions() {
            throw new NotImplementedException();
        }

        public IEnumerable<Notification> GetNotifications() {
            throw new NotImplementedException();
        }

        public IEnumerable<CodeNotification> GetCodeNotifications() {
            throw new NotImplementedException();
        }

        public IEnumerable<AssemblyNotification> GetAssemblyNotifications() {
            throw new NotImplementedException();
        }
    }

}
