Shader "Fire Flush/WaterWaves"
{
  Properties
  {
    _FlatColor ("FlatColor", Color) = (0.5,0.5,0.5,1)
    _EdgeColor ("EdgeColor", Color) = (0.5,0.5,0.5,1)
    _VerticalColor ("VerticalColor", Color) = (0.5,0.5,0.5,1)
    _WaveSpeed ("WaveSpeed", float) = 1
    _WaveCount ("WaveCount", float) = 1
    _WaveSize ("WaveSize", float) = 1
    _EdgeSize ("EdgeSize", float) = 0.1
    _EdgeSizeMax ("EdgeSizeMax", float) = 0.1
    _SecondaryWaveSpeed ("SecondaryWaveSpeed", float) = 1
    _SecondaryWaveCount ("SecondaryWaveCount", float) = 1
    _SecondaryWaveSize ("SecondaryWaveSize", float) = 1
    _DiveSize ("DiveSize", float) = 2
    _DiveDistanceStart ("DiveDistanceStart", float) = 5
    _DiveDistanceEnd ("DiveDistanceEnd", float) = 5
    _DiveCountStart ("DiveCountStart", float) = 3
    _DiveCountEnd ("DiveCountEnd", float) = 5
    _DiveSpeed ("DiveSpeed", float) = 5
    _GlideSize ("GlideSize", float) = 0.33
    _GlideLengthMin ("GlideLengthMin", float) = 3
    _GlideLengthMax ("GlideLengthMax", float) = 6
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      uniform float4 _FlatColor;
      uniform float4 _EdgeColor;
      uniform float4 _VerticalColor;
      uniform float _WaveSpeed;
      uniform float _WaveCount;
      uniform float _WaveSize;
      uniform float _EdgeSize;
      uniform float _SecondaryWaveSpeed;
      uniform float _SecondaryWaveCount;
      uniform float _SecondaryWaveSize;
      uniform float4 _DivePosition;
      uniform float _DiveDistanceStart;
      uniform float _DiveSize;
      uniform float _DiveCountStart;
      uniform float _DiveCountEnd;
      uniform float _DiveSpeed;
      uniform float _DiveDistanceEnd;
      uniform float _DiveFactor;
      uniform float4 _GlidePosition;
      uniform float _GlideSize;
      uniform float _GlideLengthMin;
      uniform float _GlideLengthMax;
      uniform float _DiveProgress;
      uniform float _GlideFactor;
      uniform float _EdgeSizeMax;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
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
          out_v.texcoord1 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = in_v.texcoord.xy;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float4 u_xlat1_d;
      int u_xlatb1;
      float4 u_xlat2;
      float3 u_xlat3;
      float u_xlat4;
      float u_xlat6;
      int u_xlatb6;
      float u_xlat9;
      int u_xlatb9;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord1.xyz) + _DivePosition.xyz);
          u_xlat0_d.x = length(u_xlat0_d.xyz);
          u_xlat3.x = ((-_DiveDistanceStart) + _DiveDistanceEnd);
          u_xlat3.x = ((_DiveProgress * u_xlat3.x) + _DiveDistanceStart);
          u_xlat0_d.x = (u_xlat0_d.x / u_xlat3.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlat0_d.x = min(max(u_xlat0_d.x, 0), 1);
          #else
          u_xlat0_d.x = clamp(u_xlat0_d.x, 0, 1);
          #endif
          u_xlat3.xy = (u_xlat0_d.xx * float2(3.14159274, 6.28318548));
          u_xlat6 = cos(u_xlat3.y);
          u_xlat3.x = sin(u_xlat3.x);
          u_xlat6 = ((u_xlat6 * (-0.25)) + 0.75);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb9 = (0.5>=u_xlat0_d.x);
          #else
          u_xlatb9 = (0.5>=u_xlat0_d.x);
          #endif
          u_xlat9 = (u_xlatb9)?(1):(float(0));
          u_xlat6 = (u_xlat6 * u_xlat9);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb1 = (u_xlat0_d.x>=0.5);
          #else
          u_xlatb1 = (u_xlat0_d.x>=0.5);
          #endif
          u_xlat1_d.x = (u_xlatb1)?(1):(float(0));
          u_xlat6 = ((u_xlat1_d.x * u_xlat3.x) + u_xlat6);
          u_xlat9 = (u_xlat9 * u_xlat1_d.x);
          u_xlat3.x = ((-u_xlat6) + u_xlat3.x);
          u_xlat3.x = ((u_xlat9 * u_xlat3.x) + u_xlat6);
          u_xlat6 = ((-u_xlat0_d.x) + 1);
          u_xlat3.x = ((-u_xlat6) + u_xlat3.x);
          u_xlat3.x = ((_DiveProgress * u_xlat3.x) + u_xlat6);
          u_xlat6 = ((-_DiveCountStart) + _DiveCountEnd);
          u_xlat6 = ((_DiveProgress * u_xlat6) + _DiveCountStart);
          u_xlat9 = (_DiveSpeed * _DiveProgress);
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat6) + (-u_xlat9));
          u_xlat0_d.x = sin(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * _DiveSize);
          u_xlat6 = ((-_DiveProgress) + 1);
          u_xlat0_d.x = (u_xlat6 * u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat3.x * u_xlat0_d.x);
          u_xlat3.x = (in_f.texcoord1.x * _SecondaryWaveCount);
          u_xlat3.x = ((_Time.y * _SecondaryWaveSpeed) + u_xlat3.x);
          u_xlat3.x = sin(u_xlat3.x);
          u_xlat3.x = (u_xlat3.x * _SecondaryWaveSize);
          u_xlat6 = (in_f.texcoord1.x * _WaveCount);
          u_xlat6 = ((_Time.y * _WaveSpeed) + u_xlat6);
          u_xlat6 = sin(u_xlat6);
          u_xlat3.x = ((u_xlat6 * _WaveSize) + u_xlat3.x);
          u_xlat0_d.x = ((u_xlat0_d.x * _DiveFactor) + u_xlat3.x);
          u_xlat3.xyz = (in_f.texcoord1.xyz + (-_GlidePosition.xyz));
          u_xlat3.x = length(u_xlat3.xyz);
          u_xlat6 = ((-_GlideLengthMin) + _GlideLengthMax);
          u_xlat6 = ((_GlideFactor * u_xlat6) + _GlideLengthMin);
          u_xlat3.x = (u_xlat3.x / u_xlat6);
          u_xlat6 = (u_xlat3.x * 6.28318548);
          u_xlat3.x = u_xlat3.x;
          #ifdef UNITY_ADRENO_ES3
          u_xlat3.x = min(max(u_xlat3.x, 0), 1);
          #else
          u_xlat3.x = clamp(u_xlat3.x, 0, 1);
          #endif
          u_xlat3.x = ((-u_xlat3.x) + 1);
          u_xlat6 = sin(u_xlat6);
          u_xlat6 = ((u_xlat6 * 0.5) + 0.5);
          u_xlat3.x = (u_xlat6 * u_xlat3.x);
          u_xlat3.x = (u_xlat3.x * _GlideSize);
          u_xlat0_d.x = ((u_xlat3.x * _GlideFactor) + u_xlat0_d.x);
          u_xlat0_d.x = (((-u_xlat0_d.x) * in_f.texcoord.x) + in_f.texcoord.y);
          u_xlat3.x = (in_f.texcoord1.x + _Time.w);
          u_xlat3.x = (u_xlat3.x * 0.610000014);
          u_xlat3.x = sin(u_xlat3.x);
          u_xlat3.x = ((in_f.texcoord1.x * 0.5) + u_xlat3.x);
          u_xlat3.x = sin(u_xlat3.x);
          u_xlat3.x = (u_xlat3.x + 1);
          u_xlat6 = ((-_EdgeSize) + _EdgeSizeMax);
          u_xlat3.x = (u_xlat6 * u_xlat3.x);
          u_xlat3.x = ((u_xlat3.x * 0.5) + _EdgeSize);
          u_xlat0_d.z = (((-u_xlat3.x) * 0.5) + u_xlat0_d.x);
          u_xlat0_d.x = ((u_xlat3.x * 0.5) + u_xlat0_d.x);
          u_xlat0_d.xy = round(u_xlat0_d.xz);
          u_xlat6 = ((-u_xlat0_d.y) + 1);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb9 = (in_f.texcoord.y>=0.5);
          #else
          u_xlatb9 = (in_f.texcoord.y>=0.5);
          #endif
          u_xlat9 = (u_xlatb9)?(1):(float(0));
          #ifdef UNITY_ADRENO_ES3
          u_xlatb1 = (0.5>=in_f.texcoord.y);
          #else
          u_xlatb1 = (0.5>=in_f.texcoord.y);
          #endif
          u_xlat4 = (u_xlatb1)?(1):(float(0));
          u_xlat9 = (u_xlat9 * u_xlat4);
          u_xlat9 = (u_xlatb1)?(0):(u_xlat9);
          u_xlat9 = (u_xlat9 + u_xlat4);
          u_xlat6 = max(u_xlat9, u_xlat6);
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
          u_xlat1_d = (_EdgeColor + (-_VerticalColor));
          u_xlat1_d = ((u_xlat0_d.xxxx * u_xlat1_d) + _VerticalColor);
          u_xlat2 = ((-u_xlat1_d) + _FlatColor);
          out_f.color = ((u_xlat0_d.yyyy * u_xlat2) + u_xlat1_d);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: ShadowCaster
    {
      Name "ShadowCaster"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      Offset 1, 1
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile SHADOWS_DEPTH
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _Time;
      uniform float _WaveSpeed;
      uniform float _WaveCount;
      uniform float _WaveSize;
      uniform float _EdgeSize;
      uniform float _SecondaryWaveSpeed;
      uniform float _SecondaryWaveCount;
      uniform float _SecondaryWaveSize;
      uniform float4 _DivePosition;
      uniform float _DiveDistanceStart;
      uniform float _DiveSize;
      uniform float _DiveCountStart;
      uniform float _DiveCountEnd;
      uniform float _DiveSpeed;
      uniform float _DiveDistanceEnd;
      uniform float _DiveFactor;
      uniform float4 _GlidePosition;
      uniform float _GlideSize;
      uniform float _GlideLengthMin;
      uniform float _GlideLengthMax;
      uniform float _DiveProgress;
      uniform float _GlideFactor;
      uniform float _EdgeSizeMax;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord2 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          u_xlat0 = mul(unity_MatrixVP, u_xlat1);
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
      float3 u_xlat0_d;
      int u_xlatb0;
      float u_xlat1_d;
      int u_xlatb1;
      float3 u_xlat2;
      float2 u_xlatb2;
      float u_xlat4_d;
      float u_xlat6;
      int u_xlatb6;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord2.xyz) + _DivePosition.xyz);
          u_xlat0_d.x = length(u_xlat0_d.xyz);
          u_xlat2.x = ((-_DiveDistanceStart) + _DiveDistanceEnd);
          u_xlat2.x = ((_DiveProgress * u_xlat2.x) + _DiveDistanceStart);
          u_xlat0_d.x = (u_xlat0_d.x / u_xlat2.x);
          #ifdef UNITY_ADRENO_ES3
          u_xlat0_d.x = min(max(u_xlat0_d.x, 0), 1);
          #else
          u_xlat0_d.x = clamp(u_xlat0_d.x, 0, 1);
          #endif
          u_xlat2.xy = (u_xlat0_d.xx * float2(3.14159274, 6.28318548));
          u_xlat4_d = cos(u_xlat2.y);
          u_xlat2.x = sin(u_xlat2.x);
          u_xlat4_d = ((u_xlat4_d * (-0.25)) + 0.75);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb6 = (0.5>=u_xlat0_d.x);
          #else
          u_xlatb6 = (0.5>=u_xlat0_d.x);
          #endif
          u_xlat6 = (u_xlatb6)?(1):(float(0));
          u_xlat4_d = (u_xlat4_d * u_xlat6);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb1 = (u_xlat0_d.x>=0.5);
          #else
          u_xlatb1 = (u_xlat0_d.x>=0.5);
          #endif
          u_xlat1_d = (u_xlatb1)?(1):(float(0));
          u_xlat4_d = ((u_xlat1_d * u_xlat2.x) + u_xlat4_d);
          u_xlat6 = (u_xlat6 * u_xlat1_d);
          u_xlat2.x = ((-u_xlat4_d) + u_xlat2.x);
          u_xlat2.x = ((u_xlat6 * u_xlat2.x) + u_xlat4_d);
          u_xlat4_d = ((-u_xlat0_d.x) + 1);
          u_xlat2.x = ((-u_xlat4_d) + u_xlat2.x);
          u_xlat2.x = ((_DiveProgress * u_xlat2.x) + u_xlat4_d);
          u_xlat4_d = ((-_DiveCountStart) + _DiveCountEnd);
          u_xlat4_d = ((_DiveProgress * u_xlat4_d) + _DiveCountStart);
          u_xlat6 = (_DiveSpeed * _DiveProgress);
          u_xlat0_d.x = ((u_xlat0_d.x * u_xlat4_d) + (-u_xlat6));
          u_xlat0_d.x = sin(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * _DiveSize);
          u_xlat4_d = ((-_DiveProgress) + 1);
          u_xlat0_d.x = (u_xlat4_d * u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat2.x * u_xlat0_d.x);
          u_xlat2.x = (in_f.texcoord2.x * _SecondaryWaveCount);
          u_xlat2.x = ((_Time.y * _SecondaryWaveSpeed) + u_xlat2.x);
          u_xlat2.x = sin(u_xlat2.x);
          u_xlat2.x = (u_xlat2.x * _SecondaryWaveSize);
          u_xlat4_d = (in_f.texcoord2.x * _WaveCount);
          u_xlat4_d = ((_Time.y * _WaveSpeed) + u_xlat4_d);
          u_xlat4_d = sin(u_xlat4_d);
          u_xlat2.x = ((u_xlat4_d * _WaveSize) + u_xlat2.x);
          u_xlat0_d.x = ((u_xlat0_d.x * _DiveFactor) + u_xlat2.x);
          u_xlat2.xyz = (in_f.texcoord2.xyz + (-_GlidePosition.xyz));
          u_xlat2.x = length(u_xlat2.xyz);
          u_xlat4_d = ((-_GlideLengthMin) + _GlideLengthMax);
          u_xlat4_d = ((_GlideFactor * u_xlat4_d) + _GlideLengthMin);
          u_xlat2.x = (u_xlat2.x / u_xlat4_d);
          u_xlat4_d = (u_xlat2.x * 6.28318548);
          u_xlat2.x = u_xlat2.x;
          #ifdef UNITY_ADRENO_ES3
          u_xlat2.x = min(max(u_xlat2.x, 0), 1);
          #else
          u_xlat2.x = clamp(u_xlat2.x, 0, 1);
          #endif
          u_xlat2.x = ((-u_xlat2.x) + 1);
          u_xlat4_d = sin(u_xlat4_d);
          u_xlat4_d = ((u_xlat4_d * 0.5) + 0.5);
          u_xlat2.x = (u_xlat4_d * u_xlat2.x);
          u_xlat2.x = (u_xlat2.x * _GlideSize);
          u_xlat0_d.x = ((u_xlat2.x * _GlideFactor) + u_xlat0_d.x);
          u_xlat0_d.x = (((-u_xlat0_d.x) * in_f.texcoord1.x) + in_f.texcoord1.y);
          u_xlat2.x = (in_f.texcoord2.x + _Time.w);
          u_xlat2.x = (u_xlat2.x * 0.610000014);
          u_xlat2.x = sin(u_xlat2.x);
          u_xlat2.x = ((in_f.texcoord2.x * 0.5) + u_xlat2.x);
          u_xlat2.x = sin(u_xlat2.x);
          u_xlat2.x = (u_xlat2.x + 1);
          u_xlat4_d = ((-_EdgeSize) + _EdgeSizeMax);
          u_xlat2.x = (u_xlat4_d * u_xlat2.x);
          u_xlat2.x = ((u_xlat2.x * 0.5) + _EdgeSize);
          u_xlat0_d.x = (((-u_xlat2.x) * 0.5) + u_xlat0_d.x);
          u_xlat0_d.x = round(u_xlat0_d.x);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          #ifdef UNITY_ADRENO_ES3
          u_xlatb2.x = (in_f.texcoord1.y>=0.5);
          #else
          u_xlatb2.x = (in_f.texcoord1.y>=0.5);
          #endif
          #ifdef UNITY_ADRENO_ES3
          u_xlatb2.y = (0.5>=in_f.texcoord1.y);
          #else
          u_xlatb2.y = (0.5>=in_f.texcoord1.y);
          #endif
          u_xlat2.xz = lerp(float2(0, 0), float2(1, 1), float2(u_xlatb2.xy));
          u_xlat2.x = (u_xlat2.x * u_xlat2.z);
          u_xlat2.x = (u_xlatb2.y)?(0):(u_xlat2.x);
          u_xlat2.x = (u_xlat2.x + u_xlat2.z);
          u_xlat0_d.x = max(u_xlat2.x, u_xlat0_d.x);
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
