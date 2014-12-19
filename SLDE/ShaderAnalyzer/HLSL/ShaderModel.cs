
using System;

namespace SLDE.ShaderAnalyzer.HLSL {
    public class ShaderModel {
        public readonly string signature;
        public readonly string name;

        public static readonly ShaderModel PIXEL_SHADER_2_0 = new ShaderModel("ps_2_0", "Pixel Shader 2.0");
        public static readonly ShaderModel PIXEL_SHADER_3_0 = new ShaderModel("ps_3_0", "Pixel Shader 3.0");
        public static readonly ShaderModel PIXEL_SHADER_4_0 = new ShaderModel("ps_4_0", "Pixel Shader 4.0");
        public static readonly ShaderModel PIXEL_SHADER_4_1 = new ShaderModel("ps_4_1", "Pixel Shader 4.1");
        public static readonly ShaderModel PIXEL_SHADER_5_0 = new ShaderModel("ps_5_0", "Pixel Shader 5.0");

        public static readonly ShaderModel VERTEX_SHADER_2_0 = new ShaderModel("vs_2_0", "Vertex Shader 2.0");
        public static readonly ShaderModel VERTEX_SHADER_3_0 = new ShaderModel("vs_3_0", "Vertex Shader 3.0");
        public static readonly ShaderModel VERTEX_SHADER_4_0 = new ShaderModel("vs_4_0", "Vertex Shader 4.0");
        public static readonly ShaderModel VERTEX_SHADER_4_1 = new ShaderModel("vs_4_1", "Vertex Shader 4.1");
        public static readonly ShaderModel VERTEX_SHADER_5_0 = new ShaderModel("vs_5_0", "Vertex Shader 5.0");

        public static readonly ShaderModel GEOMETRY_SHADER_4_0 = new ShaderModel("gs_4_0", "Geometry Shader 4.0");
        public static readonly ShaderModel GEOMETRY_SHADER_4_1 = new ShaderModel("gs_4_1", "Geometry Shader 4.1");
        public static readonly ShaderModel GEOMETRY_SHADER_5_0 = new ShaderModel("gs_5_0", "Geometry Shader 5.0");

        public static readonly ShaderModel HULL_SHADER_5_0 = new ShaderModel("hs_5_0", "Hull Shader 5.0");

        public static readonly ShaderModel DOMAIN_SHADER_5_0 = new ShaderModel("ds_5_0", "Domain Shader 5.0");

        public static readonly ShaderModel COMPUTE_SHADER_4_0 = new ShaderModel("cs_4_0", "Compute Shader 4.0");
        public static readonly ShaderModel COMPUTE_SHADER_4_1 = new ShaderModel("cs_4_1", "Compute Shader 4.1");
        public static readonly ShaderModel COMPUTE_SHADER_5_0 = new ShaderModel("cs_5_0", "Compute Shader 5.0");
                                                                        
        private ShaderModel(string signature, string name) {
            this.signature = signature;
            this.name = name;
        }

        public override string ToString() {
            return name;
        }
    }
}
