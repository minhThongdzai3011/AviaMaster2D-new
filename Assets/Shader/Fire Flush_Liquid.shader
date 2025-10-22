Shader "Fire Flush/Liquid"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0.6768491, 0.7616133, 0.8602941, 1)
        _Color ("Fill Color", Color) = (0.07843138, 0.3921569, 0.7843137, 1)
        _EdgeSize ("Edge Size", Range(0, 5)) = 0.1282052
        _FillAmount ("Fill Amount", Range(-10, 10)) = 0.03485786
        _ObjectUp ("Object Up Vector", Vector) = (1, 1, 0, 0)
        _WavesSpeed ("Waves Speed", Range(-50, 50)) = 0
        _WavesAmountX ("Waves Amount X", float) = 10
        _WavesAmountY ("Waves Amount Y", float) = 0
        _WaveAmplitudeMin ("Wave Amplitude Min", Range(0, 1)) = 0
        _WaveAmplitudeMax ("Wave Amplitude Max", Range(0, 1)) = 0.04273505
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest"
            "RenderType" = "TransparentCutout"
        }
        
        // Forward rendering pass
        Pass
        {
            Name "FORWARD"
            Tags
            {
                "LightMode" = "ForwardBase"
                "Queue" = "AlphaTest"
                "RenderType" = "TransparentCutout"
                "ShadowSupport" = "true"
            }
            
            CGPROGRAM
            #pragma multi_compile DIRECTIONAL
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            uniform float4 _Color;
            uniform float4 _EdgeColor;
            uniform float _EdgeSize;
            uniform float4 _ObjectUp;
            uniform float _FillAmount;
            uniform float _WaveAmplitudeMax;
            uniform float _WavesAmountX;
            uniform float _WavesAmountY;
            uniform float _WavesSpeed;
            uniform float _WaveAmplitudeMin;
            uniform float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Transform world position to object space
                float3 objectPos = mul(unity_WorldToObject, float4(i.worldPos, 1.0)).xyz;

                // Normalize up vector
                float3 up = normalize(_ObjectUp.xyz);
                float upLength = length(_ObjectUp.xyz);
                if (upLength < 0.0001) up = float3(0, 1, 0); // Fallback to avoid division by zero

                // Calculate wave effect
                float2 uv = objectPos.yz; // Use yz plane for wave calculation
                float wave = sin((_WavesAmountX * objectPos.x + _WavesAmountY * objectPos.y + _WavesSpeed * _Time.y));
                float amplitude = lerp(_WaveAmplitudeMin, _WaveAmplitudeMax, saturate(abs(up.x)));

                // Project position along up vector
                float height = dot(objectPos, up);
                float signedDistance = height + wave * amplitude - _FillAmount;

                // Alpha test
                if (signedDistance < -_EdgeSize)
                    discard;

                // Color blending
                float edgeFactor = saturate(signedDistance / _EdgeSize);
                float3 finalColor = lerp(_EdgeColor.rgb, _Color.rgb, edgeFactor);

                // Apply alpha cutoff
                if (edgeFactor < _Cutoff)
                    discard;

                return float4(finalColor, 1.0);
            }
            ENDCG
        }

        // Shadow caster pass
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
                "Queue" = "AlphaTest"
                "RenderType" = "TransparentCutout"
            }
            Offset 1, 1

            CGPROGRAM
            #pragma multi_compile SHADOWS_DEPTH
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            uniform float4 _ObjectUp;
            uniform float _FillAmount;
            uniform float _WaveAmplitudeMax;
            uniform float _WavesAmountX;
            uniform float _WavesAmountY;
            uniform float _WavesSpeed;
            uniform float _WaveAmplitudeMin;
            uniform float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = worldPos.xyz;

                // Apply shadow bias
                float4 clipPos = o.pos;
                float bias = max(unity_LightShadowBias.x / clipPos.w, 0.0);
                float z = max(clipPos.z / clipPos.w, -1.0) + bias;
                clipPos.z = z * clipPos.w;
                clipPos.z += unity_LightShadowBias.y * (z - (clipPos.z / clipPos.w));
                o.pos = clipPos;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Transform world position to object space
                float3 objectPos = mul(unity_WorldToObject, float4(i.worldPos, 1.0)).xyz;

                // Normalize up vector
                float3 up = normalize(_ObjectUp.xyz);
                float upLength = length(_ObjectUp.xyz);
                if (upLength < 0.0001) up = float3(0, 1, 0); // Fallback to avoid division by zero

                // Calculate wave effect
                float2 uv = objectPos.yz;
                float wave = sin((_WavesAmountX * objectPos.x + _WavesAmountY * objectPos.y + _WavesSpeed * _Time.y));
                float amplitude = lerp(_WaveAmplitudeMin, _WaveAmplitudeMax, saturate(abs(up.x)));

                // Project position along up vector
                float height = dot(objectPos, up);
                float signedDistance = height + wave * amplitude - _FillAmount;

                // Alpha test
                if (signedDistance < 0.0)
                    discard;

                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}