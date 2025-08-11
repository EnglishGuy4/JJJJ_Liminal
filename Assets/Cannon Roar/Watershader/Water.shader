// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33090,y:32625,varname:node_2865,prsc:2|diff-9304-OUT,spec-358-OUT,gloss-1813-OUT,normal-683-OUT,emission-9304-OUT,alpha-7592-OUT,voffset-9781-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32179,y:32710,varname:node_6343,prsc:2|A-614-A,B-5773-OUT;n:type:ShaderForge.SFN_Color,id:6665,x:31794,y:32901,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:5964,x:31701,y:33160,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-752-UVOUT;n:type:ShaderForge.SFN_Slider,id:358,x:32435,y:32769,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32435,y:32864,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Multiply,id:2975,x:32303,y:32482,varname:node_2975,prsc:2|A-614-RGB,B-6665-RGB;n:type:ShaderForge.SFN_VertexColor,id:614,x:31998,y:32374,varname:node_614,prsc:2;n:type:ShaderForge.SFN_Fresnel,id:3883,x:32084,y:32876,varname:node_3883,prsc:2|EXP-6279-OUT;n:type:ShaderForge.SFN_Multiply,id:5184,x:30984,y:33292,varname:node_5184,prsc:2|A-7489-UVOUT,B-7598-OUT;n:type:ShaderForge.SFN_Multiply,id:5172,x:32468,y:32624,varname:node_5172,prsc:2|A-2975-OUT,B-3883-OUT;n:type:ShaderForge.SFN_Lerp,id:3011,x:31841,y:33345,varname:node_3011,prsc:2|A-621-OUT,B-212-RGB,T-2339-OUT;n:type:ShaderForge.SFN_Normalize,id:683,x:32513,y:32989,varname:node_683,prsc:2|IN-556-OUT;n:type:ShaderForge.SFN_NormalBlend,id:3409,x:31963,y:33064,varname:node_3409,prsc:2|BSE-3011-OUT,DTL-5964-RGB;n:type:ShaderForge.SFN_Vector3,id:621,x:31597,y:33363,varname:node_621,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:212,x:31352,y:33494,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False|UVIN-1275-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7592,x:32663,y:33112,varname:node_7592,prsc:2|A-2494-OUT,B-1577-A;n:type:ShaderForge.SFN_Multiply,id:2403,x:32225,y:33496,varname:node_2403,prsc:2|A-2649-OUT,B-8997-OUT;n:type:ShaderForge.SFN_Slider,id:2339,x:31597,y:33574,ptovrint:False,ptlb:Reflection,ptin:_Reflection,varname:node_2339,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Panner,id:1275,x:31162,y:33494,varname:node_1275,prsc:2,spu:1,spv:1|UVIN-5184-OUT;n:type:ShaderForge.SFN_Panner,id:752,x:31304,y:33049,varname:node_752,prsc:2,spu:1,spv:1|UVIN-8323-OUT;n:type:ShaderForge.SFN_Multiply,id:8323,x:31000,y:33065,varname:node_8323,prsc:2|A-7489-UVOUT,B-5008-OUT;n:type:ShaderForge.SFN_TexCoord,id:7489,x:30708,y:33186,varname:node_7489,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:1557,x:30706,y:33027,varname:node_1557,prsc:2,v1:100;n:type:ShaderForge.SFN_Slider,id:2494,x:32159,y:33239,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_2494,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.29,max:1;n:type:ShaderForge.SFN_VertexColor,id:1577,x:32237,y:33324,varname:node_1577,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:2649,x:31965,y:33479,varname:node_2649,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-212-RGB;n:type:ShaderForge.SFN_Slider,id:4308,x:31693,y:33710,ptovrint:False,ptlb:Distortion,ptin:_Distortion,varname:node_4308,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9313193,max:1;n:type:ShaderForge.SFN_Multiply,id:8997,x:32058,y:33684,varname:node_8997,prsc:2|A-4308-OUT,B-8423-OUT;n:type:ShaderForge.SFN_Vector1,id:8423,x:31917,y:33811,varname:node_8423,prsc:2,v1:0.2;n:type:ShaderForge.SFN_ValueProperty,id:7598,x:30708,y:33383,ptovrint:False,ptlb:UV_1,ptin:_UV_1,varname:node_7598,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_ValueProperty,id:5008,x:30691,y:33093,ptovrint:False,ptlb:UV_2,ptin:_UV_2,varname:node_5008,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:50;n:type:ShaderForge.SFN_Noise,id:7333,x:32968,y:33570,varname:node_7333,prsc:2|XY-8203-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:3711,x:33072,y:33262,ptovrint:False,ptlb:Displacement,ptin:_Displacement,varname:node_3711,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector2,id:2355,x:31794,y:33869,varname:node_2355,prsc:2,v1:20,v2:20;n:type:ShaderForge.SFN_UVTile,id:8203,x:32803,y:33649,varname:node_8203,prsc:2|UVIN-9178-UVOUT,WDT-927-OUT,HGT-927-OUT,TILE-927-OUT;n:type:ShaderForge.SFN_TexCoord,id:8536,x:32386,y:33679,varname:node_8536,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:927,x:32421,y:33542,ptovrint:False,ptlb:UV_D,ptin:_UV_D,varname:node_927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Panner,id:9178,x:32611,y:33846,varname:node_9178,prsc:2,spu:0,spv:1|UVIN-305-UVOUT,DIST-4270-OUT;n:type:ShaderForge.SFN_Multiply,id:2210,x:32027,y:33922,varname:node_2210,prsc:2|A-305-UVOUT,B-2355-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2449,x:32027,y:34124,ptovrint:False,ptlb:Displacement_speed_U,ptin:_Displacement_speed_U,varname:_UV_3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:25;n:type:ShaderForge.SFN_TexCoord,id:305,x:31763,y:34007,varname:node_305,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:6279,x:31430,y:32915,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_6279,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Lerp,id:556,x:32281,y:33047,varname:node_556,prsc:2|A-621-OUT,B-3409-OUT,T-1983-OUT;n:type:ShaderForge.SFN_OneMinus,id:1983,x:32262,y:32876,varname:node_1983,prsc:2|IN-6343-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5773,x:31905,y:32809,ptovrint:False,ptlb:Blend,ptin:_Blend,varname:node_5773,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_TexCoord,id:3042,x:32745,y:33388,varname:node_3042,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4264,x:32837,y:33906,varname:node_4264,prsc:2|A-9178-UVOUT,B-4270-OUT;n:type:ShaderForge.SFN_Multiply,id:4270,x:32572,y:34083,varname:node_4270,prsc:2|A-4499-OUT,B-347-T;n:type:ShaderForge.SFN_Time,id:347,x:32395,y:34223,varname:node_347,prsc:2;n:type:ShaderForge.SFN_Append,id:4499,x:32410,y:34043,varname:node_4499,prsc:2|A-2449-OUT,B-4438-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4438,x:32027,y:34267,ptovrint:False,ptlb:Displacement_speed_V,ptin:_Displacement_speed_V,varname:node_4438,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:25;n:type:ShaderForge.SFN_Multiply,id:5096,x:32186,y:33812,varname:node_5096,prsc:2;n:type:ShaderForge.SFN_Slider,id:1243,x:32320,y:33164,ptovrint:False,ptlb:Tes,ptin:_Tes,varname:node_1243,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Time,id:2171,x:32680,y:34424,varname:node_2171,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6132,x:32944,y:34334,varname:node_6132,prsc:2|A-9330-OUT,B-2171-TSL;n:type:ShaderForge.SFN_TexCoord,id:427,x:32944,y:34161,varname:node_427,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:9330,x:32740,y:34315,ptovrint:False,ptlb:WaveSpeed,ptin:_WaveSpeed,varname:node_9330,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Panner,id:8681,x:33183,y:34298,varname:node_8681,prsc:2,spu:0,spv:1|UVIN-427-UVOUT,DIST-6132-OUT;n:type:ShaderForge.SFN_Tex2d,id:9813,x:33407,y:34184,ptovrint:False,ptlb:WaveTexture,ptin:_WaveTexture,varname:node_9813,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c59b8cbed956f2042844a72433e62eaf,ntxv:0,isnm:False|UVIN-8681-UVOUT;n:type:ShaderForge.SFN_NormalVector,id:1806,x:33653,y:34302,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:6892,x:33574,y:34105,ptovrint:False,ptlb:WaveAmplitude,ptin:_WaveAmplitude,varname:node_6892,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Panner,id:1793,x:32739,y:33974,varname:node_1793,prsc:2,spu:0,spv:1;n:type:ShaderForge.SFN_Panner,id:7146,x:32803,y:34038,varname:node_7146,prsc:2,spu:0,spv:1;n:type:ShaderForge.SFN_Multiply,id:1568,x:33961,y:34201,varname:node_1568,prsc:2|A-9813-RGB,B-1806-OUT;n:type:ShaderForge.SFN_Multiply,id:9781,x:34137,y:34110,varname:node_9781,prsc:2|A-6892-OUT,B-1568-OUT;n:type:ShaderForge.SFN_Add,id:9304,x:33438,y:33760,varname:node_9304,prsc:2|A-9813-RGB,B-5172-OUT;proporder:5964-6665-358-1813-212-2339-2494-4308-7598-5008-3711-927-2449-5773-4438-6279-1243-9330-9813-6892;pass:END;sub:END;*/

