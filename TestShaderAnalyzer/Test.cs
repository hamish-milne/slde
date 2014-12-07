
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


            analyzer.Cleanup();
        }
    }
}
