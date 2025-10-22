Shader "Fire Flush/Plane/Shadow Self"
{
  Properties
  {
    _ShadowColor ("ShadowColor", Color) = (0.5,0.5,0.5,1)
    _ShadowIntensity ("ShadowIntensity", Range(0, 1)) = 0.6752137
    _MainTex ("MainTex", 2D) = "white" {}
    _LightDirection ("LightDirection", Vector) = (2,1.37,-0.56,0)
    _WaterLevel ("WaterLevel", Range(0, 1)) = 0
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
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _ShadowColor;
      uniform float4 _MainTex_ST;
      uniform float _ShadowIntensity;
      uniform float4 _LightDirection;
      uniform float4 _PlaneWaterColor;
      uniform float _WaterLevel;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord1 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = in_v.texcoord.xy;
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord2.xyz = normalize(u_xlat0.xyz);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlat1_d;
      float3 u_xlat2;
      float3 u_xlat10_2;
      float u_xlat4;
      float u_xlat6_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.texcoord1.x + (-conv_mxt4x4_3(unity_ObjectToWorld)));
          u_xlat0_d.x = (u_xlat0_d.x + in_f.texcoord1.z);
          u_xlat0_d.x = (u_xlat0_d.x + (-conv_mxt4x4_3(unity_ObjectToWorld)));
          u_xlat0_d.x = sin(u_xlat0_d.x);
          u_xlat2.x = dot(in_f.texcoord2.xyz, in_f.texcoord2.xyz);
          u_xlat2.x = rsqrt(u_xlat2.x);
          u_xlat2.xyz = (u_xlat2.xxx * in_f.texcoord2.xyz);
          u_xlat1_d.x = dot(u_xlat2.xyz, _LightDirection.xyz);
          u_xlat1_d.x = ((-abs(u_xlat1_d.x)) + 1);
          u_xlat1_d.y = ((u_xlat1_d.x * u_xlat0_d.x) + _LightDirection.y);
          u_xlat1_d.xz = _LightDirection.xz;
          u_xlat0_d.x = dot(u_xlat1_d.xyz, u_xlat1_d.xyz);
          u_xlat0_d.x = rsqrt(u_xlat0_d.x);
          u_xlat1_d.xyz = (u_xlat0_d.xxx * u_xlat1_d.xyz);
          u_xlat0_d.x = dot(u_xlat1_d.xyz, u_xlat2.xyz);
          u_xlat0_d.x = (u_xlat0_d.x + 1);
          u_xlat0_d.x = (u_xlat0_d.x * 0.5);
          u_xlat0_d.x = round(u_xlat0_d.x);
          u_xlat2.x = ((-_ShadowIntensity) + 1);
          u_xlat4 = ((-u_xlat2.x) + 1);
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat4) + u_xlat2.x);
          u_xlat2.xy = TRANSFORM_TEX(in_f.texcoord.xy, _MainTex);
          u_xlat10_2.xyz = tex2D(_MainTex, u_xlat2.xy).xyz;
          u_xlat2.xyz = (u_xlat10_2.xyz + (-_ShadowColor.xyz));
          u_xlat0_d.xyz = ((u_xlat0_d.xxx * u_xlat2.xyz) + _ShadowColor.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xyz) + _PlaneWaterColor.xyz);
          u_xlat6_d = (_PlaneWaterColor.w * _WaterLevel);
          out_f.color.xyz = ((float3(u_xlat6_d, u_xlat6_d, u_xlat6_d) * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          out_f.color.w = 1;
          //return z;
          //return x;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
