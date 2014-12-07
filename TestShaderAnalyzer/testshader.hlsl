#include "./testinclude.hlsl"

float3 _SomeParameter;
float2 _AnotherOne;

struct v2f
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;    
};  

float4 main(v2f i) : COLOR {
	float s = initialize(_SomeParameter.x);

	for (float i = 0; i < 4; i ++) {
		s -= i * _AnotherOne.y - 0.1;
		s = 1.0 / s;
    }
		
    float2 stuff;
    sincos(s, stuff.x, stuff.y);
    float result = funify(stuff);
    clip(result - 0.25);

	return float4(result, 0.0, 0.0, 0.0);
}