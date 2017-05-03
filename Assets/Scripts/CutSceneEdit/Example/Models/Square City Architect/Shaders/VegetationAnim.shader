// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Town Center/Vegetation"
{
	Properties
	{
		//vertex animation
		_windSource("Wind Direction", Float) = (1, 0, 0, 0)
		windSpeed("Wind Speed", Range(0.0, 5.0)) = 1
		windSpeed(" ", Float) = 1
		windStrength("Wind Strength", Range(0.0, 3.0)) = 1
		windStrength(" ", Float) = 1
		_pivot("Pivot Offset", Float) = (0, 0, 0, 1)
		_mainBendFalloff("Main Bending Falloff", Range(0.0, 1.0)) = .25
		_mainBendFalloff(" ", Float) = .25
		_mainBendSpeed("Main Bending Speed", Range(0.0, 1.0)) = .25
		_mainBendSpeed(" ", Float) = .25
		_mainBendStrength("Main Bending Strength", Range(0.0, 1.0)) = .25
		_mainBendStrength(" ", Float) = .25
		_Phase("Phase", Float) = .25
	}
	SubShader
	{
		Tags{"RenderType" = "Opaque" }

		
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			Cull Off
			ZTest Less
			CGPROGRAM
			#define UNITY_PASS_FORWARDBASE
			#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "HLSLSupport.cginc"
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0

			uniform fixed3 _windSource;
			half windStrength;
			half windSpeed;
			half _mainBendSpeed;
			half _mainBendStrength;
			uniform fixed3 _pivot;
			half _mainBendFalloff;
			half _Phase;

			struct app2vert
			{
				float4 vertex 	: POSITION;
				float2 texCoord0 : TEXCOORD0;
				float2 texCoord1 : TEXCOORD1;
				float2 texCoord2 : TEXCOORD2;
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 color : COLOR;
			};

			struct vert2Pixel
			{
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
				half3 posWorld : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 lighting : TEXCOORD3;
			#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
				float4 ambientOrLightmapUV : TEXCOORD4;
			#endif
			};

			fixed3 SmoothWave(fixed3 input)
			{
				return input * input * (3.0 - 2.0 * input);
			}
			fixed3 TriangleWave(fixed3 input)
			{
				return abs(frac(input + 0.5) * 2.0 - 1.0);
			}
			fixed3 SmoothTriangleWave(fixed3 input)
			{
				return SmoothWave(TriangleWave(input));
			}

			vert2Pixel vert(app2vert IN)
			{
				vert2Pixel OUT;
				UNITY_INITIALIZE_OUTPUT(vert2Pixel, OUT);

				IN.vertex = mul(unity_ObjectToWorld, IN.vertex);
				float4 originalPosition = IN.vertex;

				float3 binormal = cross(IN.normal.xyz, IN.tangent.xyz);

				float3 deformedPositionT = IN.vertex.xyz - (IN.tangent.xyz * 0.01);
				float3 originalPositionT = deformedPositionT;
				float3 deformedPositionB = IN.vertex.xyz + (binormal* 0.01);
				float3 originalPositionB = deformedPositionB;

				half3 windDir = normalize(_windSource.xyz);
				float Time = (_Time.y*windSpeed) + _Phase;;
				//fixed variation = _branchBendVariation * IN.color.y;
				half3 pivot = mul(unity_ObjectToWorld, half4(_pivot.xyz, 1)).xyz;

				//main bending
				//saturate since we don't want to bend back opposing the wind
				half3 mainDeformation = saturate(SmoothTriangleWave(windDir * Time * _mainBendSpeed))  * windStrength;
				mainDeformation.y = 0;
				//create a mask based on distance from the object pivot point 
				half mainDeformationMask = length(originalPosition.xyz - pivot) * _mainBendFalloff;
				// Smooth bending factor similar to triangle waves
				mainDeformationMask += 1.0;
				mainDeformationMask *= mainDeformationMask;
				mainDeformationMask = mainDeformationMask * mainDeformationMask - mainDeformationMask;
				// Rescale  
				IN.vertex.xyz += mainDeformation * mainDeformationMask*  _mainBendStrength;

				//main bending T
				//saturate since we don't want to bend back opposing the wind
				mainDeformation = saturate(SmoothTriangleWave(Time * _mainBendSpeed * windDir))  * windStrength;
				mainDeformation.y = 0;
				//create a mask based on distance from the object pivot point 
				mainDeformationMask = length(originalPositionT.xyz - pivot) * _mainBendFalloff;
				// Smooth bending factor similar to triangle waves
				mainDeformationMask += 1.0;
				mainDeformationMask *= mainDeformationMask;
				mainDeformationMask = mainDeformationMask * mainDeformationMask - mainDeformationMask;
				// Rescale  
				deformedPositionT.xyz += mainDeformation * mainDeformationMask*  _mainBendStrength;

				//main bending B
				//saturate since we don't want to bend back opposing the wind
				mainDeformation = saturate(SmoothTriangleWave(Time * _mainBendSpeed * windDir))  * windStrength;
				mainDeformation.y = 0;
				//create a mask based on distance from the object pivot point 
				mainDeformationMask = length(originalPositionB.xyz - pivot) * _mainBendFalloff;
				// Smooth bending factor similar to triangle waves
				mainDeformationMask += 1.0;
				mainDeformationMask *= mainDeformationMask;
				mainDeformationMask = mainDeformationMask * mainDeformationMask - mainDeformationMask;
				// Rescale  
				deformedPositionB.xyz += mainDeformation * mainDeformationMask*  _mainBendStrength;

				//new normals
				fixed3 normB = normalize(deformedPositionB - IN.vertex.xyz);
				fixed3 normT = normalize(IN.vertex.xyz - deformedPositionT);
				fixed3 normC = cross(normB, -normT);

				fixed4 ambient = fixed4(0, 0, 0, 0);

				#ifdef LIGHTMAP_ON
				OUT.ambientOrLightmapUV.xy = IN.texCoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				OUT.ambientOrLightmapUV.zw = 0;
				ambient = OUT.ambientOrLightmapUV;
				#elif UNITY_SHOULD_SAMPLE_SH
				#endif
				#ifdef DYNAMICLIGHTMAP_ON
				OUT.ambientOrLightmapUV.zw = IN.texCoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				ambient = OUT.ambientOrLightmapUV;
				#endif			

				OUT.posWorld = IN.vertex.xyz;
				OUT.viewDir = normalize(OUT.posWorld - _WorldSpaceCameraPos);

				IN.vertex = mul(unity_WorldToObject, float4 (IN.vertex.xyz, 1))* 1.0;
				OUT.pos = UnityObjectToClipPos(IN.vertex);

				OUT.uvs = IN.texCoord0;

				fixed3 vertexLighting = fixed3(0.0, 0.0, 0.0);

				float3 worldN = UnityObjectToWorldNormal(normC);
					fixed3 NdotL = max(0, dot(_WorldSpaceLightPos0.xyz, worldN));

				vertexLighting = saturate(NdotL);

				OUT.lighting = IN.color.xyz * (vertexLighting + ShadeSH9(half4(worldN, 1.0)));

				#ifdef VERTEXLIGHT_ON
				OUT.lighting.rgb += Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, OUT.posWorld, worldN);
				#endif

				return OUT;
			}

			fixed4 frag(vert2Pixel IN) : COLOR
			{
				fixed3 color = IN.lighting;
				return fixed4(color, 0);
			}

			ENDCG
		}
	}
}