Shader "Fire Flush/FireSmokeBillboard"
{
  Properties
  {
    _Dissolve ("Dissolve", 2D) = "white" {}
    _Highlight ("Highlight", Range(0, 1)) = 0.5740471
    _Wobble ("Wobble", float) = 1
    _Noise ("Noise", 2D) = "white" {}
    _Speed ("Speed", float) = 2
    _Lighting ("Lighting", 2D) = "white" {}
    _SmokeLightingDistortion ("SmokeLightingDistortion", Range(0, 1)) = 0.2307692
    _Normals ("Normals", 2D) = "bump" {}
    _Fresnel ("Fresnel", 2D) = "white" {}
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "AlphaTest"
      "RenderType" = "TransparentCutout"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
        "SHADOWSUPPORT" = "true"
      }
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      uniform float4 _FireSmokeColor;
      uniform float4 _Dissolve_ST;
      uniform float _Highlight;
      uniform float4 _FireSmokeShadowColor;
      uniform float _Wobble;
      uniform float4 _Noise_ST;
      uniform float _Speed;
      uniform float4 _Lighting_ST;
      uniform float _SmokeLightingDistortion;
      uniform float4 _Normals_ST;
      uniform float4 _Fresnel_ST;
      uniform sampler2D _Dissolve;
      uniform sampler2D _Noise;
      uniform sampler2D _Fresnel;
      uniform sampler2D _Normals;
      uniform sampler2D _Lighting;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.texcoord.xy = in_v.texcoord.xy;
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlat16_0;
      float3 u_xlat10_0;
      int u_xlatb0;
      float3 u_xlat1_d;
      float u_xlat16_1;
      float u_xlat10_1;
      float2 u_xlat2;
      float u_xlat10_2;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _Dissolve);
          u_xlat10_0.x = tex2D(_Dissolve, u_xlat0_d.xy).x;
          u_xlat0_d.x = (u_xlat10_0.x + in_f.color.w);
          u_xlat0_d.x = (u_xlat0_d.x + (-0.5));
          u_xlat1_d.xy = ((_Time.xx * float2(_Speed, _Speed)) + in_f.texcoord.xy);
          u_xlat1_d.xy = TRANSFORM_TEX(u_xlat1_d.xy, _Noise);
          u_xlat10_1 = tex2D(_Noise, u_xlat1_d.xy).x;
          u_xlat16_1 = ((u_xlat10_1 * 0.200000003) + (-0.100000001));
          u_xlat1_d.x = (u_xlat16_1 * _Wobble);
          u_xlat2.xy = ((in_f.color.ww * float2(10, (-0.400000006))) + float2(-8.00000095, 0.600000024));
          u_xlat1_d.x = (u_xlat2.y * u_xlat1_d.x);
          u_xlat2.x = u_xlat2.x;
          #ifdef UNITY_ADRENO_ES3
          u_xlat2.x = min(max(u_xlat2.x, 0), 1);
          #else
          u_xlat2.x = clamp(u_xlat2.x, 0, 1);
          #endif
          u_xlat0_d.x = ((u_xlat1_d.x * u_xlat2.x) + u_xlat0_d.x);
          u_xlat2.xy = TRANSFORM_TEX(in_f.texcoord.xy, _Fresnel);
          u_xlat10_2 = tex2D(_Fresnel, u_xlat2.xy).x;
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat10_2) + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0 = (u_xlat0_d.x<0);
          #else
          u_xlatb0 = (u_xlat0_d.x<0);
          #endif
          if(((int(u_xlatb0) * int(4294967295))!=0))
          {
              discard;
          }
          u_xlat0_d.xz = TRANSFORM_TEX(in_f.texcoord.xy, _Normals);
          u_xlat10_0.xz = tex2D(_Normals, u_xlat0_d.xz).xy;
          u_xlat16_0.xz = (u_xlat10_0.xz + float2(-0.5, (-0.5)));
          u_xlat0_d.xy = (u_xlat1_d.xx * u_xlat16_0.xz);
          u_xlat0_d.xy = (u_xlat0_d.xy * float2(_SmokeLightingDistortion, _SmokeLightingDistortion));
          u_xlat0_d.xy = ((u_xlat0_d.xy * float2(2, 2)) + in_f.texcoord.xy);
          u_xlat0_d.xy = TRANSFORM_TEX(u_xlat0_d.xy, _Lighting);
          u_xlat0_d.x = tex2D(_Lighting, u_xlat0_d.xy).x;
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0 = (u_xlat0_d.x>=_Highlight);
          #else
          u_xlatb0 = (u_xlat0_d.x>=_Highlight);
          #endif
          u_xlat0_d.x = (u_xlatb0)?(1):(float(0));
          u_xlat1_d.xyz = (_FireSmokeColor.xyz + (-_FireSmokeShadowColor.xyz));
          u_xlat0_d.xyz = ((u_xlat0_d.xxx * u_xlat1_d.xyz) + _FireSmokeShadowColor.xyz);
          out_f.color.xyz = (u_xlat0_d.xyz * in_f.color.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: ShadowCaster
    {
      Name "ShadowCaster"
      Tags
      { 
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
        "SHADOWSUPPORT" = "true"
      }
      Offset 1, 1
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile SHADOWS_DEPTH
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      uniform float4 _Dissolve_ST;
      uniform float _Wobble;
      uniform float4 _Noise_ST;
      uniform float _Speed;
      uniform float4 _Fresnel_ST;
      uniform sampler2D _Dissolve;
      uniform sampler2D _Noise;
      uniform sampler2D _Fresnel;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord1 :TEXCOORD1;
          float4 color :COLOR0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord1 :TEXCOORD1;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float u_xlat4;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = UnityObjectToClipPos(in_v.vertex);
          u_xlat1.x = (unity_LightShadowBias.x / u_xlat0.w);
          #ifdef UNITY_ADRENO_ES3
          u_xlat1.x = min(max(u_xlat1.x, 0), 1);
          #else
          u_xlat1.x = clamp(u_xlat1.x, 0, 1);
          #endif
          u_xlat4 = (u_xlat0.z + u_xlat1.x);
          u_xlat1.x = max((-u_xlat0.w), u_xlat4);
          out_v.vertex.xyw = u_xlat0.xyw;
          u_xlat0.x = ((-u_xlat4) + u_xlat1.x);
          out_v.vertex.z = ((unity_LightShadowBias.y * u_xlat0.x) + u_xlat4);
          out_v.texcoord1.xy = in_v.texcoord.xy;
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float2 u_xlat0_d;
      float u_xlat10_0;
      int u_xlatb0;
      float2 u_xlat1_d;
      float u_xlat16_1;
      float u_xlat10_1;
      float2 u_xlat2;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _Dissolve);
          u_xlat10_0 = tex2D(_Dissolve, u_xlat0_d.xy).x;
          u_xlat0_d.x = (u_xlat10_0 + in_f.color.w);
          u_xlat0_d.x = (u_xlat0_d.x + (-0.5));
          u_xlat1_d.xy = ((_Time.xx * float2(_Speed, _Speed)) + in_f.texcoord1.xy);
          u_xlat1_d.xy = TRANSFORM_TEX(u_xlat1_d.xy, _Noise);
          u_xlat10_1 = tex2D(_Noise, u_xlat1_d.xy).x;
          u_xlat16_1 = ((u_xlat10_1 * 0.200000003) + (-0.100000001));
          u_xlat1_d.x = (u_xlat16_1 * _Wobble);
          u_xlat2.xy = ((in_f.color.ww * float2(10, (-0.400000006))) + float2(-8.00000095, 0.600000024));
          u_xlat1_d.x = (u_xlat2.y * u_xlat1_d.x);
          u_xlat2.x = u_xlat2.x;
          #ifdef UNITY_ADRENO_ES3
          u_xlat2.x = min(max(u_xlat2.x, 0), 1);
          #else
          u_xlat2.x = clamp(u_xlat2.x, 0, 1);
          #endif
          u_xlat0_d.x = ((u_xlat1_d.x * u_xlat2.x) + u_xlat0_d.x);
          u_xlat1_d.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _Fresnel);
          u_xlat10_1 = tex2D(_Fresnel, u_xlat1_d.xy).x;
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat10_1) + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0 = (u_xlat0_d.x<0);
          #else
          u_xlatb0 = (u_xlat0_d.x<0);
          #endif
          if(((int(u_xlatb0) * int(4294967295))!=0))
          {
              discard;
          }
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
