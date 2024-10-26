Shader "Custom/RetroGridBackground"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _SecondaryColor ("Secondary Color", Color) = (0,0,0,1)
        _GridSize ("Grid Size", Float) = 10
        _LineWidth ("Line Width", Range(0, 1)) = 0.1
        _ScrollSpeed ("Scroll Speed", Vector) = (0, 1, 0, 0)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1
        _GridTexture ("Grid Texture", 2D) = "white" {}
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            float4 _MainColor;
            float4 _SecondaryColor;
            float _GridSize;
            float _LineWidth;
            float4 _ScrollSpeed;
            float _EmissionIntensity;
            sampler2D _GridTexture;
            float4 _GridTexture_ST;
            sampler2D _NoiseTexture;
            float _NoiseIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _GridTexture);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float2 grid(float2 uv)
            {
                float2 grid = frac(uv * _GridSize);
                float2 lines = smoothstep(_LineWidth, 0., abs(grid - 0.5));
                return lines.x + lines.y;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scrolling UV
                float2 scrolledUV = i.uv + _Time.y * _ScrollSpeed.xy;
                
                // Base grid
                float gridValue = grid(scrolledUV);
                
                // Noise
                float2 noiseUV = i.uv * 0.5;
                float noise = tex2D(_NoiseTexture, noiseUV + _Time.y * 0.1).r;
                
                // Combine grid and noise
                float finalValue = gridValue + (noise * _NoiseIntensity);
                
                // Color gradient
                float gradientValue = i.uv.y;
                float4 gradientColor = lerp(_SecondaryColor, _MainColor, gradientValue);
                
                // Final color
                float4 finalColor = gradientColor * finalValue * _EmissionIntensity;
                finalColor.a = 1;
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}