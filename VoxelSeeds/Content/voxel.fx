cbuffer GlobalMapInfo : register(b0)
{
	uint3 WorldSize;
	float3 LightDirection;
	float3 Translation;
}

float Transparency;
matrix ViewProjection;
float Ambient;
float ScalingFactor;
float3 CameraPosition;
bool SpecularModifier;

static const float3 LightColor = float3(1.0, 0.98, 0.8);

SamplerState PointSampler;
Texture2D VoxelTexture;

struct VS_INPUT
{
    float3 Position_Cube : POSITION_CUBE;
	float3 Normal : NORMAL;
	float2 Texcoord : TEXCOORD;
    uint Position_Instance : POSITION_INSTANCE;
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
	float Light : DIFFUSE;
	float Specular : SPECULAR;
	float2 Texcoord : TEXCOORD;
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
    
	float3 worldPos = input.Position_Cube * ScalingFactor;
	worldPos += DecodePosition(input.Position_Instance) + Translation;
    output.Position = mul(float4(worldPos, 1), ViewProjection);
    
	// lighting
	float NDotL = dot(LightDirection, input.Normal);
	output.Light = saturate(NDotL) + Ambient;

	// specular
	float3 viewDir = normalize(CameraPosition - worldPos);
	float3 refl = normalize((2 * NDotL) * input.Normal - LightDirection);
	const float SpecularPower = 4.0f;
  	output.Specular = pow(max(dot(refl, viewDir), 0.0), SpecularPower);

	// schlick-fresnel
	float3 halfVector = normalize(viewDir + LightDirection);
	float base = 1.0f - dot(viewDir, halfVector);
	float exponential = pow( base, 10.0);
	float fresnel = exponential + 0.3f * (1.0 - exponential);
	output.Specular *= fresnel;

	output.Texcoord = input.Texcoord;

    return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
	float3 textureColor = VoxelTexture.Sample(PointSampler, input.Texcoord).rgb;
	float specular = input.Specular;
	if(SpecularModifier)
		specular *= saturate(1.0f - dot(textureColor, textureColor)) * 7;

	return float4(textureColor * input.Light * LightColor + 
					specular * LightColor, Transparency);
}


technique basic
{
	pass p0
	{
		VertexShader = compile vs_5_0 VS();
		PixelShader = compile ps_5_0 PS();
	}
}