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
        
        //_Noise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10"}
        LOD 100

        GrabPass
        {
            "_BackgroundTexture"
        }

        // color
        Pass
        { 
            //Tags { "RenderType"="Transparent" "Queue"="Transparent+10"}
            ZWrite Off
            CGPROGRAM
            //#pragma vertex vert
            #pragma fragment frag
            
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            
            // make fog work
            #pragma multi_compile_fog
            
            #pragma target 4.6

            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #include "Lighting.cginc"

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
                float3 wPos : TEXCOORD5;
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
                float3 wPos = mul(unity_ObjectToWorld, v.vertex);
                //v.vertex.y += sin(wPos.x + _Time.y) * 0.1;
            
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                
                TessVertex tessvert (appdata v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    //o.tangent = v.tangent;
                    o.texcoord0 = v.uv;
                    //o.texcoord1 = v.texcoord1;
                    //o.texcoord2 = v.texcoord2;
                    return o;
                }
                
                void displacement (inout appdata v){
                    float3 wPos = mul(unity_ObjectToWorld, v.vertex);
                    v.vertex.xyz = wPos;
                    
                    float k = 2 * UNITY_PI;          
                    float heightSin = 0;
                    float heightCos = 0;
                    
                    heightSin += sin(k * wPos.x * 0.1 - _Time.y * 0.314) * 0.1;
                    heightSin += sin(k * wPos.z * 0.2 - _Time.y * 0.543) * 0.1;           
                    heightSin += sin(k * (wPos.z + wPos.x) * 0.02 - _Time.y) * 0.5;// * sin(k * wPos.z * 0.5 - _Time.y);
                    
                    heightCos += cos(k * wPos.x * 0.1 - _Time.y * 0.314) * 0.1;
                    heightCos += cos(k * wPos.z * 0.2 - _Time.y * 0.543) * 0.1;           
                    heightCos += cos(k * (wPos.z + wPos.x) * 0.02 - _Time.y) * 0.5;// * cos(k * wPos.z * 0.5 - _Time.y);
                    
                    float3 tangent = normalize(float3(1, k * heightCos, 0));
                    v.normal = float3(-tangent.y, tangent.x, 0);
                    v.normal = lerp(v.normal, float3(0, 1, 0), 0.9);
                    
                    v.vertex.y += heightSin;
                    v.vertex.y += sin(_Time.y) * 0.5;
                    v.vertex = mul(unity_WorldToObject, v.vertex);
                    
                }
                
                float Tessellation(TessVertex v){
                    return 21;
                }
                
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                
                [domain("tri")]
                v2f domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    appdata v = (appdata)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    //v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    //v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    //v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    displacement(v);
                    v2f o = vert(v);
                    return o;
                }
            #endif

            float4 frag (v2f i) : SV_Target
            {
                fixed caustics1 = tex2D(_MainTex, i.wPos.xz * 0.1 - _Time.x);
                fixed caustics2 = tex2D(_MainTex, i.wPos.xz * 0.05 + float2(1.232, 0.31415) * _Time.x);
                
                fixed finalCaustics = min(caustics1, caustics2);
                finalCaustics = pow(finalCaustics, 10) * 50;
                
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
                
                finalColor = lerp(saturate(lerp(1, caustics1, depth)), finalColor, depth) * _LightColor0;
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                //return float4(i.normal, 1);
                return finalColor + finalCaustics;
            }
            ENDCG
        }
    }
}