Shader "Shader Forge/Water" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Refraction ("Refraction", 2D) = "bump" {}
        _Reflection ("Reflection", Range(0, 1)) = 0
        _Opacity ("Opacity", Range(0, 1)) = 0.29
        _Distortion ("Distortion", Range(0, 1)) = 0.9313193
        _UV_1 ("UV_1", Float ) = 40
        _UV_2 ("UV_2", Float ) = 50
        _Displacement ("Displacement", Float ) = 1
        _UV_D ("UV_D", Float ) = 1
        _Displacement_speed_U ("Displacement_speed_U", Float ) = 25
        _Blend ("Blend", Float ) = 0.5
        _Displacement_speed_V ("Displacement_speed_V", Float ) = 25
        _Fresnel ("Fresnel", Range(0, 10)) = 0
        _Tes ("Tes", Range(0, 100)) = 0
        _WaveSpeed ("WaveSpeed", Float ) = 0
        _WaveTexture ("WaveTexture", 2D) = "white" {}
        _WaveAmplitude ("WaveAmplitude", Range(0, 10)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 2.0
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform sampler2D _WaveTexture; uniform float4 _WaveTexture_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Metallic)
                UNITY_DEFINE_INSTANCED_PROP( float, _Gloss)
                UNITY_DEFINE_INSTANCED_PROP( float, _Reflection)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
                UNITY_DEFINE_INSTANCED_PROP( float, _UV_1)
                UNITY_DEFINE_INSTANCED_PROP( float, _UV_2)
                UNITY_DEFINE_INSTANCED_PROP( float, _Fresnel)
                UNITY_DEFINE_INSTANCED_PROP( float, _Blend)
                UNITY_DEFINE_INSTANCED_PROP( float, _WaveSpeed)
                UNITY_DEFINE_INSTANCED_PROP( float, _WaveAmplitude)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(7)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD8;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
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
                float _WaveAmplitude_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveAmplitude );
                float _WaveSpeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveSpeed );
                float4 node_2171 = _Time;
                float2 node_8681 = (o.uv0+(_WaveSpeed_var*node_2171.r)*float2(0,1));
                float4 _WaveTexture_var = tex2Dlod(_WaveTexture,float4(TRANSFORM_TEX(node_8681, _WaveTexture),0.0,0));
                v.vertex.xyz += (_WaveAmplitude_var*(_WaveTexture_var.rgb*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_621 = float3(0,0,1);
                float4 node_9623 = _Time;
                float _UV_1_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UV_1 );
                float2 node_1275 = ((i.uv0*_UV_1_var)+node_9623.g*float2(1,1));
                float4 _Refraction_var = tex2D(_Refraction,TRANSFORM_TEX(node_1275, _Refraction));
                float _Reflection_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Reflection );
                float _UV_2_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UV_2 );
                float2 node_752 = ((i.uv0*_UV_2_var)+node_9623.g*float2(1,1));
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_752, _BumpMap)));
                float3 node_3409_nrm_base = lerp(node_621,_Refraction_var.rgb,_Reflection_var) + float3(0,0,1);
                float3 node_3409_nrm_detail = _BumpMap_var.rgb * float3(-1,-1,1);
                float3 node_3409_nrm_combined = node_3409_nrm_base*dot(node_3409_nrm_base, node_3409_nrm_detail)/node_3409_nrm_base.z - node_3409_nrm_detail;
                float3 node_3409 = node_3409_nrm_combined;
                float _Blend_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Blend );
                float3 normalLocal = normalize(lerp(node_621,node_3409,(1.0 - (i.vertexColor.a*_Blend_var))));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float _Gloss_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Gloss );
                float gloss = _Gloss_var;
                float perceptualRoughness = 1.0 - _Gloss_var;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
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
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float _Metallic_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Metallic );
                float3 specularColor = _Metallic_var;
                float specularMonochrome;
                float _WaveSpeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveSpeed );
                float4 node_2171 = _Time;
                float2 node_8681 = (i.uv0+(_WaveSpeed_var*node_2171.r)*float2(0,1));
                float4 _WaveTexture_var = tex2D(_WaveTexture,TRANSFORM_TEX(node_8681, _WaveTexture));
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float _Fresnel_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Fresnel );
                float3 node_9304 = (_WaveTexture_var.rgb+((i.vertexColor.rgb*_Color_var.rgb)*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel_var)));
                float3 diffuseColor = node_9304; // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = node_9304;
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                float _Opacity_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Opacity );
                fixed4 finalRGBA = fixed4(finalColor,(_Opacity_var*i.vertexColor.a));
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
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 2.0
            uniform sampler2D _WaveTexture; uniform float4 _WaveTexture_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _WaveSpeed)
                UNITY_DEFINE_INSTANCED_PROP( float, _WaveAmplitude)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float _WaveAmplitude_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveAmplitude );
                float _WaveSpeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveSpeed );
                float4 node_2171 = _Time;
                float2 node_8681 = (o.uv0+(_WaveSpeed_var*node_2171.r)*float2(0,1));
                float4 _WaveTexture_var = tex2Dlod(_WaveTexture,float4(TRANSFORM_TEX(node_8681, _WaveTexture),0.0,0));
                v.vertex.xyz += (_WaveAmplitude_var*(_WaveTexture_var.rgb*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
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
            #define _GLOSSYENV 1
            #pragma multi_compile_instancing
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
            #pragma multi_compile_fog
            #pragma target 2.0
            uniform sampler2D _WaveTexture; uniform float4 _WaveTexture_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Metallic)
                UNITY_DEFINE_INSTANCED_PROP( float, _Gloss)
                UNITY_DEFINE_INSTANCED_PROP( float, _Fresnel)
                UNITY_DEFINE_INSTANCED_PROP( float, _WaveSpeed)
                UNITY_DEFINE_INSTANCED_PROP( float, _WaveAmplitude)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float _WaveAmplitude_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveAmplitude );
                float _WaveSpeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveSpeed );
                float4 node_2171 = _Time;
                float2 node_8681 = (o.uv0+(_WaveSpeed_var*node_2171.r)*float2(0,1));
                float4 _WaveTexture_var = tex2Dlod(_WaveTexture,float4(TRANSFORM_TEX(node_8681, _WaveTexture),0.0,0));
                v.vertex.xyz += (_WaveAmplitude_var*(_WaveTexture_var.rgb*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float _WaveSpeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveSpeed );
                float4 node_2171 = _Time;
                float2 node_8681 = (i.uv0+(_WaveSpeed_var*node_2171.r)*float2(0,1));
                float4 _WaveTexture_var = tex2D(_WaveTexture,TRANSFORM_TEX(node_8681, _WaveTexture));
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float _Fresnel_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Fresnel );
                float3 node_9304 = (_WaveTexture_var.rgb+((i.vertexColor.rgb*_Color_var.rgb)*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel_var)));
                o.Emission = node_9304;
                
                float3 diffColor = node_9304;
                float specularMonochrome;
                float3 specColor;
                float _Metallic_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Metallic );
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic_var, specColor, specularMonochrome );
                float _Gloss_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Gloss );
                float roughness = 1.0 - _Gloss_var;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
