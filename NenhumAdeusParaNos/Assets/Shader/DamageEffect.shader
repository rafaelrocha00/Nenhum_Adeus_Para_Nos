Shader "Hidden/DamageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Mask;
            sampler2D _Mask2;
            float4 _Color;
            float _Density;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float4 ogCol = col;
                
                float sinTime = sin(_Time.w) * 0.5 + 0.5;
                sinTime *= cos(_Time.w) * 0.5 + 0.5;
                
                float mask = smoothstep(sinTime + 0.1, sinTime + 0.15, tex2D(_Mask, i.uv).r * 1.5+ 0.1);
                float mask2 = tex2D(_Mask2, i.uv).r;
                
                col = lerp(col, _Color, saturate(mask - (mask2 * (1 - _Color.a))));
                
                float mask3 = length(i.uv * 2 - 1);
                col *= saturate(smoothstep(0, 0.6, 1 - mask3) + 0.01);
                return lerp(ogCol, col, _Density);
            }
            ENDCG
        }
    }
}
