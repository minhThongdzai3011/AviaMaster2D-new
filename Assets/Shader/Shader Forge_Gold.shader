Shader "Shader Forge/Gold"
{
  Properties
  {
    _TextureMask ("TextureMask", 2D) = "white" {}
    _PlaneTexture ("PlaneTexture", 2D) = "white" {}
    _PlaneColor ("PlaneColor", Color) = (1,0.9696857,0,1)
    _PlaneColorIntensity ("PlaneColorIntensity", Range(0, 1.5)) = 0.7051282
    _PlaneTextureTint ("PlaneTextureTint", Color) = (0,0,0,1)
    _ShineOpacity ("ShineOpacity", Range(0, 1)) = 0.3792089
    _ShineIntensity ("ShineIntensity", Range(0.0001, 1.5)) = 1.014135
    _ShineSteps ("ShineSteps", Range(0, 15)) = 4.766854
    _HighlightAngle ("HighlightAngle", Range(0, 1)) = 0.5683761
    _HighlightSize ("HighlightSize", Range(0.1, 0.5)) = 0.2975459
    _HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0.2689313
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
      //uniform float3 _WorldSpaceCameraPos;
      uniform float _ShineIntensity;
      uniform float _ShineSteps;
      uniform float _HighlightAngle;
      uniform float _HighlightSize;
      uniform float4 _TextureMask_ST;
      uniform float4 _PlaneColor;
      uniform float _ShineOpacity;
      uniform float4 _PlaneTexture_ST;
      uniform float4 _PlaneTextureTint;
      uniform float _PlaneColorIntensity;
      uniform float _HighlightIntensity;
      uniform float4 _PlaneWaterColor;
      uniform float _WaterLevel;
      uniform sampler2D _PlaneTexture;
      uniform sampler2D _TextureMask;
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
      int u_xlatb0;
      float3 u_xlat1_d;
      float3 u_xlat10_1;
      float3 u_xlat2;
      float3 u_xlat3;
      float u_xlat16_3;
      float3 u_xlat10_3;
      float2 u_xlat6_d;
      float u_xlat9;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (in_f.texcoord2.xy + float2(-0.5, (-0.5)));
          u_xlat6_d.x = (_HighlightAngle * 6.28318548);
          u_xlat1_d.y = cos(u_xlat6_d.x);
          u_xlat1_d.x = sin((-u_xlat6_d.x));
          u_xlat0_d.x = dot(u_xlat0_d.xy, u_xlat1_d.xy);
          u_xlat0_d.x = (u_xlat0_d.x + 0.5);
          u_xlat0_d.x = (u_xlat0_d.x + (-_Time.y));
          u_xlat0_d.x = (u_xlat0_d.x / _HighlightSize);
          u_xlat0_d.x = sin(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x + u_xlat0_d.x);
          u_xlat0_d.x = floor(u_xlat0_d.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0 = (u_xlat0_d.x>=1);
          #else
          u_xlatb0 = (u_xlat0_d.x>=1);
          #endif
          u_xlat0_d.x = (u_xlatb0)?(1):(float(0));
          u_xlat0_d.x = (u_xlat0_d.x * _HighlightIntensity);
          u_xlat3.xy = TRANSFORM_TEX(in_f.texcoord.xy, _PlaneTexture);
          u_xlat10_3.xyz = tex2D(_PlaneTexture, u_xlat3.xy).xyz;
          u_xlat16_3 = dot(u_xlat10_3.xyz, float3(0.300000012, 0.589999974, 0.109999999));
          u_xlat6_d.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _TextureMask);
          u_xlat10_1.xyz = tex2D(_TextureMask, u_xlat6_d.xy).xyz;
          u_xlat2.xyz = ((-u_xlat10_1.xyz) + _PlaneTextureTint.xyz);
          u_xlat3.xyz = ((u_xlat2.xyz * float3(_PlaneColorIntensity, _PlaneColorIntensity, _PlaneColorIntensity)) + float3(u_xlat16_3, u_xlat16_3, u_xlat16_3));
          u_xlat3.xyz = ((_PlaneColor.xyz * u_xlat10_1.xyz) + u_xlat3.xyz);
          u_xlat0_d.xyz = ((u_xlat0_d.xxx * u_xlat10_1.xyz) + u_xlat3.xyz);
          u_xlat1_d.xyz = ((-u_xlat0_d.xyz) + _PlaneWaterColor.xyz);
          u_xlat9 = (_PlaneWaterColor.w * _WaterLevel);
          u_xlat0_d.xyz = ((float3(u_xlat9, u_xlat9, u_xlat9) * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          u_xlat1_d.xyz = ((-in_f.texcoord3.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat1_d.xyz = normalize(u_xlat1_d.xyz);
          u_xlat2.xyz = normalize(in_f.texcoord4.xyz);
          u_xlat9 = dot(u_xlat2.xyz, u_xlat1_d.xyz);
          u_xlat9 = max(u_xlat9, 0);
          u_xlat9 = ((-u_xlat9) + 1);
          u_xlat9 = log2(u_xlat9);
          u_xlat9 = (u_xlat9 * _ShineIntensity);
          u_xlat9 = exp2(u_xlat9);
          u_xlat9 = (u_xlat9 * _ShineSteps);
          u_xlat9 = floor(u_xlat9);
          u_xlat1_d.x = (_ShineSteps + (-1));
          u_xlat9 = (u_xlat9 / u_xlat1_d.x);
          out_f.color.xyz = ((float3(u_xlat9, u_xlat9, u_xlat9) * float3(_ShineOpacity, _ShineOpacity, _ShineOpacity)) + u_xlat0_d.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
