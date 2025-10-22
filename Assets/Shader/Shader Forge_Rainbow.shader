Shader "Shader Forge/Rainbow"
{
  Properties
  {
    _PlaneTexture ("PlaneTexture", 2D) = "white" {}
    _TextureMask ("TextureMask", 2D) = "white" {}
    _SparkleTexture ("SparkleTexture", 2D) = "white" {}
    _ScrollSpeed ("ScrollSpeed", Range(0, 5)) = 1.165048
    _ShineOpacity ("ShineOpacity", Range(0, 1)) = 1
    _ShineIntensity ("ShineIntensity", Range(0.0001, 1.5)) = 1.014135
    _ShineSteps ("ShineSteps", Range(0, 15)) = 4.405025
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
      //uniform float4 _ProjectionParams;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixV;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      //uniform float3 _WorldSpaceCameraPos;
      uniform float4 _PlaneTexture_ST;
      uniform float4 _TextureMask_ST;
      uniform float _ScrollSpeed;
      uniform float _ShineIntensity;
      uniform float _ShineSteps;
      uniform float _ShineOpacity;
      uniform float4 _SparkleTexture_ST;
      uniform float4 _ShadowColor;
      uniform float _ShadowIntensity;
      uniform float4 _LightDirection;
      uniform float4 _PlaneWaterColor;
      uniform float _WaterLevel;
      uniform sampler2D _PlaneTexture;
      uniform sampler2D _SparkleTexture;
      uniform sampler2D _TextureMask;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float3 u_xlat2;
      float u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord2 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          u_xlat0 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat0;
          out_v.texcoord.xy = in_v.texcoord.xy;
          out_v.texcoord1.xy = in_v.texcoord1.xy;
          u_xlat2.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat2.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat2.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord3.xyz = normalize(u_xlat2.xyz);
          u_xlat6 = (u_xlat1.y * conv_mxt4x4_1(unity_MatrixV).z);
          u_xlat6 = ((conv_mxt4x4_0(unity_MatrixV).z * u_xlat1.x) + u_xlat6);
          u_xlat6 = ((conv_mxt4x4_2(unity_MatrixV).z * u_xlat1.z) + u_xlat6);
          u_xlat6 = ((conv_mxt4x4_3(unity_MatrixV).z * u_xlat1.w) + u_xlat6);
          out_v.texcoord4.z = (-u_xlat6);
          u_xlat0.y = (u_xlat0.y * _ProjectionParams.x);
          u_xlat1.xzw = (u_xlat0.xwy * float3(0.5, 0.5, 0.5));
          out_v.texcoord4.w = u_xlat0.w;
          out_v.texcoord4.xy = (u_xlat1.zz + u_xlat1.xw);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlatb0;
      float3 u_xlat1_d;
      float3 u_xlat10_1;
      float3 u_xlat2_d;
      float3 u_xlat3;
      float u_xlat5;
      float u_xlat8;
      float u_xlat12;
      float u_xlat16_12;
      float u_xlat10_12;
      float u_xlat13;
      float u_xlat14;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (in_f.texcoord4.xy / in_f.texcoord4.ww);
          u_xlat0_d.xy = ((u_xlat0_d.xy * float2(2, 2)) + float2(-0.399999976, (-0.399999976)));
          u_xlat8 = (_Time.y * 10);
          u_xlat1_d.x = sin(u_xlat8);
          u_xlat2_d.x = cos(u_xlat8);
          u_xlat3.z = u_xlat1_d.x;
          u_xlat3.y = u_xlat2_d.x;
          u_xlat3.x = (-u_xlat1_d.x);
          u_xlat1_d.y = dot(u_xlat0_d.xy, u_xlat3.xy);
          u_xlat1_d.x = dot(u_xlat0_d.xy, u_xlat3.yz);
          u_xlat0_d.xy = (u_xlat1_d.xy + float2(-0.600000024, (-0.600000024)));
          u_xlat0_d.xy = TRANSFORM_TEX(u_xlat0_d.xy, _SparkleTexture);
          u_xlat0_d.xyz = tex2D(_SparkleTexture, u_xlat0_d.xy).xyz;
          u_xlatb0.xyz = bool4(float4(0.939999998, 0.939999998, 0.939999998, 0) >= u_xlat0_d.xyzx).xyz;
          u_xlat0_d.xyz = lerp(float3(0, 0, 0), float3(1, 1, 1), float3(u_xlatb0.xyz));
          u_xlat12 = (((-_Time.y) * _ScrollSpeed) + in_f.texcoord1.x);
          u_xlat1_d.xyz = float3((float3(u_xlat12, u_xlat12, u_xlat12) + float3(0, (-0.333333343), 0.333333343)));
          u_xlat1_d.xyz = frac(u_xlat1_d.xyz);
          u_xlat1_d.xyz = (((-u_xlat1_d.xyz) * float3(2, 2, 2)) + float3(1, 1, 1));
          u_xlat1_d.xyz = ((abs(u_xlat1_d.xyz) * float3(3, 3, 3)) + float3(-1, (-1), (-1)));
          #ifdef UNITY_ADRENO_ES3
          u_xlat1_d.xyz = min(max(u_xlat1_d.xyz, 0), 1);
          #else
          u_xlat1_d.xyz = clamp(u_xlat1_d.xyz, 0, 1);
          #endif
          u_xlat1_d.xyz = (u_xlat1_d.xyz * float3(5, 5, 5));
          u_xlat1_d.xyz = floor(u_xlat1_d.xyz);
          u_xlat0_d.xyz = ((u_xlat1_d.xyz * float3(0.25, 0.25, 0.25)) + u_xlat0_d.xyz);
          u_xlat1_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _PlaneTexture);
          u_xlat10_1.xyz = tex2D(_PlaneTexture, u_xlat1_d.xy).xyz;
          u_xlat16_12 = dot(u_xlat10_1.xyz, float3(0.300000012, 0.589999974, 0.109999999));
          u_xlat0_d.xyz = (u_xlat0_d.xyz + float3(u_xlat16_12, u_xlat16_12, u_xlat16_12));
          u_xlat1_d.xyz = (float3(u_xlat16_12, u_xlat16_12, u_xlat16_12) + (-_ShadowColor.xyz));
          u_xlat12 = (in_f.texcoord2.x + (-conv_mxt4x4_3(unity_ObjectToWorld)));
          u_xlat12 = (u_xlat12 + in_f.texcoord2.z);
          u_xlat12 = (u_xlat12 + (-conv_mxt4x4_3(unity_ObjectToWorld))).x;
          u_xlat12 = sin(u_xlat12);
          u_xlat2_d.xyz = normalize(in_f.texcoord3.xyz);
          u_xlat13 = dot(u_xlat2_d.xyz, _LightDirection.xyz);
          u_xlat13 = ((-abs(u_xlat13)) + 1);
          u_xlat3.y = ((u_xlat13 * u_xlat12) + _LightDirection.y);
          u_xlat3.xz = _LightDirection.xz;
          u_xlat3.xyz = normalize(u_xlat3.xyz);
          u_xlat12 = dot(u_xlat3.xyz, u_xlat2_d.xyz);
          u_xlat12 = (u_xlat12 + 1);
          u_xlat12 = (u_xlat12 * 0.5);
          u_xlat12 = round(u_xlat12);
          u_xlat13 = ((-_ShadowIntensity) + 1);
          u_xlat14 = ((-u_xlat13) + 1);
          u_xlat12 = ((u_xlat12 * u_xlat14) + u_xlat13);
          u_xlat1_d.xyz = ((float3(u_xlat12, u_xlat12, u_xlat12) * u_xlat1_d.xyz) + _ShadowColor.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.xyz + (-u_xlat1_d.xyz));
          u_xlat3.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _TextureMask);
          u_xlat10_12 = tex2D(_TextureMask, u_xlat3.xy).x;
          u_xlat0_d.xyz = ((float3(u_xlat10_12, u_xlat10_12, u_xlat10_12) * u_xlat0_d.xyz) + u_xlat1_d.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xyz) + _PlaneWaterColor.xyz);
          u_xlat13 = (_PlaneWaterColor.w * _WaterLevel);
          u_xlat0_d.xyz = ((float3(u_xlat13, u_xlat13, u_xlat13) * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          u_xlat1_d.xyz = ((-in_f.texcoord2.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat1_d.xyz = normalize(u_xlat1_d.xyz);
          u_xlat1_d.x = dot(u_xlat2_d.xyz, u_xlat1_d.xyz);
          u_xlat1_d.x = max(u_xlat1_d.x, 0);
          u_xlat1_d.x = ((-u_xlat1_d.x) + 1);
          u_xlat1_d.x = log2(u_xlat1_d.x);
          u_xlat1_d.x = (u_xlat1_d.x * _ShineIntensity);
          u_xlat1_d.x = exp2(u_xlat1_d.x);
          u_xlat1_d.x = (u_xlat1_d.x * _ShineSteps);
          u_xlat1_d.x = floor(u_xlat1_d.x);
          u_xlat5 = (_ShineSteps + (-1));
          u_xlat1_d.x = (u_xlat1_d.x / u_xlat5);
          u_xlat1_d.x = (u_xlat1_d.x * _ShineOpacity);
          out_f.color.xyz = ((float3(u_xlat10_12, u_xlat10_12, u_xlat10_12) * u_xlat1_d.xxx) + u_xlat0_d.xyz);
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
