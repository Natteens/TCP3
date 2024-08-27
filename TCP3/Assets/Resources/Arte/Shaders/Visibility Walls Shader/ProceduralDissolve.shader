Shader "Custom/ProceduralDissolve"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveHeight ("Dissolve Height", Float) = 0
        _DissolveLimit ("Dissolve Limit", Range(0, 1)) = 1
        [HDR] _DissolveColor ("Dissolve Color", Color) = (2,0,0,1)
        _DissolveIntensity ("Dissolve Intensity", Range(1, 10)) = 2
        _NoiseScale ("Noise Scale", Float) = 10
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
        _EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.05
        _EdgeColorWidth ("Edge Color Width", Range(0, 0.2)) = 0.02
        _EdgeIntensity ("Edge Intensity", Range(1, 10)) = 5
        _EdgeSharpness ("Edge Sharpness", Range(1, 50)) = 10
        _FresnelStrength ("Fresnel Strength", Range(0, 5)) = 1
        _DissolveDirection ("Dissolve Direction", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry"}

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS               : SV_POSITION;
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Smoothness;
                float _Metallic;
                float _DissolveAmount;
                float _DissolveHeight;
                float _DissolveLimit;
                float4 _DissolveColor;
                float _DissolveIntensity;
                float _NoiseScale;
                float _NoiseStrength;
                float _EdgeWidth;
                float _EdgeColorWidth;
                float _EdgeIntensity;
                float _EdgeSharpness;
                float _FresnelStrength;
                float4 _DissolveDirection;
            CBUFFER_END

            float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 mod289(float4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 permute(float4 x) { return mod289(((x*34.0)+1.0)*x); }

            float noise(float3 p)
            {
                float3 a = floor(p);
                float3 d = p - a;
                d = d * d * (3.0 - 2.0 * d);

                float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
                float4 k1 = permute(b.xyxy);
                float4 k2 = permute(k1.xyxy + b.zzww);

                float4 c = k2 + a.zzzz;
                float4 k3 = permute(c);
                float4 k4 = permute(c + 1.0);

                float4 o1 = frac(k3 * (1.0 / 41.0));
                float4 o2 = frac(k4 * (1.0 / 41.0));

                float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
                float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

                return o4.y * d.y + o4.x * (1.0 - d.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float4 color = baseMap * _BaseColor;
                
                float noiseValue = noise(IN.positionWS * _NoiseScale) * _NoiseStrength;

                float3 dissolveDirection = normalize(_DissolveDirection.xyz);
                float dissolveThreshold = dot(IN.positionWS - _DissolveHeight * dissolveDirection, dissolveDirection) / _DissolveLimit;
                float dissolve = dissolveThreshold - _DissolveAmount + (noiseValue * 0.5);
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float fresnelFactor = pow(1.0 - saturate(dot(normalize(IN.normalWS), viewDir)), _FresnelStrength);
                
                float edgeWidth = _EdgeWidth + _EdgeColorWidth;
                float edge = 1 - saturate(dissolve / edgeWidth);
                float colorEdge = 1 - saturate(dissolve / _EdgeColorWidth);
                
                float edgeFactor = max(edge, fresnelFactor * edge);
                float colorEdgeFactor = max(colorEdge, fresnelFactor * colorEdge);
                
                float3 dissolveEdgeColor = _DissolveColor.rgb * (colorEdgeFactor * _EdgeIntensity);
                color.rgb = lerp(color.rgb, dissolveEdgeColor, saturate(colorEdgeFactor));
                
                clip(dissolve);
                
                InputData inputData;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalize(IN.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                inputData.fogCoord = 0;
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = SampleSH(inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                
                half4 finalColor = UniversalFragmentPBR(inputData, color.rgb, _Metallic, _Smoothness, 0.0h, 0.0h, 1.0h, color.a);
                
                finalColor.rgb += dissolveEdgeColor * edgeFactor * _DissolveIntensity;
                
                return finalColor;
            }

            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }
}