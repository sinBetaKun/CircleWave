Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer constants : register(b0)
{
    float amp : packoffset(c0.x);
    float wlen : packoffset(c0.y);
    float phase : packoffset(c0.z);
    float strd : packoffset(c0.w);
    float x : packoffset(c1.x);
    float y : packoffset(c1.y);
    bool mode : packoffset(c1.z);
    float time : packoffset(c1.w);
};

float2 rotate(float2 p, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    return float2(
        c * p.x - s * p.y,
        s * p.x + c * p.y
    );
}

static const float Tau = 3.14159265f * 2;

float4 main(
    float4 pos      : SV_POSITION,
    float4 posScene : SCENE_POSITION,
    float4 uv0 : TEXCOORD0
) : SV_Target
{
	float radius = length(posScene.xy - float2(x, y));
    float offset = time / phase * Tau;
	float t = amp * ((radius < strd)
        ? (mode ? 0 : sin(strd / wlen - time*10))
        : sin(radius / wlen - offset)) - (mode ? sin(strd / wlen - offset) : 0);
	float2 center = uv0.xy - (posScene.xy - float2(x, y)) * uv0.zw;
	float2 uv = center + rotate(uv0.xy - center, t);
	float4 color = InputTexture.Sample(InputSampler, uv.xy);
	return color;
}

// コマンド：
// cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64
// fxc /T ps_4_1 /E main "C:\Users\瀬井 玄\source\repos\CircleWave\CircleWave\Shaders\CircleWave.hlsl" /Fo "C:\Users\瀬井 玄\source\repos\CircleWave\CircleWave\Shaders\CircleWave.cso"