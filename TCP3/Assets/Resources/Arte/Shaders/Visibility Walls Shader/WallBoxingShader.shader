Shader "Custom/WallBoxingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveHeight ("Dissolve Height", Float) = 0
        _DissolveWidth ("Dissolve Width", Float) = 0.1
        _DissolveColor ("Dissolve Color", Color) = (1,0,0,1)
        _NoiseScale ("Noise Scale", Float) = 50
        _GlowIntensity ("Glow Intensity", Float) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        float _DissolveHeight;
        float _DissolveWidth;
        float4 _DissolveColor;
        float _NoiseScale;
        float _GlowIntensity;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
        float4 mod289(float4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
        float4 permute(float4 x) { return mod289(((x*34.0)+1.0)*x); }
        float4 taylorInvSqrt(float4 r) { return 1.79284291400159 - 0.85373472095314 * r; }

        float snoise(float3 v)
        {
            const float2 C = float2(1.0/6.0, 1.0/3.0);
            float3 i  = floor(v + dot(v, C.yyy));
            float3 x0 = v   - i + dot(i, C.xxx);
            float3 g = step(x0.yzx, x0.xyz);
            float3 l = 1.0 - g;
            float3 i1 = min(g.xyz, l.zxy);
            float3 i2 = max(g.xyz, l.zxy);
            float3 x1 = x0 - i1 + C.xxx;
            float3 x2 = x0 - i2 + C.yyy;
            float3 x3 = x0 - 0.5;
            i = mod289(i);
            float4 p = permute(permute(permute(
                      i.z + float4(0.0, i1.z, i2.z, 1.0))
                    + i.y + float4(0.0, i1.y, i2.y, 1.0))
                    + i.x + float4(0.0, i1.x, i2.x, 1.0));
            float4 j = p - 49.0 * floor(p * 0.0039215686);  // mod(p,7*7)
            float4 x_ = floor(j * 0.142857142857);
            float4 y_ = floor(j - 7.0 * x_ );
            float4 x = frac(x_ * 0.142857142857) - 0.5;
            float4 y = frac(y_ * 0.142857142857) - 0.5;
            float4 h = abs(x) + abs(y) - 0.25;
            float4 sx = step(h, float4(0,0,0,0));
            float4 sy = step(h, float4(0,0,0,0));
            float4 s = sx * sy;
            float4 d = x - x_ * 2.0 + y - y_ * 2.0 + 0.5;
            float4 sh = -step(h, float4(0,0,0,0));
            float4 a0 = x + sx;
            float4 a1 = x - sx;
            float4 b0 = y + sy;
            float4 b1 = y - sy;
            float4 h0 = 1.0 - smoothstep(0.0, 1.0, abs(a0) + abs(b0));
            float4 h1 = 1.0 - smoothstep(0.0, 1.0, abs(a1) + abs(b1));
            float4 n = h0 * b0 * a0 + h1 * b1 * a1;
            return dot(n, float4(20,20,20,20));
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;

            float noise = snoise(IN.worldPos * _NoiseScale) * 0.5 + 0.5;
            float dissolveEdge = _DissolveHeight + noise * _DissolveWidth;
            
            if (IN.worldPos.y > dissolveEdge)
            {
                o.Alpha = 0;
            }
            else if (IN.worldPos.y > dissolveEdge - _DissolveWidth)
            {
                float t = (dissolveEdge - IN.worldPos.y) / _DissolveWidth;
                o.Emission = _DissolveColor.rgb * (1-t) * _GlowIntensity;
                o.Alpha = t;
            }
            else
            {
                o.Alpha = 1;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}