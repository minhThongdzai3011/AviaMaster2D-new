Shader "Fire Flush/Plane/CockpitCutout"
{
  Properties
  {
    _OutlineColor ("OutlineColor", Color) = (1,1,1,1)
    _SpecularColor ("SpecularColor", Color) = (0.5,0.5,0.5,1)
    _FillColor ("FillColor", Color) = (0.07843138,0.3921569,0.7843137,1)
    _Specular ("Specular", Range(0.5, 1)) = 0.5
    _Outline ("Outline", Range(0, 3)) = 1
    _LightDirection ("LightDirection", Vector) = (1,1,1,0)
    _WaterLevel ("WaterLevel", Range(0, 1)) = 0
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDBASE"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      ZWrite Off
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
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _FillColor;
      uniform float4 _LightDirection;
      uniform float _Specular;
      uniform float4 _OutlineColor;
      uniform float4 _SpecularColor;
      uniform float _Outline;
      uniform float4 _PlaneWaterColor;
      uniform float _WaterLevel;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
      };
      
      struct OUT_Data_Vert
      {
          float3 texcoord :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float3 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord.xyz = normalize(u_xlat0.xyz);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlat1_d;
      float3 u_xlat2;
      float u_xlat6_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = dot(_LightDirection.xyz, _LightDirection.xyz);
          u_xlat0_d.x = rsqrt(u_xlat0_d.x);
          u_xlat0_d.xyz = ((_LightDirection.xyz * u_xlat0_d.xxx) + float3(0, 0, (-1)));
          u_xlat0_d.xyz = normalize(u_xlat0_d.xyz);
          u_xlat1_d.xyz = normalize(in_f.texcoord.xyz);
          u_xlat0_d.x = dot(u_xlat1_d.xyz, u_xlat0_d.xyz);
          u_xlat2.x = log2((-u_xlat1_d.z));
          u_xlat2.x = (u_xlat2.x * _Outline);
          u_xlat2.x = exp2(u_xlat2.x);
          u_xlat2.x = ((-u_xlat2.x) + 1);
          u_xlat2.x = round(u_xlat2.x);
          u_xlat2.x = (u_xlat2.x * _OutlineColor.w);
          u_xlat0_d.x = ((u_xlat0_d.x * 0.5) + 0.5);
          #ifdef UNITY_ADRENO_ES3
          u_xlat0_d.x = min(max(u_xlat0_d.x, 0), 1);
          #else
          u_xlat0_d.x = clamp(u_xlat0_d.x, 0, 1);
          #endif
          u_xlat0_d.x = (u_xlat0_d.x * _Specular);
          u_xlat0_d.x = round(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * _SpecularColor.w);
          u_xlat1_d.xyz = ((-_FillColor.xyz) + _OutlineColor.xyz);
          u_xlat1_d.xyz = ((u_xlat2.xxx * u_xlat1_d.xyz) + _FillColor.xyz);
          u_xlat2.x = max(u_xlat2.x, u_xlat0_d.x);
          out_f.color.w = max(u_xlat2.x, _FillColor.w);
          u_xlat2.xyz = ((-u_xlat1_d.xyz) + _SpecularColor.xyz);
          u_xlat0_d.xyz = ((u_xlat0_d.xxx * u_xlat2.xyz) + u_xlat1_d.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xyz) + _PlaneWaterColor.xyz);
          u_xlat6_d = (_PlaneWaterColor.w * _WaterLevel);
          out_f.color.xyz = ((float3(u_xlat6_d, u_xlat6_d, u_xlat6_d) * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
