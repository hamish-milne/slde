
using System;

namespace SLDE.ShaderAnalyzer.HLSL {
    public class Profile {
        public readonly String signature;
        public readonly String name;

        public static readonly Profile PIXEL_SHADER_2_0 = new Profile("ps_2_0", "Pixel Shader 2.0");
        public static readonly Profile PIXEL_SHADER_3_0 = new Profile("ps_3_0", "Pixel Shader 3.0");
        public static readonly Profile PIXEL_SHADER_4_0 = new Profile("ps_4_0", "Pixel Shader 4.0");
        public static readonly Profile PIXEL_SHADER_4_1 = new Profile("ps_4_1", "Pixel Shader 4.1");
        public static readonly Profile PIXEL_SHADER_5_0 = new Profile("ps_5_0", "Pixel Shader 5.0");

        public static readonly Profile VERTEX_SHADER_2_0 = new Profile("vs_2_0", "Vertex Shader 2.0");
        public static readonly Profile VERTEX_SHADER_3_0 = new Profile("vs_3_0", "Vertex Shader 3.0");
        public static readonly Profile VERTEX_SHADER_4_0 = new Profile("vs_4_0", "Vertex Shader 4.0");
        public static readonly Profile VERTEX_SHADER_4_1 = new Profile("vs_4_1", "Vertex Shader 4.1");
        public static readonly Profile VERTEX_SHADER_5_0 = new Profile("vs_5_0", "Vertex Shader 5.0");

        public static readonly Profile GEOMETRY_SHADER_4_0 = new Profile("gs_4_0", "Geometry Shader 4.0");
        public static readonly Profile GEOMETRY_SHADER_4_1 = new Profile("gs_4_1", "Geometry Shader 4.1");
        public static readonly Profile GEOMETRY_SHADER_5_0 = new Profile("gs_5_0", "Geometry Shader 5.0");

        public static readonly Profile HULL_SHADER_5_0 = new Profile("hs_5_0", "Hull Shader 5.0");

        public static readonly Profile DOMAIN_SHADER_5_0 = new Profile("ds_5_0", "Domain Shader 5.0");

        public static readonly Profile COMPUTE_SHADER_4_0 = new Profile("cs_4_0", "Compute Shader 4.0");
        public static readonly Profile COMPUTE_SHADER_4_1 = new Profile("cs_4_1", "Compute Shader 4.1");
        public static readonly Profile COMPUTE_SHADER_5_0 = new Profile("cs_5_0", "Compute Shader 5.0");
                                                                        
        private Profile(String signature, String name) {
            this.signature = signature;
            this.name = name;
        }

        public override string ToString() {
            return name;
        }
    }
}
