Shader "Custom/CircuitBackground"
{
    Properties
    {
        _MainTex ("Circuit Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
        _CircuitColor ("Circuit Color", Color) = (0, 0.5, 1, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 1
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float2 _ScrollSpeed;
        float4 _CircuitColor;
        float _EmissionIntensity;
        float _PulseSpeed;
        float _PulseIntensity;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Scroll UV
            float2 scrolledUV = IN.uv_MainTex + _ScrollSpeed * _Time.y;
            
            // Sample texture
            fixed4 c = tex2D (_MainTex, scrolledUV);
            
            // Pulse effect
            float pulse = 1 + sin(_Time.y * _PulseSpeed) * _PulseIntensity;
            
            o.Albedo = c.rgb * _CircuitColor.rgb;
            o.Emission = c.rgb * _CircuitColor.rgb * _EmissionIntensity * pulse;
            o.Metallic = 0;
            o.Smoothness = 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}