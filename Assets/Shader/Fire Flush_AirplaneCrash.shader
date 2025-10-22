Shader "Fire Flush/AirplaneCrash"
{
  Properties
  {
    _MainTex (" Main Tex", 2D) = "white" {}
    _CrashProgress ("CrashProgress", Range(0, 1)) = 0
    _Color ("Color", Color) = (1,0,0,1)
    _DimmedColor ("Dimmed Color", Color) = (0.5,0.5,0.5,1)
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
      uniform float4 _Color;
      uniform float _CrashProgress;
      uniform float4 _MainTex_ST;
      uniform float4 _DimmedColor;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
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
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlat10_0;
      float3 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _MainTex);
          u_xlat10_0.xyz = tex2D(_MainTex, u_xlat0_d.xy).xyz;
          u_xlat1_d.xyz = (u_xlat10_0.xyz * _DimmedColor.xyz);
          u_xlat0_d.xyz = (((-u_xlat10_0.xyz) * _DimmedColor.xyz) + _Color.xyz);
          out_f.color.xyz = ((float3(_CrashProgress, _CrashProgress, _CrashProgress) * u_xlat0_d.xyz) + u_xlat1_d.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
