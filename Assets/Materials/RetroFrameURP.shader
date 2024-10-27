Shader "Custom/RetroFrameURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 1
        _RimPower ("Rim Power", Range(0.1,8.0)) = 3.0
        _ScanlineSpeed ("Scanline Speed", Range(0,10)) = 1
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.5
        _GridSize ("Grid Size", Range(1,100)) = 10
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _EmissionColor;
                float _EmissionIntensity;
                float _RimPower;
                float _ScanlineSpeed;
                float _ScanlineIntensity;
                float _GridSize;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(output.positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Sample base texture
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;

                // Rim effect
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                float rim = 1.0 - saturate(dot(viewDirWS, normalWS));
                rim = pow(rim, _RimPower);

                // Scanline effect
                float scanline = sin(input.positionWS.y * _GridSize + _Time.y * _ScanlineSpeed) * 0.5 + 0.5;
                scanline = lerp(1, scanline, _ScanlineIntensity);

                // Grid pattern
                float2 grid = frac(input.uv * _GridSize);
                float gridEffect = step(0.95, max(grid.x, grid.y));

                // Get main light
                Light mainLight = GetMainLight();
                
                // Calculate final color
                float3 finalColor = baseColor.rgb * mainLight.color;
                float3 emission = _EmissionColor.rgb * _EmissionIntensity * rim * scanline + gridEffect * _EmissionColor.rgb;
                
                return float4(finalColor + emission, baseColor.a);
            }
            ENDHLSL
        }
    }
}