Shader "Custom/TronGrid"
{
    Properties
    {
        _GridSize ("Grid Size", Float) = 1.0
        _GlowIntensity ("Glow Intensity", Float) = 1.5
        _GridColor ("Grid Color", Color) = (0,1,1,1)
        _WaveOffset ("Wave Offset", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            float _GridSize;
            float _GlowIntensity;
            float4 _GridColor;
            float _WaveOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 grid = frac(i.uv * _GridSize + _WaveOffset);
                float2 lines = 1 - step(0.02, grid) * step(0.02, 1 - grid);
                float brightness = (lines.x + lines.y) * _GlowIntensity;
                
                return fixed4(_GridColor.rgb, brightness);
            }
            ENDCG
        }
    }
}
