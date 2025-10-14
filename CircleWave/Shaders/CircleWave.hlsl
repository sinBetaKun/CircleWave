Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer constants : register(b0)
{
    float amp : packoffset(c0.x);
    float wlen : packoffset(c0.y);
    float phase : packoffset(c0.z);
    float offset : packoffset(c0.w);
    float strd : packoffset(c1.x);
    float cmpl : packoffset(c1.y);
    float x : packoffset(c1.z);
    float y : packoffset(c1.w);
    bool mode : packoffset(c2.x);
    float time : packoffset(c2.y);
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

static const float PI = 3.14159265f;

float completion(float x)
{
    return (x < 0)
            ? 0
            : (x > 1)
                ? 1
                : (-cos(x * PI) / 2 + 0.5);
}

float4 main(
    float4 pos : SV_POSITION,
    float4 posScene : SCENE_POSITION,
    float4 uv0 : TEXCOORD0
) : SV_Target
{
    float radius = length(posScene.xy - float2(x, y));
    float offset2 = (time + offset) / phase * 2 * PI;
    float sub = radius - strd;
    float t2 = (sin((-sub / wlen * 2 * PI) * ((cmpl > 0) ? completion(sub / cmpl) : 1) + offset2) - (mode ? 0 : sin(offset2)));
    float t = amp / 180 * PI * (
        (sub < 0)
            ? (mode ? sin(offset2) : 0)
            : (t2));
    float2 center = uv0.xy - (posScene.xy - float2(x, y)) * uv0.zw;
    float2 uv = center + rotate(uv0.xy - center, t);
    float4 color = (uv.x < 0 || uv.x > 1) && (uv.y < 0 || uv.y > 1) ? float4(0, 0, 0, 0) : InputTexture.Sample(InputSampler, uv.xy);
    return color;
}

/*
コマンド：
cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64
fxc /T ps_4_1 /E main "C:\Users\瀬井 玄\source\repos\CircleWave\CircleWave\Shaders\CircleWave.hlsl" /Fo "C:\Users\瀬井 玄\source\repos\CircleWave\CircleWave\Shaders\CircleWave.cso"
*/ 