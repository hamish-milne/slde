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

struct v2f2
{
	float4 pos : SV_POSITION;
	float4 v1 : texcoord1;
	float4 v2 : texcoord2;
	float4 v3 : texcoord3;
	float4 v4 : texcoord4;

};

float4 main(v2f IN) : COLOR {
	float s = initialize(_SomeParameter.x);

	for (float i = 0; i < 16; i ++) {
		s += i * _AnotherOne.y + 0.1;
		s = _AnArray[i] / -s;
    }
		
    float2 stuff;
    sincos(s, stuff.x, stuff.y);
    float result = funify(stuff + IN.uv);
    clip(result - 0.25);

	return float4(result, 0.0, 0.0, 0.0);
}

float4 main2(v2f2 IN) : COLOR {
	float4 o = IN.v2;
	o.xz *= IN.v3.zw;
	o.ywz *= IN.v4.xxy;
	o.x *= IN.v1.z;
	float4 o2 = IN.v3 + IN.v1.zzzz;
	return o * float4(-abs(o2.xy) / o2.zw, dot(IN.v3, IN.v4), 1);
}