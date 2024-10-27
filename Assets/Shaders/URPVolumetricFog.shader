Shader "Custom/URPVolumetricFog"
{
    Properties
    {
        _FogColor("Fog Color", Color) = (0,1,1,0.1)
        _Density("Fog Density", Range(0,1)) = 0.1
        _NoiseScale("Noise Scale", Vector) = (0.1,0.1,0.1,1)
        _NoiseSpeed("Noise Speed", Float) = 0.5
        _FogStart("Fog Start", Float) = 0
        _FogEnd("Fog End", Float) = 10
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "URPFogPass"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float _Density;
                float4 _NoiseScale;
                float _NoiseSpeed;
                float _FogStart;
                float _FogEnd;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Función de ruido simplificada
            float rand(float3 co)
            {
                return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }

            float noise(float3 pos)
            {
                float3 ip = floor(pos);
                float3 fp = frac(pos);
                float4 a = float4(
                    rand(ip),
                    rand(ip + float3(1,0,0)),
                    rand(ip + float3(0,1,0)),
                    rand(ip + float3(1,1,0)));
                    
                fp = fp * fp * (3.0 - 2.0 * fp);
                return lerp(lerp(a.x, a.y, fp.x), lerp(a.z, a.w, fp.x), fp.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Calcular profundidad
                float depth = length(input.positionWS - GetCameraPositionWS());
                
                // Calcular ruido
                float3 noisePos = input.positionWS * _NoiseScale.xyz + _Time.y * _NoiseSpeed;
                float noiseValue = noise(noisePos) * 0.5 + 0.5;
                
                // Factor de niebla
                float fogFactor = saturate((depth - _FogStart) / (_FogEnd - _FogStart));
                fogFactor *= _Density * (1.0 + noiseValue);
                
                // Color final
                half4 finalColor = _FogColor;
                finalColor.a *= fogFactor;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}