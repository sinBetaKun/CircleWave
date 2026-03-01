Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer constants : register(b0)
{
    float amp : packoffset(c0.x);
    int count : packoffset(c0.y);
    float angle : packoffset(c0.z);
    float x : packoffset(c0.w);
    float y : packoffset(c1.x);
};

static const float PI = 3.14159265f;

float4 main(
    float4 pos : SV_POSITION,
    float4 posScene : SCENE_POSITION,
    float4 uv0 : TEXCOORD0
) : SV_Target
{
    float2 center = uv0.xy - (posScene.xy - float2(x, y)) * uv0.zw;
    float2 dv = posScene.xy - float2(x, y);
    float divisor = 2 * PI / count;
    return InputTexture.Sample(
        InputSampler,
        center + (
            dv / (1 + 1e-6 - amp / 100 * cos(fmod(fmod(atan2(dv.y, dv.x) - angle / 180 * PI, divisor) + divisor, divisor) * count))
        ) * uv0.zw, 0);
}

/*
コマンド：
cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64
fxc /T ps_4_1 /E main "C:\Users\瀬井 玄\source\repos\CircleWave\CircleWave\Shaders\RadialWave.hlsl" /Fo "C:\Users\瀬井 玄\source\repos\CircleWave\CircleWave\Shaders\RadialWave.cso"
*/ 