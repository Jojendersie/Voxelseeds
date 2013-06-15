// global constants (global constant buffer)
matrix WorldViewProjection;

struct VS_INPUT
{
    float3 Position_Cube : POSITION_CUBE;
    int Position_Instance : POSITION_INSTANCE;
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    
	float3 worldPos = input.Position_Cube;
	worldPos.x += input.Position_Instance * 5;
    output.Position = mul(float4(worldPos, 1), WorldViewProjection);
    
    return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    return float4(1,0,1,1);
}

technique basic
{
	pass p0
	{
		VertexShader = compile vs_5_0 VS();
		PixelShader = compile ps_5_0 PS();
	}
}