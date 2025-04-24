Shader "Roystan/Grass"
{
    Properties
    {
		[Header(Shading)]
		_TopColor("Top Col0or", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
		_BendRotationRandom("Bend Rotation Random", Range(0,1)) = 0.2
		_BladeWidth("Blade Width", Float) = 0.05
		_BladeWidthRandom("Blade Width Random", Float) = 0.02
		_BladeHeight("Blade Height", Float) = 0.5
		_BladeHeightRandom("Blade Height Random", Float) = 0.3
		_TessellationUniform("Tessellation Uniform", Range(1,64)) = 1
		//Gestion du vent
		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		_WindStrength("Wind Strength", Float) = 1
		//Gestion de la courbe
		_BladeForward("Blade Forward Amount", Float) = 0.38
		_BladeCurve("Blade Curve Amount", Range(1,4)) = 2
		//TRAMPLE
		_Trample("Trample", Vector) = (0.05,0.05,0,0)
		_TrampleStrength("Trample Strength", Range(0,1)) = 0.1
	
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"
	#include "CustomTessellation.cginc"
	
	//Pour donner le contrôle au dev du nombre de segments on veut sur notre herbe
	#define BLADE_SEGMENTS 3

	//rotation
	float _BendRotationRandom;
	//size variation
	float _BladeHeight;
	float _BladeHeightRandom;
	float _BladeWidth;
	float _BladeWidthRandom;
	//Wind
	sampler2D _WindDistortionMap;
	float4 _WindDistortionMap_ST;
	float2 _WindFrequency;
	float _WindStrength;
	//Curve
	float _BladeForward;
	float _BladeCurve;

	//Trample
	float4 _Trample;
	float _TrampleStrength;


	//notre structure de retour pour le geometryshader
	struct geometryOutput {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		//ScreenSpace UV  for shadows ??? THE NAME MUST BE _ShadowCoord FOR UNITY
		unityShadowCoord4 _ShadowCoord : TEXCOORD1;
		//normale pour le lighting
		float3 normal : NORMAL;
	};


	//Simple fonction pour convertir en clip space
	geometryOutput VertexOutput(float3 pos, float2 uv, float3 normal)
	{
		geometryOutput o;
		o.pos = UnityObjectToClipPos(pos);
		o.uv = uv;
		//ScreenPos for Shadow Coord
		o._ShadowCoord = ComputeScreenPos(o.pos);

		//SI ON EST DANS UNE PASS SHADOWCASTER
		#if UNITY_PASS_SHADOWCASTER
			//pour éviter les artefacts, comme ça chaque brin ne cast pas d'ombre sur lui même
			o.pos = UnityApplyLinearShadowBias(o.pos);
		#endif

		//Normale de local à World pour le lighting
		o.normal = UnityObjectToWorldNormal(normal);

		return o;
	}

	


	//La fonction rand() utilise la position pour que la seed du random reste la même selon les frames

	// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
	// Extended discussion on this function can be found at the following link:
	// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
	// Returns a number in the 0...1 range.
	float rand(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
	}

	// Construct a rotation matrix that rotates around the provided axis, sourced from:
	// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
	float3x3 AngleAxis3x3(float angle, float3 axis)
	{
		float c, s;
		sincos(angle, s, c);

		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c
			);
	}

	//Fonction de création de vertex de l'herbe
	geometryOutput GenerateGrassVertex(float3 vertexPosition, 
										float width, 
										float height, 
										float forward,
										float2 uv, 
										float3x3 transformMatrix) 
	{
		//Le forward sert à décaler le vertex en y pour la curve
		float3 tangentPoint = float3(width, forward, height);

		//Calcul de la normale pour le lighting
		//en espace tangent, décalé par le forward
		float3 tangentNormal = normalize(float3(0, -1, forward));
		//en espace local
		float3 localNormal = mul(transformMatrix, tangentNormal);

		float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
		return VertexOutput(localPosition, uv, localNormal);
	}

	float4 GetTrampleVector(float3 pos, float4 objectOrigin)
	{
		float3 trampleDiff = pos - (_Trample.xyz - objectOrigin);
		return float4(float3(normalize(trampleDiff).x,
							0,
							normalize(trampleDiff).z) * (1.0 - saturate(length(trampleDiff) / _Trample.w)),
					  0);
	}

	// C'EST NOTRE GEOMETRY SHADER
	//Maximum de vertex = nombre de segments *2 +1
	[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
	void geo(triangle vertexOutput IN[3], inout TriangleStream<geometryOutput> triStream) {
		//Creer output
		geometryOutput o;

		//Recupérer l'input (position du vertes)
		float3 pos = IN[0].vertex;
		float3 vNormal = IN[0].normal;
		float4 vTangent = IN[0].tangent;
		float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

		//matrice de tangente
		float3x3 tangentToLocal = float3x3(
			vTangent.x, vBinormal.x, vNormal.x,
			vTangent.y, vBinormal.y, vNormal.y,
			vTangent.z, vBinormal.z, vNormal.z);

		//Créer matrice de rotation random pour chaque brin d'herbe, sur l'axe z (up, en tangentSpace)
		float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
		//Créer matrice de rotation(bend) avec un random range de 0 à 90° (PI * 0.5)
		float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

		//GENERATION DES UV DE VENT
		float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;

		//Valeur entre -1 et 1 * WindStrength
		float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1)* _WindStrength;
		//On transforme en vec3 avec z (height en tangentspace) à 0
		float3 wind = normalize(float3(windSample.x, windSample.y, 0));
		//On en fait encore une matrice de rotation le long de l'axe du vent
		float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

		//On multiplie au windRotation
		//ON multiplie au tangenteToLocal pour appliquer les deux rotations.
		// Puis on multiplie par la matrice de bend
		//ATTENTION , IL Y A UN SENS
		float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);

		//MATRICE pour faire en sorte que le bas de l'herbe reste collé au sol
		float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);


		//Récupération des valeurs de dimensions de la grass
		float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;

		//gestion de la courbe
		float forward = rand(pos.yyz) * _BladeForward;

		float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));

		//Maintenant, on créé deux points par segment d'herbe
		//G foiré mon for, lol
		for(int i =0; i < BLADE_SEGMENTS; i++)
		{
			//la variable t definit à quel "pourcentage" de la hauteur de l'herbe on est (entre 0 et 1)
			float t = i / (float)BLADE_SEGMENTS;

			float segmentHeight = height * t;
			float segmentWidth = width * (1 - t);
			//le décalage pour la courbe
			float segmentForward = pow(t, _BladeCurve) * forward;

			//Quelle matrice appliquer? Si on est au sol, transformationMAtrixFacing, sinon, transformationMatrix
			float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

			//GESTION DU TRAMPLE
			float4 trample = GetTrampleVector(pos, objectOrigin);
			pos += trample * _TrampleStrength;
			//GESTION DU Trample

			//Set the position converted to clip space
		//Add to the Triangle Stream en multipliant par la matrice de tangente
			triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
			//Do it two more times
			triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
		}

		//GESTION DU TRAMPLE
		float4 trample = GetTrampleVector(pos, objectOrigin);
		pos += trample * _TrampleStrength;
		//GESTION DU Trample

		//Une fois qu'on a fait les segments, on ajoute le dernier point (la pointe de l'herbe)
		//Pour le haut du triangle, on utilise la matrice qui prend toutes les rotations (dont le vent et le bend)
		triStream.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));

	}

	ENDCG

    SubShader
    {
		Cull Off

        Pass
        {
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma geometry geo
			#pragma target 4.6
			// pour compiler les variantes du shader
			#pragma multi_compile_fwdbase
			#pragma hull hull
			#pragma domain domain
            
			#include "Lighting.cginc"

			float4 _TopColor;
			float4 _BottomColor;
			float _TranslucentGain;

			float4 frag(geometryOutput i, fixed facing : VFACE) : SV_Target
			{
				//Normale selon si facing ou pas
				float3 normal = facing > 0 ? i.normal : -i.normal;

				//GESTION DE LA LUMIERE
				//Fonction de unity pour générer la reception d'ombre
				float shadow = SHADOW_ATTENUATION(i);
				//Calcul de la valeur de lumière Générale (NdotL)
				float NdotL = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
				//I HAVE NO IDEA
				float3 ambient = ShadeSH9(float4(normal, 1));
				//Multiplie par la couleur, ajoute l'ambient
				float4 lightIntensity = NdotL * _LightColor0 + float4(ambient, 1);

			

				//Calcul de la couleur: si on est en bas: bottomColor. puis, degradé selon la hauteur * l'intensité
				float4 col = lerp(_BottomColor, _TopColor * lightIntensity, i.uv.y) ;

				return col;
            }
            ENDCG
        }

		//Pass de shadow cast
		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			
			float4 frag(geometryOutput i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i);
			}

			ENDCG
		}
    }
}