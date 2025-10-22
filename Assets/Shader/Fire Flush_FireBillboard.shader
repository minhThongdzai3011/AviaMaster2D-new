Shader "Fire Flush/FireBillboard"
{
    Properties
    {
        _Dissolve ("Dissolve", 2D) = "white" {}
        _ShapeX ("ShapeX", Float) = 0.1
        _ShapeY ("ShapeY", Float) = 2.0
        _Fresnel ("Fresnel", 2D) = "white" {}
        [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags
        { 
            "Queue" = "AlphaTest"
            "RenderType" = "TransparentCutout"
        }
        
        Pass // Forward pass
        {
            Name "FORWARD"
            Tags
            { 
                "LightMode" = "ForwardBase"
                "Queue" = "AlphaTest"
                "RenderType" = "TransparentCutout"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            
            // Properties
            uniform float _ShapeX;
            uniform float _ShapeY;
            uniform sampler2D _Dissolve;
            uniform float4 _Dissolve_ST;
            uniform sampler2D _Fresnel;
            uniform float4 _Fresnel_ST;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 color : COLOR;
                LIGHTING_COORDS(2, 3)
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                
                // Billboard effect calculation
                float3 billboardOffset = float3(v.normal.xy, 0);
                float dissolveAmount = 1.0 - v.color.a;
                float3 shapeOffset = float3(
                    dissolveAmount * _ShapeX,
                    dissolveAmount * _ShapeY,
                    dissolveAmount * v.normal.z
                );
                
                float3 worldPos = v.vertex.xyz + billboardOffset * shapeOffset;
                o.pos = UnityObjectToClipPos(float4(worldPos, 1.0));
                
                o.uv = v.texcoord;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.color = v.color;
                
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample dissolve texture
                float2 dissolveUV = TRANSFORM_TEX(i.uv, _Dissolve);
                float dissolve = tex2D(_Dissolve, dissolveUV).r;
                
                // Sample fresnel texture
                float2 fresnelUV = TRANSFORM_TEX(i.uv, _Fresnel);
                float fresnel = tex2D(_Fresnel, fresnelUV).r;
                
                // Calculate alpha cutoff
                float alpha = dissolve + i.color.a - 0.5;
                alpha = alpha * fresnel - 0.5;
                
                // Alpha test
                clip(alpha);
                
                // Final color with slight brightness boost
                fixed3 finalColor = i.color.rgb + 0.15;
                
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
        
        Pass // Shadow caster pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            
            #include "UnityCG.cginc"
            
            // Properties
            uniform float _ShapeX;
            uniform float _ShapeY;
            uniform sampler2D _Dissolve;
            uniform float4 _Dissolve_ST;
            uniform sampler2D _Fresnel;
            uniform float4 _Fresnel_ST;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
                float4 color : COLOR;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                
                // Billboard effect calculation (same as forward pass)
                float3 billboardOffset = float3(v.normal.xy, 0);
                float dissolveAmount = 1.0 - v.color.a;
                float3 shapeOffset = float3(
                    dissolveAmount * _ShapeX,
                    dissolveAmount * _ShapeY,
                    dissolveAmount * v.normal.z
                );
                
                v.vertex.xyz += billboardOffset * shapeOffset;
                
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                
                o.uv = v.texcoord;
                o.color = v.color;
                
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                // Sample dissolve texture
                float2 dissolveUV = TRANSFORM_TEX(i.uv, _Dissolve);
                float dissolve = tex2D(_Dissolve, dissolveUV).r;
                
                // Sample fresnel texture
                float2 fresnelUV = TRANSFORM_TEX(i.uv, _Fresnel);
                float fresnel = tex2D(_Fresnel, fresnelUV).r;
                
                // Calculate alpha cutoff (same as forward pass)
                float alpha = dissolve + i.color.a - 0.5;
                alpha = alpha * fresnel - 0.5;
                
                // Alpha test
                clip(alpha);
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    
    FallBack "Transparent/Cutout/VertexLit"
}