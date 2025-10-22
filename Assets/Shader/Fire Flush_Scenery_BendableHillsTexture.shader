Shader "Fire Flush/Scenery/BendableHillsTexture"
{
  Properties
  {
    _GradientHigh ("GradientHigh", float) = 0
    _GradientLow ("GradientLow", float) = -20
    _ColorHigh ("ColorHigh", Color) = (0.5,0.5,0.5,1)
    _ColorLow ("ColorLow", Color) = (0.5,0.5,0.5,1)
    _MainTex ("MainTex", 2D) = "white" {}
    _TextureWidth ("TextureWidth", float) = 0
    _TextureHeight ("TextureHeight", float) = 0
    _RedColor ("RedColor", Color) = (1,0,0,1)
    _GreenColor ("GreenColor", Color) = (0,1,0,1)
    _AltitudeOffset ("AltitudeOffset", float) = 0
    _HillWidth ("HillWidth", float) = 30
    _HillAmplitude ("HillAmplitude", float) = 10
    _HillSmooth ("HillSmooth", Range(0, 1)) = 1
    _HillOffset ("HillOffset", float) = 0
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float _AltitudeOffset;
      uniform float _HillWidth;
      uniform float _HillAmplitude;
      uniform float _HillSmooth;
      uniform float _HillOffset;
      uniform float4 _MainTex_ST;
      uniform float _TextureHeight;
      uniform float _TextureWidth;
      uniform float4 _RedColor;
      uniform float4 _GreenColor;
      uniform float _GradientLow;
      uniform float _GradientHigh;
      uniform float4 _ColorLow;
      uniform float4 _ColorHigh;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord2 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = in_v.texcoord.xy;
          out_v.texcoord1.xy = in_v.texcoord1.xy;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float u_xlat0_d;
      float3 u_xlat1_d;
      float2 u_xlat10_1;
      float3 u_xlat2;
      float3 u_xlatb2;
      float3 u_xlat3;
      float u_xlat4;
      int u_xlatb4;
      float u_xlat6;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = (in_f.texcoord.x * 3.14159274);
          u_xlat0_d = cos(u_xlat0_d);
          u_xlat0_d = (u_xlat0_d + (-1));
          u_xlat0_d = (u_xlat0_d * (-0.5));
          u_xlat2.x = ((-in_f.texcoord1.x) + in_f.texcoord1.y);
          u_xlat0_d = ((u_xlat0_d * u_xlat2.x) + in_f.texcoord1.x);
          u_xlat0_d = (u_xlat0_d + _AltitudeOffset);
          u_xlat2.x = (in_f.texcoord2.x + _HillOffset);
          u_xlat2.x = (u_xlat2.x / _HillWidth);
          u_xlat2.x = frac(u_xlat2.x);
          u_xlat4 = ((_HillSmooth * 0.5) + 0.5);
          u_xlat6 = ((-u_xlat4) + 1);
          u_xlat4 = ((-u_xlat6) + u_xlat4);
          u_xlat2.x = ((u_xlat2.x * u_xlat4) + u_xlat6);
          u_xlat2.x = (u_xlat2.x * 6.28318548);
          u_xlat2.x = cos(u_xlat2.x);
          u_xlat0_d = (((-u_xlat2.x) * _HillAmplitude) + u_xlat0_d);
          u_xlat0_d = ((-u_xlat0_d) + in_f.texcoord2.y);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb2.y = (0>=u_xlat0_d);
          #else
          u_xlatb2.y = (0>=u_xlat0_d);
          #endif
          u_xlat1_d.y = (u_xlat0_d / _TextureHeight);
          u_xlat0_d = (((-u_xlat2.x) * _HillAmplitude) + u_xlat0_d);
          u_xlat0_d = (u_xlat0_d + (-_GradientLow));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb2.x = (u_xlat1_d.y>=0);
          #else
          u_xlatb2.x = (u_xlat1_d.y>=0);
          #endif
          #ifdef UNITY_ADRENO_ES3
          u_xlatb2.z = (1>=u_xlat1_d.y);
          #else
          u_xlatb2.z = (1>=u_xlat1_d.y);
          #endif
          u_xlat2.xyz = lerp(float3(0, 0, 0), float3(1, 1, 1), float3(u_xlatb2.xyz));
          u_xlat2.x = (u_xlat2.z * u_xlat2.x);
          u_xlat1_d.x = (in_f.texcoord2.x / _TextureWidth);
          u_xlat1_d.xy = TRANSFORM_TEX(u_xlat1_d.xy, _MainTex);
          u_xlat10_1.xy = tex2D(_MainTex, u_xlat1_d.xy).yw;
          u_xlat6 = (u_xlat2.x * u_xlat10_1.y);
          u_xlat4 = max(u_xlat6, u_xlat2.y);
          u_xlat4 = (u_xlat4 + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb4 = (u_xlat4<0);
          #else
          u_xlatb4 = (u_xlat4<0);
          #endif
          if(((int(u_xlatb4) * int(4294967295))!=0))
          {
              discard;
          }
          u_xlat3.xyz = ((-_RedColor.xyz) + _GreenColor.xyz);
          u_xlat1_d.xyz = ((u_xlat10_1.xxx * u_xlat3.xyz) + _RedColor.xyz);
          u_xlat1_d.xyz = (u_xlat1_d.xyz + (-_ColorHigh.xyz));
          u_xlat2.xyz = ((u_xlat2.xxx * u_xlat1_d.xyz) + _ColorHigh.xyz);
          u_xlat2.xyz = (u_xlat2.xyz + (-_ColorLow.xyz));
          u_xlat1_d.x = ((-_GradientLow) + _GradientHigh);
          u_xlat0_d = (u_xlat0_d / u_xlat1_d.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlat0_d = min(max(u_xlat0_d, 0), 1);
          #else
          u_xlat0_d = clamp(u_xlat0_d, 0, 1);
          #endif
          out_f.color.xyz = ((float3(u_xlat0_d, u_xlat0_d, u_xlat0_d) * u_xlat2.xyz) + _ColorLow.xyz);
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
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float _AltitudeOffset;
      uniform float _HillWidth;
      uniform float _HillAmplitude;
      uniform float _HillSmooth;
      uniform float _HillOffset;
      uniform float4 _MainTex_ST;
      uniform float _TextureHeight;
      uniform float _TextureWidth;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord3 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
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
          out_v.texcoord2.xy = in_v.texcoord1.xy;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlatb0;
      float2 u_xlat1_d;
      float u_xlat2;
      float2 u_xlat4_d;
      float u_xlat10_4;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.texcoord3.x + _HillOffset);
          u_xlat0_d.x = (u_xlat0_d.x / _HillWidth);
          u_xlat0_d.x = frac(u_xlat0_d.x);
          u_xlat2 = ((_HillSmooth * 0.5) + 0.5);
          u_xlat4_d.x = ((-u_xlat2) + 1);
          u_xlat2 = ((-u_xlat4_d.x) + u_xlat2);
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat2) + u_xlat4_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * 6.28318548);
          u_xlat0_d.y = (in_f.texcoord1.x * 3.14159274);
          u_xlat0_d.xy = cos(u_xlat0_d.xy);
          u_xlat2 = (u_xlat0_d.y + (-1));
          u_xlat2 = (u_xlat2 * (-0.5));
          u_xlat4_d.x = ((-in_f.texcoord2.x) + in_f.texcoord2.y);
          u_xlat2 = ((u_xlat2 * u_xlat4_d.x) + in_f.texcoord2.x);
          u_xlat2 = (u_xlat2 + _AltitudeOffset);
          u_xlat0_d.x = (((-u_xlat0_d.x) * _HillAmplitude) + u_xlat2);
          u_xlat0_d.x = ((-u_xlat0_d.x) + in_f.texcoord3.y);
          u_xlat1_d.y = (u_xlat0_d.x / _TextureHeight);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.x = (0>=u_xlat0_d.x);
          #else
          u_xlatb0.x = (0>=u_xlat0_d.x);
          #endif
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.y = (u_xlat1_d.y>=0);
          #else
          u_xlatb0.y = (u_xlat1_d.y>=0);
          #endif
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.z = (1>=u_xlat1_d.y);
          #else
          u_xlatb0.z = (1>=u_xlat1_d.y);
          #endif
          u_xlat0_d.xyz = lerp(float3(0, 0, 0), float3(1, 1, 1), float3(u_xlatb0.xyz));
          u_xlat2 = (u_xlat0_d.z * u_xlat0_d.y);
          u_xlat1_d.x = (in_f.texcoord3.x / _TextureWidth);
          u_xlat4_d.xy = TRANSFORM_TEX(u_xlat1_d.xy, _MainTex);
          u_xlat10_4 = tex2D(_MainTex, u_xlat4_d.xy).w;
          u_xlat2 = (u_xlat2 * u_xlat10_4);
          u_xlat0_d.x = max(u_xlat2, u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.x = (u_xlat0_d.x<0);
          #else
          u_xlatb0.x = (u_xlat0_d.x<0);
          #endif
          if(((int(u_xlatb0.x) * int(4294967295))!=0))
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
