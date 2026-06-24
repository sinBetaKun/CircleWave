Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer constants : register(b0)
{
    float amp : packoffset(c0.x);
    float wlen : packoffset(c0.y);
    float offset : packoffset(c0.z);
    float strd : packoffset(c0.w);
    float x : packoffset(c1.x);
    float y : packoffset(c1.y);
    float e : packoffset(c1.z);
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

static const float TAU = 3.14159265f * 2.0f;

float4 main(
    float4 pos : SV_POSITION,
    float4 posScene : SCENE_POSITION,
    float4 uv0 : TEXCOORD0
) : SV_Target
{
    float2 _0 = posScene.xy - float2(x, y);
    float _1 = length(_0);
    float _2 = max(0.0f, _1 - strd);
    float _3 = _2 * amp * sin((offset - pow(_2 / wlen + 1.0f, e)) * TAU);
    float _4 = _3 / _1;
    float2 center = uv0.xy - _0 * uv0.zw;
    float2 uv = center + rotate(_0, _4) * uv0.zw;
    return InputTexture.SampleLevel(InputSampler, uv.xy, 0);
}

/*
コマンド：
cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64
fxc /T ps_4_1 /E main "C:\Users\sinbe\source\repos\CircleWave\CircleWave\Shaders\CircleWave2.hlsl" /Fo "C:\Users\sinbe\source\repos\CircleWave\CircleWave\Shaders\CircleWave2.cso"
*/ 