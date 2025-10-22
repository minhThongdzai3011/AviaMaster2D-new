Shader "Fire Flush/Water Flat"
{
  Properties
  {
    _FlatColor ("FlatColor", Color) = (0.5,0.5,0.5,1)
    _VertColor ("VertColor", Color) = (0.5,0.5,0.5,1)
    _Opacity ("Opacity", float) = 0.8
    _WaveSpeed ("WaveSpeed", float) = 1
    _WaveCount ("WaveCount", float) = 1
    _WaveSize ("WaveSize", float) = 1
    _SecondaryWaveSpeed ("SecondaryWaveSpeed", float) = 1
    _SecondaryWaveCount ("SecondaryWaveCount", float) = 1
    _SecondaryWaveSize ("SecondaryWaveSize", float) = 1
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent+2"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDBASE"
        "QUEUE" = "Transparent+2"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      Blend SrcAlpha OneMinusSrcAlpha
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
      //uniform float4 _Time;
      uniform float4 _FlatColor;
      uniform float _WaveSpeed;
      uniform float _WaveCount;
      uniform float _WaveSize;
      uniform float _SecondaryWaveSpeed;
      uniform float _SecondaryWaveCount;
      uniform float _SecondaryWaveSize;
      uniform float4 _VertColor;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 color :COLOR0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord1 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = in_v.texcoord.xy;
          out_v.color = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float2 u_xlatb0;
      float2 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (_Time.yy * float2(_WaveSpeed, _SecondaryWaveSpeed));
          u_xlat0_d.y = ((in_f.texcoord1.x * _SecondaryWaveCount) + u_xlat0_d.y);
          u_xlat0_d.x = ((in_f.texcoord1.x * _WaveCount) + u_xlat0_d.x);
          u_xlat0_d.xy = sin(u_xlat0_d.xy);
          u_xlat1_d.x = (u_xlat0_d.y * _SecondaryWaveSize);
          u_xlat0_d.x = ((u_xlat0_d.x * _WaveSize) + u_xlat1_d.x);
          u_xlat0_d.x = (((-u_xlat0_d.x) * in_f.texcoord.x) + 1);
          u_xlat1_d.x = (_WaveSize + _SecondaryWaveSize);
          u_xlat0_d.x = ((-u_xlat1_d.x) + u_xlat0_d.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.y = (in_f.texcoord.y>=u_xlat0_d.x);
          #else
          u_xlatb0.y = (in_f.texcoord.y>=u_xlat0_d.x);
          #endif
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.x = (u_xlat0_d.x>=in_f.texcoord.y);
          #else
          u_xlatb0.x = (u_xlat0_d.x>=in_f.texcoord.y);
          #endif
          u_xlat1_d.xy = lerp(float2(0, 0), float2(1, 1), float2(u_xlatb0.yx));
          u_xlat1_d.x = (u_xlat1_d.x * u_xlat1_d.y);
          u_xlat0_d.x = (u_xlatb0.x)?(0):(u_xlat1_d.x);
          u_xlat0_d.x = (u_xlat0_d.x + u_xlat1_d.y);
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
          u_xlat0_d = ((-_FlatColor) + _VertColor);
          out_f.color = ((in_f.color.zzzz * u_xlat0_d) + _FlatColor);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: ShadowCaster
    {
      Name "ShadowCaster"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "Transparent+2"
        "RenderType" = "Transparent"
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
      //uniform float4 _Time;
      uniform float _WaveSpeed;
      uniform float _WaveCount;
      uniform float _WaveSize;
      uniform float _SecondaryWaveSpeed;
      uniform float _SecondaryWaveCount;
      uniform float _SecondaryWaveSize;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
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
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float2 u_xlat0_d;
      float2 u_xlatb0;
      float2 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (_Time.yy * float2(_WaveSpeed, _SecondaryWaveSpeed));
          u_xlat0_d.y = ((in_f.texcoord2.x * _SecondaryWaveCount) + u_xlat0_d.y);
          u_xlat0_d.x = ((in_f.texcoord2.x * _WaveCount) + u_xlat0_d.x);
          u_xlat0_d.xy = sin(u_xlat0_d.xy);
          u_xlat1_d.x = (u_xlat0_d.y * _SecondaryWaveSize);
          u_xlat0_d.x = ((u_xlat0_d.x * _WaveSize) + u_xlat1_d.x);
          u_xlat0_d.x = (((-u_xlat0_d.x) * in_f.texcoord1.x) + 1);
          u_xlat1_d.x = (_WaveSize + _SecondaryWaveSize);
          u_xlat0_d.x = ((-u_xlat1_d.x) + u_xlat0_d.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.y = (in_f.texcoord1.y>=u_xlat0_d.x);
          #else
          u_xlatb0.y = (in_f.texcoord1.y>=u_xlat0_d.x);
          #endif
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0.x = (u_xlat0_d.x>=in_f.texcoord1.y);
          #else
          u_xlatb0.x = (u_xlat0_d.x>=in_f.texcoord1.y);
          #endif
          u_xlat1_d.xy = lerp(float2(0, 0), float2(1, 1), float2(u_xlatb0.yx));
          u_xlat1_d.x = (u_xlat1_d.x * u_xlat1_d.y);
          u_xlat0_d.x = (u_xlatb0.x)?(0):(u_xlat1_d.x);
          u_xlat0_d.x = (u_xlat0_d.x + u_xlat1_d.y);
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
