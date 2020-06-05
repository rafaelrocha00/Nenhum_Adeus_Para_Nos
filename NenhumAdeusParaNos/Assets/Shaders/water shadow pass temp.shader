Shader "Unlit/water shadow pass temp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        LOD 100

        Pass
        {
            Tags {"RenderType"="Opaque" "LightMode"="ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                SHADOW_COORDS(2)
                float4 vertex : SV_POSITION;
                float4 viewPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _Noise;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                TRANSFER_SHADOW(o)
                o.viewPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUv = i.viewPos.xy / i.viewPos.w;
                screenUv *= _ScreenParams.xy / 128;
                
                // sample the texture
                float dither = tex2D(_Noise, screenUv) * 2 - 1;
                float shadow = SHADOW_ATTENUATION(i);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, shadow);
                
                clip(dither);
                
                return shadow;
            }
            ENDCG
        }
    }
}
