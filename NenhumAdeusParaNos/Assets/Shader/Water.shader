Shader "Nature/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DepthAlpha ("Clarity", float) = 2
        _Fade ("Fade", float) = 1
        _From ("From", Vector) = (1,1,1,1)
        _To ("To", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10"}
        LOD 100

        GrabPass
        {
            "_BackgroundTexture"
        }

        Pass
        { 
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
                UNITY_FOG_COORDS(9)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 grabPos : TEXCOORD4;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _BackgroundTexture;
            float4 _MainTex_ST;
            float4 _From;
            float4 _To;
            half4 _Color;
            half _DepthAlpha;
            half _Fade;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv);
                
                //UNITY_APPLY_FOG(i.fogCoord, col);
                i.normal = normalize(i.normal);
                i.viewDir = normalize(i.viewDir);
                
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w);
                depth = Linear01Depth(depth) * _ProjectionParams.z - i.vertex.w;   
                depth *= 1 / _DepthAlpha;
                
                float4 backGround = tex2Dproj(_BackgroundTexture, i.grabPos);
                
                _From.rgb += _From.w;
                _To.rgb += _To.w;
                float3 colorEliminator = saturate(1 + ( (depth - _From.rgb) * (0 - 1) ) / (_To.rgb - _From.rgb));
                
                backGround.rgb *= pow(colorEliminator, _Fade);
                
                half3 worldRefl = reflect(-i.viewDir, i.normal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
                
                depth = saturate(depth + 0.5);
                float fresnel = saturate((1 - pow(dot(i.viewDir, i.normal), 0.2)) + 0.05);
                float4 finalColor = backGround;
                finalColor.rgb += skyColor * fresnel;
                finalColor.a = 1;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
