Shader "Custom/WaterShader"
{
    Properties
    {
        _Color ("Water Color", Color) = (0.2, 0.6, 0.9, 0.7)
        _MainTex ("Noise Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 0.1
        _WaveHeight ("Wave Height", Float) = 0.05
        _PlayerPos ("Player Position", Vector) = (0, 0, 0, 0)
        _WaveRadius ("Wave Radius", Float) = 1.0

        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularIntensity ("Specular Intensity", Float) = 1.0
        _Shininess ("Shininess", Float) = 50.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                UNITY_FOG_COORDS(1)
            };

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveSpeed;
            float _WaveHeight;
            float4 _PlayerPos;
            float _WaveRadius;

            float _Shininess;
            float4 _SpecularColor;
            float _SpecularIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                float dist = distance(v.vertex.xz, _PlayerPos.xz);
                float wave = smoothstep(_WaveRadius, 0, dist);
                float time = _Time.y * _WaveSpeed;
                float noise = tex2Dlod(_MainTex, float4(v.uv * _MainTex_ST.xy + time, 0, 0)).r;


                v.vertex.y += (noise + wave) * _WaveHeight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.vertex.xyz);
                float3 halfDir = normalize(lightDir + viewDir);
                float specular = pow(saturate(dot(i.normal, halfDir)), _Shininess);

                fixed4 col = _Color;
                col.rgb += specular * _SpecularColor.rgb * _SpecularIntensity;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}