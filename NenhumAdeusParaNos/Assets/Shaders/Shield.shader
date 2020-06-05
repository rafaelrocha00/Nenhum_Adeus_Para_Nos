Shader "Unlit/Shield"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _BreakAmount ("Break Amount (_BreakAmount)", float) = 0
    }
    SubShader
    {
        LOD 100
        Tags {"Queue"="Transparent+1"}
        
        Pass
        {
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _BreakAmount;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fresnel = max(0.05, pow(1 - saturate(dot(normalize(i.viewDir), normalize(i.normal))), 2));
                float col = step(saturate(_BreakAmount), tex2D(_MainTex, i.uv).r);
                
                return fresnel * col * _Color;
            }
            ENDCG
        }
    }
}
