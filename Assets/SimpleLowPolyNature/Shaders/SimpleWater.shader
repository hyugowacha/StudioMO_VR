Shader "LowPoly/SimpleWater"
{
    Properties
    {
        _WaterNormal("Water Normal", 2D) = "bump" {}
        _NormalScale("Normal Scale", Float) = 1
        _DeepColor("Deep Color", Color) = (0,0.2,0.5,1)
        _ShalowColor("Shallow Color", Color) = (0.5,0.8,1,1)
        _WaterDepth("Water Depth", Float) = 1
        _WaterFalloff("Water Falloff", Float) = 1
        _Distortion("Distortion", Float) = 0.05
        _Foam("Foam", 2D) = "white" {}
        _FoamDepth("Foam Depth", Float) = 1
        _FoamFalloff("Foam Falloff", Float) = 1
        _WaterSpecular("Water Specular", Float) = 0.5
        _WaterSmoothness("Water Smoothness", Float) = 0.5
        _FoamSpecular("Foam Specular", Float) = 0.2
        _FoamSmoothness("Foam Smoothness", Float) = 0.2
        _WavesAmplitude("WavesAmplitude", Float) = 0.01
        _WavesAmount("WavesAmount", Float) = 8.87
        _CameraOpaqueTexture("Opaque Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float4 screenPos   : TEXCOORD3;
            };

            sampler2D _WaterNormal;
            float4 _WaterNormal_ST;
            float _NormalScale;
            float4 _DeepColor;
            float4 _ShalowColor;
            float _WaterDepth;
            float _WaterFalloff;
            sampler2D _Foam;
            float4 _Foam_ST;
            float _FoamDepth;
            float _FoamFalloff;
            float _WaterSpecular;
            float _WaterSmoothness;
            float _FoamSpecular;
            float _FoamSmoothness;
            float _Distortion;
            float _WavesAmplitude;
            float _WavesAmount;

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);

                float wave = sin(_WavesAmount * worldPos.z + _Time.y) * _WavesAmplitude;
                worldPos += v.normalOS * wave;

                o.worldPos = worldPos;
                o.worldNormal = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _WaterNormal);
                float4 clipPos = TransformWorldToHClip(worldPos);
                o.positionHCS = clipPos;
                o.screenPos = ComputeScreenPos(clipPos);
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float3 normal1 = UnpackNormal(tex2D(_WaterNormal, i.uv + float2(-0.03, 0) * _Time.y));
                float3 normal2 = UnpackNormal(tex2D(_WaterNormal, i.uv + float2(0.04, 0.04) * _Time.y));
                float3 normal = normalize(normal1 + normal2);
                normal.xy *= _NormalScale;

                float depthScene = SampleSceneDepth(i.screenPos.xy / i.screenPos.w);
                float depthWorld = LinearEyeDepth(depthScene, _ZBufferParams);
                float surfaceDepth = i.screenPos.w;
                float depthDiff = abs(depthWorld - surfaceDepth);

                float depthLerp = saturate(pow(depthDiff + _WaterDepth, _WaterFalloff));
                float4 baseColor = lerp(_DeepColor, _ShalowColor, depthLerp);

                float2 foamUV = TRANSFORM_TEX(i.uv, _Foam);
                float foamNoise = tex2D(_Foam, foamUV + float2(-0.01, 0.01) * _Time.y).r;
                float foamFactor = saturate(pow(depthDiff + _FoamDepth, _FoamFalloff)) * foamNoise;
                float4 colorWithFoam = lerp(baseColor, float4(1,1,1,1), foamFactor);

                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float2 distortionUV = screenUV + normal.xy * _Distortion;
                float4 backgroundColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, distortionUV);

                float4 finalColor = lerp(colorWithFoam, backgroundColor, depthLerp);

                    // Directional Light 유무에 따른 밝기 보정
    Light mainLight = GetMainLight();
    float hasLight = mainLight.distanceAttenuation; // Directional Light 없으면 0
    float brightness = lerp(0.3, 1.0, hasLight);     // 없으면 어둡게, 있으면 밝게
    finalColor.rgb *= brightness;

                finalColor.a = 1;

                return finalColor;
            }

            ENDHLSL
        }
    }

    FallBack Off
}