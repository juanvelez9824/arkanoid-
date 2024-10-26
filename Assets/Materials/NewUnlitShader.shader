Shader "Custom/RetroParticleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowPower ("Glow Power", Range(0.0, 10.0)) = 2.0
        _PixelSize ("Pixel Size", Range(0.001, 0.1)) = 0.01
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 2
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowPower;
            float _PixelSize;
            float _EmissionIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Pixelizar UVs
                float2 pixelizedUV = round(i.uv / _PixelSize) * _PixelSize;
                
                // Sample texture con UVs pixelizadas
                float4 col = tex2D(_MainTex, pixelizedUV);
                
                // Aplicar color de partícula
                col *= i.color;
                
                // Aplicar efecto glow
                float luminance = dot(col.rgb, float3(0.2126, 0.7152, 0.0722));
                float3 glow = _GlowColor.rgb * pow(luminance, _GlowPower);
                
                // Combinar color base con glow y emisión
                col.rgb = lerp(col.rgb, glow, 0.5) * _EmissionIntensity;
                
                return col;
            }
            ENDCG
        }
    }
}

