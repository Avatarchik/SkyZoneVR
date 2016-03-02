// Shader created with Shader Forge v1.19 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.19;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:753,x:32719,y:32712,varname:node_753,prsc:2|diff-2579-OUT,normal-2484-OUT,emission-6627-OUT,alpha-4630-OUT,clip-4767-OUT;n:type:ShaderForge.SFN_Tex2d,id:1235,x:32048,y:33199,ptovrint:False,ptlb:Digitize,ptin:_Digitize,varname:node_1235,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:29cb3acf556e84b448d60273fe9d6c0c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7279,x:32094,y:32111,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_7279,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b64d6691447b11a4bbf60817e658af89,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:4630,x:31687,y:32876,ptovrint:False,ptlb:Opacity_Slider,ptin:_Opacity_Slider,varname:node_4630,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:3.5,max:3.5;n:type:ShaderForge.SFN_Multiply,id:4767,x:32346,y:32987,varname:node_4767,prsc:2|A-4630-OUT,B-1235-RGB;n:type:ShaderForge.SFN_Tex2d,id:6993,x:31307,y:33403,ptovrint:False,ptlb:EmissiveRight,ptin:_EmissiveRight,varname:node_6993,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ec01889558d526141921c2b632a6fb73,ntxv:0,isnm:False|UVIN-9265-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:9730,x:31186,y:32826,ptovrint:False,ptlb:EmissiveUp,ptin:_EmissiveUp,varname:node_9730,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9377cd7dcb51d2a459c6d3485be53a48,ntxv:0,isnm:False|UVIN-2139-UVOUT;n:type:ShaderForge.SFN_Panner,id:2139,x:31030,y:33030,varname:node_2139,prsc:2,spu:0,spv:0.205;n:type:ShaderForge.SFN_Panner,id:9265,x:31045,y:33332,varname:node_9265,prsc:2,spu:0.1,spv:0;n:type:ShaderForge.SFN_Add,id:6466,x:31609,y:32295,varname:node_6466,prsc:2|A-9837-OUT,B-4611-OUT;n:type:ShaderForge.SFN_Multiply,id:5149,x:31820,y:32689,varname:node_5149,prsc:2|A-4611-OUT,B-4527-OUT;n:type:ShaderForge.SFN_Vector1,id:4527,x:31654,y:32444,varname:node_4527,prsc:2,v1:10;n:type:ShaderForge.SFN_Multiply,id:2484,x:31995,y:33027,varname:node_2484,prsc:2|A-4032-RGB,B-9399-OUT;n:type:ShaderForge.SFN_Vector1,id:9399,x:31834,y:33199,varname:node_9399,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:8013,x:32212,y:32374,varname:node_8013,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:2579,x:32501,y:32274,varname:node_2579,prsc:2|A-7279-RGB,B-8013-OUT;n:type:ShaderForge.SFN_Tex2d,id:4032,x:31736,y:33027,ptovrint:False,ptlb:node_4032,ptin:_node_4032,varname:node_4032,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ae14ecbf19fd04e80971b76744db1474,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4611,x:31595,y:32731,varname:node_4611,prsc:2|A-3581-OUT,B-6993-RGB;n:type:ShaderForge.SFN_Multiply,id:9837,x:31423,y:32675,varname:node_9837,prsc:2|A-3581-OUT,B-9730-RGB;n:type:ShaderForge.SFN_Vector3,id:3581,x:31205,y:33211,varname:node_3581,prsc:2,v1:0.9921569,v2:0.4078431,v3:0.01176471;n:type:ShaderForge.SFN_Tex2d,id:8626,x:30713,y:32215,ptovrint:False,ptlb:node_8626,ptin:_node_8626,varname:node_8626,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:498d25e441ec1924baacb0514a6c09d6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:6627,x:31980,y:32535,varname:node_6627,prsc:2|A-5149-OUT,B-6941-OUT;n:type:ShaderForge.SFN_Multiply,id:6941,x:31360,y:32470,varname:node_6941,prsc:2|A-8626-RGB,B-3287-OUT;n:type:ShaderForge.SFN_Vector1,id:3287,x:31217,y:32637,varname:node_3287,prsc:2,v1:0.5;proporder:7279-1235-4630-6993-9730-4032-8626;pass:END;sub:END;*/

Shader "Custom/Dynamic Grid" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Digitize ("Digitize", 2D) = "white" {}
        _Opacity_Slider ("Opacity_Slider", Range(0.5, 30.0)) = 3.5
        _EmissiveRight ("EmissiveRight", 2D) = "white" {}
        _EmissiveUp ("EmissiveUp", 2D) = "white" {}
        _node_4032 ("node_4032", 2D) = "white" {}
        _node_8626 ("node_8626", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Digitize; uniform float4 _Digitize_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Opacity_Slider;
            uniform sampler2D _EmissiveRight; uniform float4 _EmissiveRight_ST;
            uniform sampler2D _node_4032; uniform float4 _node_4032_ST;
            uniform sampler2D _node_8626; uniform float4 _node_8626_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _node_4032_var = tex2D(_node_4032,TRANSFORM_TEX(i.uv0, _node_4032));
                float3 normalLocal = (_node_4032_var.rgb*2.0);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _Digitize_var = tex2D(_Digitize,TRANSFORM_TEX(i.uv0, _Digitize));
                clip((_Opacity_Slider*_Digitize_var.rgb) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 diffuseColor = (_Diffuse_var.rgb*0.5);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 node_3581 = float3(0.9921569,0.4078431,0.01176471);
                float4 node_9853 = _Time + _TimeEditor;
                float2 node_9265 = (i.uv0+node_9853.g*float2(0.1,0));
                float4 _EmissiveRight_var = tex2D(_EmissiveRight,TRANSFORM_TEX(node_9265, _EmissiveRight));
                float3 node_4611 = (node_3581*_EmissiveRight_var.rgb);
                float4 _node_8626_var = tex2D(_node_8626,TRANSFORM_TEX(i.uv0, _node_8626));
                float3 emissive = ((node_4611*10.0)+(_node_8626_var.rgb*0.5));
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,_Opacity_Slider);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Digitize; uniform float4 _Digitize_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Opacity_Slider;
            uniform sampler2D _EmissiveRight; uniform float4 _EmissiveRight_ST;
            uniform sampler2D _node_4032; uniform float4 _node_4032_ST;
            uniform sampler2D _node_8626; uniform float4 _node_8626_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _node_4032_var = tex2D(_node_4032,TRANSFORM_TEX(i.uv0, _node_4032));
                float3 normalLocal = (_node_4032_var.rgb*2.0);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _Digitize_var = tex2D(_Digitize,TRANSFORM_TEX(i.uv0, _Digitize));
                clip((_Opacity_Slider*_Digitize_var.rgb) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 diffuseColor = (_Diffuse_var.rgb*0.5);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * _Opacity_Slider,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Digitize; uniform float4 _Digitize_ST;
            uniform float _Opacity_Slider;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
                float4 _Digitize_var = tex2D(_Digitize,TRANSFORM_TEX(i.uv0, _Digitize));
                clip((_Opacity_Slider*_Digitize_var.rgb) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
