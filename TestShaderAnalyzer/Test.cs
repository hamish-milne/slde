
using System.Diagnostics;
using System.IO;
using SLDE.ShaderAnalyzer;
using SLDE.ShaderAnalyzer.HLSL;

namespace TestShaderAnalyzer
{
    static class Test
    {
        static void Main() {
            Analyzer analyzer = new Analyzer();
            analyzer.options.compatibilityMode = true;

            string path = "./testshader.hlsl";
			string shader = File.ReadAllText(path);
            
            analyzer.UpdateShader(shader, path);
            IAssembly assembly = analyzer.Compile("main", ShaderModel.PIXEL_SHADER_3_0);


            Debug.Print("\n---------- Assembly ----------");

            Debug.Print(assembly.GetRawCompilerOutput());

            Debug.Print("\n---------- Errors ----------");

            Debug.Print(assembly.GetRawCompilerErrors());

            Debug.Print("\n---------- Parsed assembly ----------");

            foreach (Instruction inst in assembly.GetInstructions()) {
                Debug.Print("{0}: {1}", inst.SourceLine, inst.DisplayString);
            }

            Debug.Print("\n");

            analyzer.Cleanup();
        }
    }
}
