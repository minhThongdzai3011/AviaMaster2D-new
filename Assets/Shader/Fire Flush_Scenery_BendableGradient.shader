Shader "Fire Flush/Scenery/BendableGradient"
{
  Properties
  {
    _GradientHigh ("GradientHigh", float) = 0
    _GradientLow ("GradientLow", float) = -20
    _ColorHigh ("ColorHigh", Color) = (0.5,0.5,0.5,1)
    _ColorLow ("ColorLow", Color) = (0.5,0.5,0.5,1)
    _AltitudeOffset ("AltitudeOffset", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
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
      uniform float _GradientLow;
      uniform float _GradientHigh;
      uniform float4 _ColorLow;
      uniform float4 _ColorHigh;
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
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = (in_f.texcoord.x * 3.14159274);
          u_xlat0_d = cos(u_xlat0_d);
          u_xlat0_d = (u_xlat0_d + (-1));
          u_xlat0_d = (u_xlat0_d * (-0.5));
          u_xlat1_d.x = ((-in_f.texcoord1.x) + in_f.texcoord1.y);
          u_xlat0_d = ((u_xlat0_d * u_xlat1_d.x) + in_f.texcoord1.x);
          u_xlat0_d = (u_xlat0_d + _AltitudeOffset);
          u_xlat0_d = ((-u_xlat0_d) + in_f.texcoord2.y);
          u_xlat0_d = (u_xlat0_d + (-_GradientLow));
          u_xlat1_d.x = ((-_GradientLow) + _GradientHigh);
          u_xlat0_d = (u_xlat0_d / u_xlat1_d.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlat0_d = min(max(u_xlat0_d, 0), 1);
          #else
          u_xlat0_d = clamp(u_xlat0_d, 0, 1);
          #endif
          u_xlat1_d.xyz = ((-_ColorLow.xyz) + _ColorHigh.xyz);
          out_f.color.xyz = ((float3(u_xlat0_d, u_xlat0_d, u_xlat0_d) * u_xlat1_d.xyz) + _ColorLow.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
