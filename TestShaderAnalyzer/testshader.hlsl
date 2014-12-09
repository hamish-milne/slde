#include "./testinclude.hlsl"

float3 _SomeParameter;
int2 _AnotherOne;
float _AnArray[16];

struct v2f
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0; 
	float2 uv2 : TEXCOORD1;
};  

float4 main(v2f IN) : COLOR {
	float s = initialize(_SomeParameter.x);

	for (float i = 0; i < 16; i ++) {
		s -= i * _AnotherOne.y - 0.1;
		s = _AnArray[i] / s;
    }
		
    float2 stuff;
    sincos(s, stuff.x, stuff.y);
    float result = funify(stuff + IN.uv);
    clip(result - 0.25);

	return float4(result, 0.0, 0.0, 0.0);
}