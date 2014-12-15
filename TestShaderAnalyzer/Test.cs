
using System.Diagnostics;
using System.IO;
using SLDE.ShaderAnalyzer;
using SLDE.ShaderAnalyzer.HLSL;

namespace TestShaderAnalyzer
{
    static class Test
    {
        static void Main() {
			SLDE.D3DInterop.D3DInterop.TryCompile(@"
// blah
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 temp;
// TODO: add effect parameters here.
struct VertexShaderInput
{
    float4 Position : POSITION0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};
class MyClass { float a; };
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition : POSITION1;
    worldPosition.xzy = temp;
    worldPosition.w = 0.707;
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    temp = output.Position.xyz;
    // TODO: add your vertex shader code here.
    return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
    return float4(1, 0, 0, 1);
}
technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}

");
            /*Analyzer analyzer = new Analyzer();
            analyzer.options.compatibilityMode = true;

            string path = "./testshader.hlsl";
			string shader = "float4 main(float4 IN : COLOR0) : COLOR { return IN; }"; //File.ReadAllText(path);
            
            analyzer.UpdateShader(shader, path);
            IAssembly assembly = analyzer.Compile("main", Profile.PIXEL_SHADER_3_0);


            Debug.Print("\n---------- Assembly ----------");

            Debug.Print(assembly.GetRawCompilerOutput());

            Debug.Print("\n---------- Errors ----------");

            Debug.Print(assembly.GetRawCompilerErrors());

            Debug.Print("\n");

            analyzer.Cleanup();*/
        }
    }
}
