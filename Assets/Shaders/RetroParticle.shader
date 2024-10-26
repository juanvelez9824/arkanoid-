Shader "Custom/RetroParticle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 2
        _FlickerSpeed ("Flicker Speed", Range(0,50)) = 10
        _FlickerIntensity ("Flicker Intensity", Range(0,1)) = 0.1
        _ScanlineSpeed ("Scanline Speed", Range(0,50)) = 10
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.1
        _ScanlineCount ("Scanline Count", Range(0,100)) = 30
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend One One
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GlowIntensity;
            float _FlickerSpeed;
            float _FlickerIntensity;
            float _ScanlineSpeed;
            float _ScanlineIntensity;
            float _ScanlineCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base texture and color
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
                
                // Flicker effect
                float flicker = 1 + _FlickerIntensity * sin(_Time.y * _FlickerSpeed);
                
                // Scanline effect
                float scanline = sin(i.uv.y * _ScanlineCount + _Time.y * _ScanlineSpeed) * 0.5 + 0.5;
                scanline = lerp(1, scanline, _ScanlineIntensity);
                
                // Combine effects
                col *= flicker * scanline * _GlowIntensity;
                
                return col;
            }
            ENDCG
        }
    }
}
