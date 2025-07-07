// Shader "Custom/SoilShader"
// {
//     Properties
//     {
//         _Color ("Color", Color) = (1,1,1,1)
//         _MainTex ("Albedo (RGB)", 2D) = "white" {}
//         _Glossiness ("Smoothness", Range(0,1)) = 0.5
//         _Metallic ("Metallic", Range(0,1)) = 0.0
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         LOD 200

//         CGPROGRAM
//         // Physically based Standard lighting model, and enable shadows on all light types
//         #pragma surface surf Standard fullforwardshadows

//         // Use shader model 3.0 target, to get nicer looking lighting
//         #pragma target 3.0

//         sampler2D _MainTex;

//         struct Input
//         {
//             float2 uv_MainTex;
//         };

//         half _Glossiness;
//         half _Metallic;
//         fixed4 _Color;

//         // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
//         // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
//         // #pragma instancing_options assumeuniformscaling
//         UNITY_INSTANCING_BUFFER_START(Props)
//             // put more per-instance properties here
//         UNITY_INSTANCING_BUFFER_END(Props)

//         void surf (Input IN, inout SurfaceOutputStandard o)
//         {
//             // Albedo comes from a texture tinted by color
//             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//             o.Albedo = c.rgb;
//             // Metallic and smoothness come from slider variables
//             o.Metallic = _Metallic;
//             o.Smoothness = _Glossiness;
//             o.Alpha = c.a;
//         }
//         ENDCG
//     }
//     FallBack "Diffuse"
// }

Shader "Custom/SoilShader" {

        Properties {
            _Tess ("Tessellation", Range(1,120)) = 4
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _DispTex ("Disp Texture", 2D) = "gray" {}
            _NormalMap ("Normalmap", 2D) = "bump" {}
            _Displacement ("Displacement", Range(0, 1.0)) = 0.3
            _Color ("Color", color) = (1,1,1,0)
            _SpecColor ("Spec color", color) = (0.5,0.5,0.5,0.5)
            _TesselationMaxDistance ("Tesselation Max Distance", Range(0.1, 20)) = 4.0
        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
            #pragma target 4.6
            #include "Tessellation.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            float _Tess;
            float _TesselationMaxDistance;

            float4 tessDistance (appdata v0, appdata v1, appdata v2) {
                float minDist = 0.1;
                float maxDist = _TesselationMaxDistance;
                return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
            }

            sampler2D _DispTex;
            float4 _DispTex_ST;
            float _Displacement;

            void disp (inout appdata v)
            {
                float2 uv = v.texcoord.xy * _DispTex_ST.xy * _DispTex_ST.zw;
                float d = tex2Dlod(_DispTex, float4(uv,0,0)).r * _Displacement;
                v.vertex.xyz += v.normal * d;
            }

            struct Input {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            fixed4 _Color;

            void surf (Input IN, inout SurfaceOutput o) {
                half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Specular = 0.2;
                o.Gloss = 1.0;
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
            }
            ENDCG
        }
        FallBack "Diffuse"
    }
