﻿Shader "Custom/Sand"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
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
            };
            
            sampler2D _MainTex;
            sampler2D _Noise;
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
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed3 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed shadow = SHADOW_ATTENUATION(i);
                
                i.viewDir = normalize(i.viewDir);
                i.worldNormal = normalize(i.worldNormal);

                float2 screenUv = i.screenPosition.xy / i.screenPosition.w;
                
                float diff = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                
                float2 screenUvScale = _ScreenParams.xy / 128;
                float3 noise = normalize(tex2D(_Noise, screenUv * screenUvScale) * 2 - 1);
                noise = reflect(_WorldSpaceLightPos0.xyz, noise);
                float glitter = max(0, dot(noise, i.viewDir));
                if(glitter > 0.9)
                    glitter = 0;
                
                glitter = 1 - glitter;
                
                float3 halfDir = normalize(i.worldNormal + i.viewDir);
                float specular = max(0, dot(halfDir, i.worldNormal));
                specular = pow(specular, 64) * shadow * diff * glitter * 3;
                
                fixed3 lighting = diff * shadow + i.ambient;
                col.rgb *= lighting;
                return col + specular;
            }
            ENDCG
        }
        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}