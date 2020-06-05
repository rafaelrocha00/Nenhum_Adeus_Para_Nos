Shader "Custom/Sand"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_BumpMap0("Normal Map Dry", 2D) = "bump" {}
        [NoScaleOffset]_BumpMap1("Normal Map Wet", 2D) = "bump" {}
        [NoScaleOffset]_BumpMap2("Normal Map Slope", 2D) = "bump" {}
        [NoScaleOffset]_Noise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(2)
                fixed3 ambient : COLOR0;
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD3;
                float4 screenPosition : TEXCOORD4;
                float3 wPos : TEXCOORD5;
                
                half3 tspace0 : TEXCOORD6; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD7; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD8; // tangent.z, bitangent.z, normal.z
            };
            
            sampler2D _MainTex;
            sampler2D _Noise;
            sampler2D _BumpMap0;
            sampler2D _BumpMap1;
            sampler2D _BumpMap2;
            float4 _MainTex_ST;
            float4 _Color;
            
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.ambient = ShadeSH9(half4(o.worldNormal,1));
                o.viewDir = WorldSpaceViewDir (v.vertex);
                o.screenPosition = ComputeScreenPos(o.pos);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(o.worldNormal, wTangent) * tangentSign;
                
                o.tspace0 = half3(wTangent.x, wBitangent.x, o.worldNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, o.worldNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, o.worldNormal.z);
                
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed3 frag (v2f i) : SV_Target
            {
                float k = 2 * UNITY_PI;          
                float heightSin = 0;
                
                float wetMask = pow(saturate(abs((i.wPos.y - 2) * 0.25)), 1);
                
                i.wPos.y -= sin(_Time.y - 0.5) * 1 + 1;
                
                heightSin += sin(k * i.wPos.x * 0.05 /*- _Time.y*/) * 0.2;
                //heightSin += sin(k * i.wPos.z * 0.3 /*- _Time.y*/) * 0.1;           
                //heightSin += cos(k * (i.wPos.z + i.wPos.x) * 0.2 /*- _Time.y*/) * 0.05 * cos(k * i.wPos.z * 0.5 /*- _Time.y*/);
                
                i.wPos.y += heightSin;
                float mask = pow(saturate(i.wPos.y), 2);
                float mask2 = saturate(i.wPos.y + 1.3);
            
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col = lerp(col * 0.1, col, mask);
                col *= _LightColor0;
                fixed shadow = SHADOW_ATTENUATION(i);
                
                i.viewDir = normalize(i.viewDir);
                i.worldNormal = normalize(i.worldNormal);
                
                half3 tnormalDry = UnpackNormal(tex2D(_BumpMap0, i.uv));
                half3 tnormalWet = UnpackNormal(tex2D(_BumpMap1, i.uv));
                half3 tnormalSlope = UnpackNormal(tex2D(_BumpMap2, i.uv));
                
                tnormalDry = lerp(tnormalSlope, tnormalDry, pow(abs(i.worldNormal.y), 2));
                half3 tnormal = lerp(tnormalWet, tnormalDry, wetMask);
                
                i.worldNormal.x = dot(i.tspace0, tnormal);
                i.worldNormal.y = dot(i.tspace1, tnormal);
                i.worldNormal.z = dot(i.tspace2, tnormal);

                float2 screenUv = i.screenPosition.xy / i.screenPosition.w;
                
                float diff = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                
                float2 screenUvScale = _ScreenParams.xy / 128;
                float3 noise = normalize(tex2D(_Noise, screenUv * screenUvScale) * 2 - 1);
                noise = reflect(_WorldSpaceLightPos0.xyz, noise);
                float glitter = max(0, dot(noise, i.viewDir));
                if(glitter > 0.5)
                    glitter = 0;
                
                glitter = 1 - glitter;
                
                float specPow = lerp(256, 32, wetMask);
                float3 halfDir = normalize(i.worldNormal + i.viewDir);
                float specular = max(0, dot(halfDir, i.worldNormal));
                specular = pow(specular, specPow) * shadow * diff * glitter * 3;
                
                float3 specColor = _LightColor0;
                
                fixed3 lighting = diff * shadow + i.ambient;
                col.rgb *= lighting;
                return col + specular * specColor * (1 - saturate(mask - 0.2) * mask2);
            }
            ENDCG
        }
        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}