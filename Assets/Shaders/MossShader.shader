Shader "Custom/MossShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Tess ("Tessellation", Range(1,120)) = 4
        _DispTex ("Disp Texture", 2D) = "gray" {}
        _Displacement ("Displacement", Range(0, 1.0)) = 0.3

        _SSSColor ("SSS Color", Color) = (1,0.9,0.7,1)
        _SSSIntensity ("SSS Intensity", Range(0,1)) = 0.5
        _SSSPower ("SSS Power", Range(1, 10)) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf SSSLighting fullforwardshadows vertex:disp tessellate:tessDistance

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #include "Tessellation.cginc"
        sampler2D _MainTex;
        fixed4 _SSSColor;
        float _SSSIntensity;
        float _SSSPower;
        
        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

        float _Tess;

        float4 tessDistance (appdata v0, appdata v1, appdata v2) {
            float minDist = 0.1;
            float maxDist = 4.0;
            return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
        }

        sampler2D _DispTex;
        float _Displacement;

        void disp (inout appdata v)
        {
            float d = tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r * _Displacement;
            v.vertex.xyz += v.normal * d;
        }

       struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
        }

        half4 LightingSSSLighting(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {
            // Standard diffuse
            half NdotL = saturate(dot(s.Normal, lightDir));
            half3 diffuse = _LightColor0.rgb * s.Albedo * NdotL * atten;

            // SSS fake: dépend de l'angle vue/lumière
            half VdotL = saturate(dot(viewDir, -lightDir));
            half sss = pow(VdotL, _SSSPower) * _SSSIntensity;
            half3 sssColor = _SSSColor.rgb * sss * atten;

            return half4(diffuse + sssColor, 1.0);
        }
        ENDCG
    }
    Fallback "Diffuse"
}