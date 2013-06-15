cbuffer GlobalMapInfo : register(b0)
{
	int3 WorldSize;
	float3 LightDirection;
}

matrix WorldViewProjection;
const float Ambient = 0.3f;

struct VS_INPUT
{
    float3 Position_Cube : POSITION_CUBE;
	float3 Normal : NORMAL;
	float2 Texcoord : TEXCOORD;
    int Position_Instance : POSITION_INSTANCE;
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
	float Light : LIGHT;
};

float3 DecodePosition(int code)
{
	float3 o;
	o.x = code % WorldSize.x;
    o.y = (code / WorldSize.x) % WorldSize.y;
    o.z =  code / (WorldSize.x * WorldSize.y);
	return o;
}

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    
	float3 worldPos = input.Position_Cube;
	worldPos += DecodePosition(input.Position_Instance);
    output.Position = mul(float4(worldPos, 1), WorldViewProjection);
    
	output.Light = saturate(dot(LightDirection, input.Normal)) + Ambient;

    return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    return float4(input.Light,input.Light,input.Light,1);
}

technique basic
{
	pass p0
	{
		VertexShader = compile vs_5_0 VS();
		PixelShader = compile ps_5_0 PS();
	}
}