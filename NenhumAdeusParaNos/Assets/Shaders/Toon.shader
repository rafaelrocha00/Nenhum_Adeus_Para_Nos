Shader "Custom/Toon"
{
    Properties
    {
        _Color ("Color", Color) = (0.8, 0.8, 0.8, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
        _Scale ("Outline Scale", float) = 0.01
        
        //_XRayColor ("X-Ray Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest"}
        LOD 100
        
        // occlusion
        Pass 
        {
            ZTest Greater
            ZTest Off
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                
                #include "UnityCG.cginc"
                
                struct v2f
                {
                    float4 pos : SV_POSITION;                  
                };
                
 
                v2f vert (appdata_base v)
                {
                    v2f o;   
                    o.pos = UnityObjectToClipPos(v.vertex);
                    return o;
                }
 
                //float4 _XRayColor;
 
                float4 frag(v2f i) : COLOR
                {
                    float checker = floor(i.pos.x) + floor(i.pos.y);
                    checker = frac(checker * 0.5) * 2;
                    clip(checker - 0.1);
                    return checker;// * _XRayColor;
                }
            ENDCG
        }
        
        Pass // outline
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(9)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz += v.normal * 0.01 * _Scale;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                float3 normal = UnityObjectToWorldNormal(v.normal);
                o.color = step(dot(normal, _WorldSpaceLightPos0), 0);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 col = i.color;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(col * 0.75, 1);
            }
            ENDCG
        }
        
        Pass // directional light
        {
            Tags {"LightMode"="ForwardBase"}
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(3)
                SHADOW_COORDS(4)
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD2;      
            };

            sampler2D _MainTex;
            sampler2D _Ramp;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o)
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            float3 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                i.normal = normalize(i.normal);
                i.viewDir = normalize(i.viewDir);
                
                float3 ambient = ShadeSH9(half4(i.normal,1));
                
                float shadow = SHADOW_ATTENUATION(i);
                
                float lambert = dot(i.normal, _WorldSpaceLightPos0) * 0.5 + 0.5;
                float4 warpLighting = tex2Dlod(_Ramp, float4(lambert * shadow, 0.5, 0, 0));
                
                float3 halfDir = normalize((i.normal + _WorldSpaceLightPos0) / 2);
                float specular =  dot(halfDir, i.normal);
                specular = pow(saturate(specular), 64);
                
                float rim = pow(saturate(dot(i.normal, float3(0, 1, 0))), 2);
                
                float fresnel = 1 - dot(i.normal, i.viewDir);
                fresnel = pow(fresnel, 3);
                
                specular = specular * saturate(fresnel + 0.1) * shadow;
                
                col.rgb = col * _Color;
                col.rgb = (col * _LightColor0 * warpLighting) + (ambient * col) + (rim * fresnel * ambient);
                col.rgb += specular;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
        
        Pass 
        {
            Tags {"LightMode" = "ForwardAdd"}
            Blend One One                                           
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdadd
                
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 lightDir : TEXCOORD2;
                    float3 normal : NORMAL;
                    float3 viewDir : TEXCOORD5;
                    LIGHTING_COORDS(3,4)                            
                };
                
                sampler2D _MainTex;
                sampler2D _Ramp;
                float4 _MainTex_ST;
                float4 _Color;
 
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    
                    o.pos = UnityObjectToClipPos( v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
					o.lightDir = WorldSpaceLightDir(v.vertex);
					o.viewDir = WorldSpaceViewDir(v.vertex);
					o.normal =  UnityObjectToWorldNormal(v.normal);
                    TRANSFER_VERTEX_TO_FRAGMENT(o);
                    return o;
                }
 
                float4 _LightColor0;
 
                float4 frag(v2f i) : COLOR
                {
                    i.lightDir = normalize(i.lightDir);
                    i.normal = normalize(i.normal);
                    
                    float atten = LIGHT_ATTENUATION(i);
 
                    float4 col = tex2D(_MainTex, i.uv);
                    
                    col *= _Color;
                                       
                    float lambert = dot(i.normal, i.lightDir) * 0.5 + 0.5;
                    float4 warpLighting = tex2Dlod(_Ramp, float4(lambert, 0.5, 0, 0));
                    
                    float fresnel = 1 - dot(i.normal, normalize(i.viewDir));
                    fresnel = pow(fresnel, 3);
                    fresnel  = saturate(fresnel + 0.1);
                
                    float3 halfDir = normalize((i.normal + i.lightDir) / 2);
                    float specular =  dot(halfDir, i.normal);
                    specular = pow(saturate(specular), 64) * atten * lambert * fresnel;
                    
                    float4 c;
                    c.rgb = (col.rgb * _LightColor0.rgb * warpLighting) * (atten) + specular;
                    c.a = col.a;
                    return c;
                }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack "VertexLit"
}
