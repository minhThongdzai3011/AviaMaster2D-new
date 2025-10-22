Shader "Fire Flush/UI/Shine"
{
  Properties
  {
    _MainTex ("MainTex", 2D) = "white" {}
    _ShineTex ("ShineTex", 2D) = "white" {}
    _ShineColor ("ShineColor", Color) = (0.5,0.5,0.5,1)
    _ScrollSpeed ("ScrollSpeed", Range(0, 2)) = 1.000678
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
    [MaterialToggle] PixelSnap ("Pixel snap", float) = 0
    [HideInInspector] _Stencil ("Stencil ID", float) = 1
  }
  SubShader
  {
    Tags
    { 
      "CanUseSpriteAtlas" = "true"
      "IGNOREPROJECTOR" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "CanUseSpriteAtlas" = "true"
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDBASE"
        "PreviewType" = "Plane"
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
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _ProjectionParams;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixV;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      uniform float _ScrollSpeed;
      uniform float4 _ShineTex_ST;
      uniform float4 _MainTex_ST;
      uniform float4 _ShineColor;
      uniform sampler2D _ShineTex;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
          float4 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float4 color :COLOR0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float u_xlat2;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = mul(unity_ObjectToWorld, float4(in_v.vertex.xyz,1.0));
          u_xlat1 = mul(unity_MatrixVP, u_xlat0);
          out_v.vertex = u_xlat1;
          out_v.texcoord.xy = in_v.texcoord.xy;
          out_v.color = in_v.color;
          u_xlat2 = (u_xlat0.y * conv_mxt4x4_1(unity_MatrixV).z);
          u_xlat0.x = ((conv_mxt4x4_0(unity_MatrixV).z * u_xlat0.x) + u_xlat2);
          u_xlat0.x = ((conv_mxt4x4_2(unity_MatrixV).z * u_xlat0.z) + u_xlat0.x);
          u_xlat0.x = ((conv_mxt4x4_3(unity_MatrixV).z * u_xlat0.w) + u_xlat0.x);
          out_v.texcoord1.z = (-u_xlat0.x);
          u_xlat0.x = (u_xlat1.y * _ProjectionParams.x);
          u_xlat0.w = (u_xlat0.x * 0.5);
          u_xlat0.xz = (u_xlat1.xw * float2(0.5, 0.5));
          out_v.texcoord1.w = u_xlat1.w;
          out_v.texcoord1.xy = (u_xlat0.zz + u_xlat0.xw);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float2 u_xlat0_d;
      float3 u_xlat10_0;
      float3 u_xlat1_d;
      float3 u_xlat2_d;
      float u_xlat6;
      float u_xlat9;
      float u_xlat16_9;
      float u_xlat10_9;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (in_f.texcoord1.xy / in_f.texcoord1.ww);
          u_xlat6 = (_Time.y * _ScrollSpeed);
          u_xlat0_d.xy = ((float2(u_xlat6, u_xlat6) * float2(1, 0)) + u_xlat0_d.xy);
          u_xlat0_d.xy = TRANSFORM_TEX(u_xlat0_d.xy, _ShineTex);
          u_xlat10_0.xyz = tex2D(_ShineTex, u_xlat0_d.xy).xyz;
          u_xlat1_d.xy = TRANSFORM_TEX(in_f.texcoord.xy, _MainTex);
          u_xlat10_9 = tex2D(_MainTex, u_xlat1_d.xy).w;
          u_xlat16_9 = (u_xlat10_9 * u_xlat10_0.x);
          u_xlat9 = (u_xlat16_9 * _ShineColor.w);
          out_f.color.w = (u_xlat9 * in_f.color.w);
          u_xlat1_d.xyz = (in_f.color.xyz * _ShineColor.xyz);
          u_xlat2_d.xyz = (((-in_f.color.xyz) * _ShineColor.xyz) + _ShineColor.xyz);
          out_f.color.xyz = ((u_xlat10_0.xyz * u_xlat2_d.xyz) + u_xlat1_d.xyz);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
