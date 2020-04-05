﻿Shader "Hidden/EffectBattleStart"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Range ("Range", float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _Pivot;
            float _Range;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                float waves = sin(length(i.wPos - _Pivot) - _Time.w * 2);
                waves = pow(abs(waves), 16);
                
                float range = 1 - saturate(length(i.wPos - _Pivot) * (1 / _Range));
                range = range * range;
                
                return float4(float3(1, 0.05, 0.05) * range * waves, 1);
            }
            ENDCG
        }
    }
}