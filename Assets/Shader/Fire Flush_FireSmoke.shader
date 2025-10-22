Shader "Fire Flush/FireSmoke"
{
  Properties
  {
    _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
    _Dissolve ("Dissolve", 2D) = "white" {}
    _Highlight ("Highlight", Range(0, 1)) = 0
    _ShadowCOlor ("ShadowCOlor", Color) = (0.5,0.5,0.5,1)
    _Wobble ("Wobble", float) = 1
    _Noise ("Noise", 2D) = "white" {}
    _Speed ("Speed", float) = 2
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
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float _Wobble;
      uniform float4 _Noise_ST;
      uniform float _Speed;
      uniform sampler2D _Noise;
      //uniform float3 _WorldSpaceCameraPos;
      uniform float4 _Color;
      uniform float4 _Dissolve_ST;
      uniform float _Highlight;
      uniform float4 _ShadowCOlor;
      uniform sampler2D _Dissolve;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float2 u_xlat2;
      float u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0.x = (_Time.x * _Speed);
          u_xlat2.xy = (in_v.texcoord.xy + in_v.texcoord.xy);
          u_xlat0.xy = ((u_xlat0.xx * float2(-0.800000012, (-0.800000012))) + u_xlat2.xy);
          u_xlat0.xy = TRANSFORM_TEX(u_xlat0.xy, _Noise);
          u_xlat0.x = tex2Dlod(_Noise, float4(float3(u_xlat0.xy, 0), 0)).x;
          u_xlat2.xy = ((_Time.xx * float2(_Speed, _Speed)) + in_v.texcoord.xy);
          u_xlat2.xy = TRANSFORM_TEX(u_xlat2.xy, _Noise);
          u_xlat2.x = tex2Dlod(_Noise, float4(float3(u_xlat2.xy, 0), 0)).x;
          u_xlat0.x = ((u_xlat2.x * u_xlat0.x) + (-0.5));
          u_xlat0.xyz = (u_xlat0.xxx * in_v.normal.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * float3(_Wobble, _Wobble, _Wobble));
          u_xlat6 = ((-in_v.color.w) + 1);
          u_xlat6 = ((u_xlat6 * 0.400000006) + 0.100000001);
          u_xlat0.xyz = ((float3(u_xlat6, u_xlat6, u_xlat6) * u_xlat0.xyz) + in_v.vertex.xyz);
          u_xlat1 = (u_xlat0.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat1 = ((conv_mxt4x4_0(unity_ObjectToWorld) * u_xlat0.xxxx) + u_xlat1);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * u_xlat0.zzzz) + u_xlat1);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord1 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = in_v.texcoord.xy;
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord2.xyz = normalize(u_xlat0.xyz);
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float u_xlat10_0;
      int u_xlatb0;
      float3 u_xlat1_d;
      float3 u_xlat2_d;
      int u_xlatb2;
      float u_xlat4;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _Dissolve);
          u_xlat10_0 = tex2D(_Dissolve, u_xlat0_d.xy).x;
          u_xlat0_d.x = (u_xlat10_0 + in_f.color.w);
          u_xlat0_d.x = (u_xlat0_d.x + (-0.5));
          u_xlat2_d.xyz = ((-in_f.texcoord1.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat1_d.x = dot(u_xlat2_d.xyz, u_xlat2_d.xyz);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat2_d.xyz = (u_xlat2_d.xyz * u_xlat1_d.xxx);
          u_xlat1_d.x = dot(in_f.texcoord2.xyz, in_f.texcoord2.xyz);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat1_d.xyz = (u_xlat1_d.xxx * in_f.texcoord2.xyz);
          u_xlat2_d.x = dot(u_xlat1_d.xyz, u_xlat2_d.xyz);
          u_xlat2_d.x = max(u_xlat2_d.x, 0);
          u_xlat2_d.x = ((-u_xlat2_d.x) + 1);
          u_xlat4 = ((-u_xlat2_d.x) + 1);
          u_xlat2_d.xz = (u_xlat2_d.xx * u_xlat1_d.xy);
          u_xlat2_d.x = dot(u_xlat2_d.xz, float2(0.5, 1));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb2 = (u_xlat2_d.x>=_Highlight);
          #else
          u_xlatb2 = (u_xlat2_d.x>=_Highlight);
          #endif
          u_xlat2_d.x = (u_xlatb2)?(0):(1);
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat4) + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0 = (u_xlat0_d.x<0);
          #else
          u_xlatb0 = (u_xlat0_d.x<0);
          #endif
          if(((int(u_xlatb0) * int(4294967295))!=0))
          {
              discard;
          }
          u_xlat0_d.xzw = ((u_xlat2_d.xxx * _ShadowCOlor.xyz) + _Color.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xzw) + _ShadowCOlor.xyz);
          u_xlat0_d.xyz = ((u_xlat2_d.xxx * u_xlat1_d.xyz) + u_xlat0_d.xzw);
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
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float _Wobble;
      uniform float4 _Noise_ST;
      uniform float _Speed;
      uniform sampler2D _Noise;
      //uniform float3 _WorldSpaceCameraPos;
      uniform float4 _Dissolve_ST;
      uniform sampler2D _Dissolve;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 color :COLOR0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float2 u_xlat2;
      float u_xlat4;
      float u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0.x = (_Time.x * _Speed);
          u_xlat2.xy = (in_v.texcoord.xy + in_v.texcoord.xy);
          u_xlat0.xy = ((u_xlat0.xx * float2(-0.800000012, (-0.800000012))) + u_xlat2.xy);
          u_xlat0.xy = TRANSFORM_TEX(u_xlat0.xy, _Noise);
          u_xlat0.x = tex2Dlod(_Noise, float4(float3(u_xlat0.xy, 0), 0)).x;
          u_xlat2.xy = ((_Time.xx * float2(_Speed, _Speed)) + in_v.texcoord.xy);
          u_xlat2.xy = TRANSFORM_TEX(u_xlat2.xy, _Noise);
          u_xlat2.x = tex2Dlod(_Noise, float4(float3(u_xlat2.xy, 0), 0)).x;
          u_xlat0.x = ((u_xlat2.x * u_xlat0.x) + (-0.5));
          u_xlat0.xyz = (u_xlat0.xxx * in_v.normal.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * float3(_Wobble, _Wobble, _Wobble));
          u_xlat6 = ((-in_v.color.w) + 1);
          u_xlat6 = ((u_xlat6 * 0.400000006) + 0.100000001);
          u_xlat0.xyz = ((float3(u_xlat6, u_xlat6, u_xlat6) * u_xlat0.xyz) + in_v.vertex.xyz);
          u_xlat1 = (u_xlat0.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat1 = ((conv_mxt4x4_0(unity_ObjectToWorld) * u_xlat0.xxxx) + u_xlat1);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * u_xlat0.zzzz) + u_xlat1);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord2 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          u_xlat0 = mul(unity_MatrixVP, u_xlat1);
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
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord3.xyz = normalize(u_xlat0.xyz);
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      int u_xlatb0;
      float3 u_xlat1_d;
      float2 u_xlat2_d;
      float u_xlat10_2;
      float u_xlat6_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord2.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat0_d.xyz = normalize(u_xlat0_d.xyz);
          u_xlat1_d.xyz = normalize(in_f.texcoord3.xyz);
          u_xlat0_d.x = dot(u_xlat1_d.xyz, u_xlat0_d.xyz);
          u_xlat0_d.x = max(u_xlat0_d.x, 0);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat2_d.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _Dissolve);
          u_xlat10_2 = tex2D(_Dissolve, u_xlat2_d.xy).x;
          u_xlat2_d.x = (u_xlat10_2 + in_f.color.w);
          u_xlat2_d.x = (u_xlat2_d.x + (-0.5));
          u_xlat0_d.x = ((u_xlat2_d.x * u_xlat0_d.x) + (-0.5));
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
