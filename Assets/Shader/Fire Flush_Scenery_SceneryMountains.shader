Shader "Fire Flush/Scenery/SceneryMountains"
{
  Properties
  {
    [PerRendererData] _MainTex ("MainTex", 2D) = "white" {}
    _BaseColor ("BaseColor", Color) = (0.5,0.5,0.5,1)
    _SecondaryColor ("SecondaryColor", Color) = (0.5,0.5,0.5,1)
    _ThirdColor ("ThirdColor", Color) = (0.5,0.5,0.5,1)
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
    [MaterialToggle] PixelSnap ("Pixel snap", float) = 0
    _Stencil ("Stencil ID", float) = 0
    _StencilReadMask ("Stencil Read Mask", float) = 255
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilComp ("Stencil Comparison", float) = 8
    _StencilOp ("Stencil Operation", float) = 0
    _StencilOpFail ("Stencil Fail Operation", float) = 0
    _StencilOpZFail ("Stencil Z-Fail Operation", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "CanUseSpriteAtlas" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "AlphaTest"
      "RenderType" = "TransparentCutout"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "CanUseSpriteAtlas" = "true"
        "LIGHTMODE" = "FORWARDBASE"
        "PreviewType" = "Plane"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
        "SHADOWSUPPORT" = "true"
      }
      Cull Off
      Stencil
      { 
        Ref 0
        ReadMask 0
        WriteMask 0
        Pass Keep
        Fail Keep
        ZFail Keep
        PassFront Keep
        FailFront Keep
        ZFailFront Keep
        PassBack Keep
        FailBack Keep
        ZFailBack Keep
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
      uniform float4 _MainTex_ST;
      uniform float4 _BaseColor;
      uniform float4 _SecondaryColor;
      uniform float4 _ThirdColor;
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
      float4 u_xlat0_d;
      float3 u_xlat10_0;
      float3 u_xlat1_d;
      float u_xlat6;
      float u_xlat16_6;
      int u_xlatb6;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _MainTex);
          u_xlat10_0.xyz = tex2D(_MainTex, u_xlat0_d.xy).xyz;
          u_xlat16_6 = (u_xlat10_0.y + u_xlat10_0.x);
          u_xlat6 = (u_xlat10_0.z + u_xlat16_6);
          u_xlat6 = floor(u_xlat6);
          u_xlat6 = (u_xlat6 + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb6 = (u_xlat6<0);
          #else
          u_xlatb6 = (u_xlat6<0);
          #endif
          if(((int(u_xlatb6) * int(4294967295))!=0))
          {
              discard;
          }
          u_xlat1_d.xyz = (u_xlat10_0.yyy * _SecondaryColor.xyz);
          u_xlat0_d.xyw = ((u_xlat10_0.xxx * _BaseColor.xyz) + u_xlat1_d.xyz);
          out_f.color.xyz = ((u_xlat10_0.zzz * _ThirdColor.xyz) + u_xlat0_d.xyw);
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
        "CanUseSpriteAtlas" = "true"
        "LIGHTMODE" = "SHADOWCASTER"
        "PreviewType" = "Plane"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
        "SHADOWSUPPORT" = "true"
      }
      Cull Off
      Offset 1, 1
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile SHADOWS_DEPTH
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord1 :TEXCOORD1;
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
          u_xlat0 = UnityObjectToClipPos(in_v.vertex);
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
      float u_xlat16_0;
      float3 u_xlat10_0;
      int u_xlatb0;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = TRANSFORM_TEX(in_f.texcoord1.xy, _MainTex);
          u_xlat10_0.xyz = tex2D(_MainTex, u_xlat0_d.xy).xyz;
          u_xlat16_0 = (u_xlat10_0.y + u_xlat10_0.x);
          u_xlat0_d.x = (u_xlat10_0.z + u_xlat16_0);
          u_xlat0_d.x = floor(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x + (-0.5));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb0 = (u_xlat0_d.x<0);
          #else
          u_xlatb0 = (u_xlat0_d.x<0);
          #endif
          if(((int(u_xlatb0) * int(4294967295))!=0))
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
