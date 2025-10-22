Shader "Shader Forge/CaveEntrance"
{
  Properties
  {
    _MainTex ("Main Tex", 2D) = "white" {}
    _TextureMask ("TextureMask", 2D) = "white" {}
    _LightColor ("LightColor", Color) = (0,0.9682536,1,1)
    _ShineSpeed ("ShineSpeed", Range(0.5, 2)) = 1.305214
    _LightIntensity ("LightIntensity", Range(0, 1)) = 0.2795035
    _LightOpacity ("LightOpacity", Range(0, 1)) = 0.7319769
    _LightAmbient ("LightAmbient", Range(0, 1)) = 0
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
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      uniform float4 _MainTex_ST;
      uniform float4 _TextureMask_ST;
      uniform float4 _LightColor;
      uniform float _ShineSpeed;
      uniform float _LightIntensity;
      uniform float _LightOpacity;
      uniform float _LightAmbient;
      uniform sampler2D _MainTex;
      uniform sampler2D _TextureMask;
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
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float2 texcoord1 :TEXCOORD1;
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
          out_v.texcoord1.xy = in_v.texcoord1.xy;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float2 u_xlat1_d;
      float3 u_xlat10_1;
      float2 u_xlat2;
      float3 u_xlat10_2;
      float u_xlat9;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = float2((in_f.texcoord1.x + float(-0.5)), (in_f.texcoord1.y + float(-0.5)));
          u_xlat0_d.x = dot(u_xlat0_d.xy, float2(0.0442426763, (-0.999020815)));
          u_xlat0_d.x = (u_xlat0_d.x + 0.5);
          u_xlat0_d.x = ((u_xlat0_d.x * 1.79999995) + (-0.800000012));
          u_xlat0_d.x = (u_xlat0_d.x * _LightOpacity);
          u_xlat0_d.xyz = (u_xlat0_d.xxx * _LightColor.xyz);
          u_xlat9 = (_Time.z * _ShineSpeed);
          u_xlat9 = sin((-u_xlat9));
          u_xlat9 = (u_xlat9 + 1);
          u_xlat9 = ((u_xlat9 * _LightIntensity) + _LightAmbient);
          u_xlat0_d.xyz = (float3(u_xlat9, u_xlat9, u_xlat9) * u_xlat0_d.xyz);
          u_xlat1_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _MainTex);
          u_xlat10_1.xyz = tex2D(_MainTex, u_xlat1_d.xy).xyz;
          u_xlat2.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _TextureMask);
          u_xlat10_2.xyz = tex2D(_TextureMask, u_xlat2.xy).xyz;
          out_f.color.xyz = ((u_xlat10_2.xyz * u_xlat0_d.xyz) + u_xlat10_1.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
