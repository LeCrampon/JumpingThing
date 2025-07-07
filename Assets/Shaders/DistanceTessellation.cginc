// DistanceTessellation.cginc

#ifndef DISTANCE_TESSELLATION_INCLUDED
#define DISTANCE_TESSELLATION_INCLUDED

// Fonction de tesselation dépendante de la distance caméra
float4 TessDistance(float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tessFactor)
{
    // Utilise la position du triangle pour calculer la distance à la caméra
    float3 camPos = _WorldSpaceCameraPos;
    float3 triCenter = (v0.xyz + v1.xyz + v2.xyz) / 3.0;
    float dist = distance(camPos, triCenter);

    // Interpolation linéaire du facteur de tesselation selon la distance
    float tess = lerp(tessFactor, 1.0, saturate((dist - minDist) / (maxDist - minDist)));
    return tess;
}

// Déplacement des sommets selon une texture de displacement
void ApplyDisplacement(inout float4 vertex, float3 normal, float2 uv, sampler2D dispTex, float displacement)
{
    float d = tex2Dlod(dispTex, float4(uv, 0, 0)).r * displacement;
    vertex.xyz += normal * d;
}

#endif // DISTANCE_TESSELLATION_INCLUDED