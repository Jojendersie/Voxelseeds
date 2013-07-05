matrix InverseViewProjection;
float f;

struct VS_INPUT
{
	float2 DeviceCor : POSITION;
};

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	float3 ViewDirection : VIEW;
};

PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT output;

	output.Position = float4(input.DeviceCor, 0.9999, 1);

	float4 rayOrigin = mul(float4(input.DeviceCor, 0, 1), InverseViewProjection);
	rayOrigin.xyz /= rayOrigin.w;
	float4 rayTarget = mul(output.Position, InverseViewProjection);
	rayTarget.xyz /= rayTarget.w;
	output.ViewDirection = (rayTarget.xyz - rayOrigin.xyz);

	return output;
}



static const float3 AdditonalSunColor = float3(1.0, 0.98, 0.8)/3;
static const float3 LowerHorizonColour = float3(0.815, 1.141, 1.54)/2;
static const float3 UpperHorizonColour = float3(0.986, 1.689, 2.845)/2;
static const float3 UpperSkyColour = float3(0.16, 0.27, 0.43)*0.8;
static const float3 GroundColour = float3(0.31, 0.41, 0.5)*0.8;
static const float LowerHorizonHeight = -0.4;
static const float UpperHorizonHeight = -0.1;
static const float SunAttenuation = 2;

float3 LightDirection;

float4 PS(PS_INPUT input) : SV_Target
{
	float3 color;

	float3 ray = normalize(input.ViewDirection);

	// background
	float heightValue = ray.y;	// mirror..
	if(heightValue < LowerHorizonHeight)
		color = lerp(GroundColour, LowerHorizonColour, (heightValue+1) / (LowerHorizonHeight+1));
	else if(heightValue < UpperHorizonHeight)
		color = lerp(LowerHorizonColour, UpperHorizonColour, (heightValue-LowerHorizonHeight) / (UpperHorizonHeight - LowerHorizonHeight));
	else
		color = lerp(UpperHorizonColour, UpperSkyColour, (heightValue-UpperHorizonHeight) / (1.0-UpperHorizonHeight));
	
	// Sun
	float angle = max(0, dot(ray, LightDirection));
	color += (pow(angle, SunAttenuation) + pow(angle, 10000)*10) * AdditonalSunColor;

	return float4(color, 1.0f);
}


technique basic
{
	pass p0
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PS();
	}
}
