#include "testnestedinclude.hlsl"

sampler2D _FunTex;

float _Scale;

float funify(float2 uv) {
    float4 t = tex2D(_FunTex, uv * initialize(_Scale));

    return dot(t, float4(0.25, 0.5, 0.75, 1.0));
}