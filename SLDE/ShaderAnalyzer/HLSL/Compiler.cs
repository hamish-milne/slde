using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SLDE.ShaderAnalyzer.HLSL {
    public class Compiler {
        private static readonly string PATH_TO_FXC = "./fxc/fxc.exe";

        private List<string> outputLines = new List<string>();
        private List<string> errorLines = new List<string>();

        public static Assembly CompileToAssembly(string shaderPath, string entryPoint, Profile target, CompileOptions options) {
            var compiler = new Compiler();
            compiler.Compile(shaderPath, entryPoint, target, options);
            return compiler.AsAssembly();
        }

        public void Compile(string shaderPath, string entryPoint, Profile target, CompileOptions options) {
            using (Process fxc = new Process()) {
                fxc.EnableRaisingEvents = false;
                fxc.StartInfo.FileName = PATH_TO_FXC;
                fxc.StartInfo.Arguments = BuildArguments(shaderPath, entryPoint, target, options);
                fxc.StartInfo.CreateNoWindow = true;
                fxc.StartInfo.UseShellExecute = false;
                fxc.StartInfo.RedirectStandardOutput = true;
                fxc.StartInfo.RedirectStandardError = true;

                fxc.Start();

                fxc.OutputDataReceived += OnNewOutputLine;
                fxc.BeginOutputReadLine();
                fxc.ErrorDataReceived += OnNewErrorLine;
                fxc.BeginErrorReadLine();

                fxc.WaitForExit();
            }
        }

        public Assembly AsAssembly() {
            return new Assembly(outputLines.ToArray(), errorLines.ToArray());
        }

        private String BuildArguments(string shaderPath, string entryPoint, Profile target, CompileOptions options) {
            var args = new StringBuilder();

            args.AppendFormat("/E {0} ", entryPoint);
            args.AppendFormat("/T {0} ", target.signature);
            args.Append("/Zi "); // Enables debug information
            args.Append("/nologo ");

            foreach (KeyValuePair<string, string> entry in options.Defines) {
                args.AppendFormat("/D \"{0} = {1}\" ", entry.Key, entry.Value);
            }

            if (options.compatibilityMode) {
                args.Append("/Gec ");
            }

            if (options.strictMode) {
                args.Append("/Ges ");
            }

            switch (options.flowControl) {
                case FlowControlFlag.Avoid:
                    args.Append("/Gfa ");
                    break;
                case FlowControlFlag.Prefer:
                    args.Append("/Gfp ");
                    break;
            }

            switch (options.matrixPacking) {
                case MatrixPackingFlag.ColumnMajor:
                    args.Append("/Zpc ");
                    break;
                case MatrixPackingFlag.RowMajor:
                    args.Append("/Zpr ");
                    break;
            }

            switch (options.optimization) {
                case OptimizationFlag.Disabled:
                    args.Append("/Od ");
                    break;
                case OptimizationFlag.Level0:
                    args.Append("/O0 ");
                    break;
                case OptimizationFlag.Level1:
                    args.Append("/O1 ");
                    break;
                case OptimizationFlag.Level2:
                    args.Append("/O2 ");
                    break;
                case OptimizationFlag.Level3:
                    args.Append("/O3 ");
                    break;
            }

            args.Append(shaderPath);

            return args.ToString();
        }

        private void OnNewOutputLine(object sender, DataReceivedEventArgs e) {
            if (e.Data != null) {
                outputLines.Add(e.Data.Trim());  
            }
        }

        private void OnNewErrorLine(object sender, DataReceivedEventArgs e) {
            if (e.Data != null) {
                errorLines.Add(e.Data.Trim());
            }
        }
    }
}
