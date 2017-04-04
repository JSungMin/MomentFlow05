// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,rprd:False,enco:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:1,culm:0,dpts:0,wrdp:True,dith:0,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3799,x:33137,y:32853,varname:node_3799,prsc:2|diff-1976-OUT,emission-4922-OUT,difocc-7954-OUT;n:type:ShaderForge.SFN_Tex2d,id:3006,x:32474,y:32898,ptovrint:False,ptlb:AO,ptin:_AO,varname:node_3006,prsc:2,tex:dcda1764414da7c418345973edadfc7d,ntxv:0,isnm:False|UVIN-1533-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:1533,x:32270,y:32842,varname:node_1533,prsc:2,uv:1;n:type:ShaderForge.SFN_Slider,id:621,x:32270,y:33161,ptovrint:False,ptlb:AO Multi,ptin:_AOMulti,varname:node_621,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:7954,x:32650,y:33019,varname:node_7954,prsc:2|A-5776-OUT,B-3006-R,T-621-OUT;n:type:ShaderForge.SFN_Vector1,id:5776,x:32375,y:33055,varname:node_5776,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:5000,x:32598,y:32597,ptovrint:False,ptlb:Illumination,ptin:_Illumination,varname:node_5000,prsc:2,tex:9c83d7655028d804ba27c3e11aa16c8b,ntxv:2,isnm:False|UVIN-1919-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4922,x:32902,y:32882,varname:node_4922,prsc:2|A-8772-OUT,B-5000-RGB,C-3908-RGB;n:type:ShaderForge.SFN_Slider,id:8772,x:32375,y:32772,ptovrint:False,ptlb:Illumination Power,ptin:_IlluminationPower,varname:node_8772,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Color,id:3908,x:32670,y:32872,ptovrint:False,ptlb:Illumination Color,ptin:_IlluminationColor,varname:node_3908,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:1177,x:32783,y:32672,varname:node_1177,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1515,x:32204,y:32399,ptovrint:False,ptlb:Decals,ptin:_Decals,varname:node_1515,prsc:2,ntxv:2,isnm:False|UVIN-7417-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2388,x:32375,y:32399,varname:node_2388,prsc:2|A-1515-RGB,B-1515-A;n:type:ShaderForge.SFN_OneMinus,id:6312,x:32375,y:32546,varname:node_6312,prsc:2|IN-1515-A;n:type:ShaderForge.SFN_Multiply,id:6396,x:32916,y:32527,varname:node_6396,prsc:2|A-6312-OUT,B-1177-RGB;n:type:ShaderForge.SFN_Add,id:1976,x:33101,y:32664,varname:node_1976,prsc:2|A-2388-OUT,B-6396-OUT;n:type:ShaderForge.SFN_TexCoord,id:7417,x:32049,y:32537,varname:node_7417,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:1919,x:32425,y:32630,varname:node_1919,prsc:2,uv:1;proporder:3006-621-5000-8772-3908-1515;pass:END;sub:END;*/

Shader "Town Center/BuildingsVertexC" {
    Properties {
        _AO ("AO", 2D) = "white" {}
        _AOMulti ("AO Multi", Range(0, 1)) = 1
        _Illumination ("Illumination", 2D) = "black" {}
        _IlluminationPower ("Illumination Power", Range(0, 1)) = 1
        _IlluminationColor ("Illumination Color", Color) = (1,1,1,1)
        _Decals ("Decals", 2D) = "black" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZTest Less
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma exclude_renderers xbox360 ps3 psp2 
            #pragma target 3.0
            uniform sampler2D _AO; uniform float4 _AO_ST;
            uniform float _AOMulti;
            uniform sampler2D _Illumination; uniform float4 _Illumination_ST;
            uniform float _IlluminationPower;
            uniform float4 _IlluminationColor;
            uniform sampler2D _Decals; uniform float4 _Decals_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
            #endif
            #ifdef DYNAMICLIGHTMAP_ON
                o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            #endif
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = UnityObjectToClipPos(v.vertex);
            TRANSFER_VERTEX_TO_FRAGMENT(o)
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            i.normalDir = normalize(i.normalDir);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);

            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 normalDirection = i.normalDir;
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightColor = _LightColor0.rgb;

            float attenuation = LIGHT_ATTENUATION(i);
            float3 attenColor = attenuation * _LightColor0.xyz;
            float Pi = 3.141592654;
            float InvPi = 0.31830988618;

            UnityLight light;
            #ifdef LIGHTMAP_OFF
                light.color = lightColor;
                light.dir = lightDirection;
                light.ndotl = LambertTerm (normalDirection, light.dir);
            #else
                light.color = half3(0.f, 0.f, 0.f);
                light.ndotl = 0.0f;
                light.dir = half3(0.f, 0.f, 0.f);
            #endif
            UnityGIInput d;
            d.light = light;
            d.worldPos = i.posWorld.xyz;
            d.worldViewDir = viewDirection;
            d.atten = attenuation;
            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                d.ambient = 0;
                d.lightmapUV = i.ambientOrLightmapUV;
            #else
                d.ambient = i.ambientOrLightmapUV;
            #endif
            UnityGI gi = UnityGlobalIllumination (d, 1, 0, normalDirection);
            lightDirection = gi.light.dir;
            lightColor = gi.light.color;

            float NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float4 _AO_var = tex2D(_AO,TRANSFORM_TEX(i.uv1, _AO));
            indirectDiffuse *= lerp(1.0,_AO_var.r,_AOMulti);
            float4 _Decals_var = tex2D(_Decals,TRANSFORM_TEX(i.uv0, _Decals));
            float3 diffuseColor = ((_Decals_var.rgb*_Decals_var.a)+((1.0 - _Decals_var.a)*i.vertexColor.rgb));
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;

            float4 _Illumination_var = tex2D(_Illumination,TRANSFORM_TEX(i.uv1, _Illumination));
            float3 emissive = (_IlluminationPower*_Illumination_var.rgb*_IlluminationColor.rgb);

            float3 finalColor = diffuse + emissive;
            return fixed4(finalColor,1);
        }
        ENDCG
    }
    Pass {
        Name "Meta"
        Tags {
            "LightMode"="Meta"
        }
        Cull Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_META 1
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #include "UnityMetaPass.cginc"
        #pragma fragmentoption ARB_precision_hint_fastest
        #pragma multi_compile_shadowcaster
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma exclude_renderers xbox360 ps3 psp2 
        #pragma target 3.0
        uniform sampler2D _Illumination; uniform float4 _Illumination_ST;
        uniform float _IlluminationPower;
        uniform float4 _IlluminationColor;
        uniform sampler2D _Decals; uniform float4 _Decals_ST;
        struct VertexInput {
            float4 vertex : POSITION;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
            float4 vertexColor : COLOR;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
            float4 vertexColor : COLOR;
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.vertexColor = v.vertexColor;
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
            return o;
        }
        float4 frag(VertexOutput i) : SV_Target {

            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
            
            float4 _Illumination_var = tex2D(_Illumination,TRANSFORM_TEX(i.uv1, _Illumination));
            o.Emission = (_IlluminationPower*_Illumination_var.rgb*_IlluminationColor.rgb);
            
            float4 _Decals_var = tex2D(_Decals,TRANSFORM_TEX(i.uv0, _Decals));
            float3 diffColor = ((_Decals_var.rgb*_Decals_var.a)+((1.0 - _Decals_var.a)*i.vertexColor.rgb));
            o.Albedo = diffColor;
            
            return UnityMetaFragment( o );
        }
        ENDCG
    }
}
FallBack "Diffuse"
}
