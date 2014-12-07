
using System.Diagnostics;
using System.IO;

using SLDE.ShaderAnalyzer.HLSL;

namespace TestShaderAnalyzer
{
    static class Test
    {
        static void Main() {
            Analyzer analyzer = new Analyzer();

            string path = "./testshader.hlsl";
            string shader = File.ReadAllText(path);
            
            analyzer.UpdateShader(shader, path);
            Assembly assembly = analyzer.Compile("main", Profile.PIXEL_SHADER_3_0);


            Debug.Print("\n---------- Assembly ----------");

            foreach (string line in assembly.GetAssemblyLinesEnumerable()) {
                Debug.Print(line);
            }

            Debug.Print("\n---------- Errors ----------");

            foreach (string line in assembly.GetErrorLinesEnumerable()) {
                Debug.Print(line);
            }

            Debug.Print("\n");

            analyzer.Cleanup();
        }
    }
}
