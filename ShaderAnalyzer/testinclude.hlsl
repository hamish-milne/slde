sampler2D _FunTex;

float funify(float2 uv) {
    float4 t = tex2D(_FunTex, uv);


    return dot(t, float4(0.25, 0.5, 0.75, 1.0));
}