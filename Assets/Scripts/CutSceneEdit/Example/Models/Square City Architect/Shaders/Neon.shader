// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:Unlit/Color,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3138,x:33151,y:32932,varname:node_3138,prsc:2|emission-8152-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32383,y:32988,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_VertexColor,id:9071,x:32517,y:32831,varname:node_9071,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:7039,x:32723,y:33004,varname:node_7039,prsc:2|IN-1487-RGB;n:type:ShaderForge.SFN_Multiply,id:978,x:32840,y:33173,varname:node_978,prsc:2|A-7241-RGB,B-1487-RGB,C-4461-OUT;n:type:ShaderForge.SFN_Time,id:9347,x:31819,y:32896,varname:node_9347,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1487,x:32488,y:33156,ptovrint:False,ptlb:Pattern,ptin:_Pattern,varname:node_1487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fcc30fed504a4f4449de3501ab55a984,ntxv:0,isnm:False|UVIN-7096-OUT;n:type:ShaderForge.SFN_Vector2,id:5159,x:31819,y:33070,varname:node_5159,prsc:2,v1:-1,v2:0;n:type:ShaderForge.SFN_Multiply,id:6177,x:32035,y:33009,varname:node_6177,prsc:2|A-9347-T,B-5159-OUT,C-7804-OUT;n:type:ShaderForge.SFN_TexCoord,id:3088,x:32034,y:33171,varname:node_3088,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:7096,x:32288,y:33156,varname:node_7096,prsc:2|A-6177-OUT,B-3088-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:7804,x:31799,y:33255,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_7804,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:5926,x:32869,y:32894,varname:node_5926,prsc:2|A-9071-RGB,B-7039-OUT,C-644-OUT;n:type:ShaderForge.SFN_Add,id:8152,x:32943,y:33034,varname:node_8152,prsc:2|A-5926-OUT,B-978-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4461,x:32840,y:33429,ptovrint:False,ptlb:HDR Second Color,ptin:_HDRSecondColor,varname:node_4461,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:644,x:32941,y:32754,ptovrint:False,ptlb:HDR First Color,ptin:_HDRFirstColor,varname:_HDR_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:7241-1487-7804-4461-644;pass:END;sub:END;*/

Shader "Town Center/Neon" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Pattern ("Pattern", 2D) = "white" {}
        _Speed ("Speed", Float ) = 0.2
        _HDRSecondColor ("HDR Second Color", Float ) = 1
        _HDRFirstColor ("HDR First Color", Float ) = 1
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Pattern; uniform float4 _Pattern_ST;
            uniform float _Speed;
            uniform float _HDRSecondColor;
            uniform float _HDRFirstColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 node_9347 = _Time + _TimeEditor;
                float2 node_7096 = ((node_9347.g*float2(-1,0)*_Speed)+i.uv0);
                float4 _Pattern_var = tex2D(_Pattern,TRANSFORM_TEX(node_7096, _Pattern));
                float3 emissive = ((i.vertexColor.rgb*(1.0 - _Pattern_var.rgb)*_HDRFirstColor)+(_Color.rgb*_Pattern_var.rgb*_HDRSecondColor));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
    CustomEditor "ShaderForgeMaterialInspector"
}
