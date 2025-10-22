Shader "Shader Forge/Neon"
{
  Properties
  {
    _PulseTexture ("PulseTexture", 2D) = "white" {}
    _PlaneTexture ("PlaneTexture", 2D) = "white" {}
    _PulseColor ("PulseColor", Color) = (1,0,0,1)
    _NoPulseColor ("NoPulseColor", Color) = (0,0.2762074,1,1)
    _GradientColorFront ("GradientColorFront", Color) = (1,0,0,1)
    _GradientColorBack ("GradientColorBack", Color) = (0.5,0.5,0.5,1)
    _PulsingSpeed ("PulsingSpeed", Range(0, 5)) = 0.6445339
    _ShadowColor ("ShadowColor", Color) = (0.5,0.5,0.5,1)
    _ShadowIntensity ("ShadowIntensity", Range(0, 1)) = 0.6752137
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
      //uniform float4 _Time;
      uniform float4 _PulseTexture_ST;
      uniform float4 _PlaneTexture_ST;
      uniform float4 _PulseColor;
      uniform float4 _NoPulseColor;
      uniform float4 _GradientColorFront;
      uniform float4 _GradientColorBack;
      uniform float _PulsingSpeed;
      uniform float4 _ShadowColor;
      uniform float _ShadowIntensity;
      uniform float4 _LightDirection;
      uniform float4 _PlaneWaterColor;
      uniform float _WaterLevel;
      uniform sampler2D _PlaneTexture;
      uniform sampler2D _PulseTexture;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
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
          out_v.texcoord3 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = in_v.texcoord.xy;
          out_v.texcoord1.xy = in_v.texcoord1.xy;
          out_v.texcoord2.xy = in_v.texcoord2.xy;
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord4.xyz = normalize(u_xlat0.xyz);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlat1_d;
      float2 u_xlat2;
      float3 u_xlat3;
      float u_xlat16_3;
      float3 u_xlat10_3;
      float u_xlat6_d;
      float u_xlat9;
      float u_xlat10_9;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.texcoord3.x + (-conv_mxt4x4_3(unity_ObjectToWorld)));
          u_xlat0_d.x = (u_xlat0_d.x + in_f.texcoord3.z);
          u_xlat0_d.x = (u_xlat0_d.x + (-conv_mxt4x4_3(unity_ObjectToWorld)));
          u_xlat0_d.x = sin(u_xlat0_d.x);
          u_xlat3.x = dot(in_f.texcoord4.xyz, in_f.texcoord4.xyz);
          u_xlat3.x = rsqrt(u_xlat3.x);
          u_xlat3.xyz = (u_xlat3.xxx * in_f.texcoord4.xyz);
          u_xlat1_d.x = dot(u_xlat3.xyz, _LightDirection.xyz);
          u_xlat1_d.x = ((-abs(u_xlat1_d.x)) + 1);
          u_xlat1_d.y = ((u_xlat1_d.x * u_xlat0_d.x) + _LightDirection.y);
          u_xlat1_d.xz = _LightDirection.xz;
          u_xlat0_d.x = dot(u_xlat1_d.xyz, u_xlat1_d.xyz);
          u_xlat0_d.x = rsqrt(u_xlat0_d.x);
          u_xlat1_d.xyz = (u_xlat0_d.xxx * u_xlat1_d.xyz);
          u_xlat0_d.x = dot(u_xlat1_d.xyz, u_xlat3.xyz);
          u_xlat0_d.x = (u_xlat0_d.x + 1);
          u_xlat0_d.x = (u_xlat0_d.x * 0.5);
          u_xlat0_d.x = round(u_xlat0_d.x);
          u_xlat3.x = ((-_ShadowIntensity) + 1);
          u_xlat6_d = ((-u_xlat3.x) + 1);
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat6_d) + u_xlat3.x);
          u_xlat3.xy = TRANSFORM_TEX(in_f.texcoord.xy, _PlaneTexture);
          u_xlat10_3.xyz = tex2D(_PlaneTexture, u_xlat3.xy).xyz;
          u_xlat16_3 = dot(u_xlat10_3.xyz, float3(0.300000012, 0.589999974, 0.109999999));
          u_xlat1_d.xyz = ((-_GradientColorFront.xyz) + _GradientColorBack.xyz);
          u_xlat1_d.xyz = ((in_f.texcoord2.xxx * u_xlat1_d.xyz) + _GradientColorFront.xyz);
          u_xlat3.xyz = ((float3(u_xlat16_3, u_xlat16_3, u_xlat16_3) * u_xlat1_d.xyz) + (-_ShadowColor.xyz));
          u_xlat0_d.xyz = ((u_xlat0_d.xxx * u_xlat3.xyz) + _ShadowColor.xyz);
          u_xlat9 = (((-_Time.z) * _PulsingSpeed) + in_f.texcoord2.x);
          u_xlat9 = sin(u_xlat9);
          u_xlat1_d.xyz = (_PulseColor.xyz + (-_NoPulseColor.xyz));
          u_xlat1_d.xyz = ((float3(u_xlat9, u_xlat9, u_xlat9) * u_xlat1_d.xyz) + _NoPulseColor.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xyz) + u_xlat1_d.xyz);
          u_xlat2.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _PulseTexture);
          u_xlat10_9 = tex2D(_PulseTexture, u_xlat2.xy).x;
          u_xlat9 = (u_xlat10_9 * 1.5);
          u_xlat9 = floor(u_xlat9);
          u_xlat9 = (u_xlat9 + u_xlat9);
          u_xlat0_d.xyz = ((float3(u_xlat9, u_xlat9, u_xlat9) * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xyz) + _PlaneWaterColor.xyz);
          u_xlat9 = (_PlaneWaterColor.w * _WaterLevel);
          out_f.color.xyz = ((float3(u_xlat9, u_xlat9, u_xlat9) * u_xlat1_d.xyz) + u_xlat0_d.xyz);
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
