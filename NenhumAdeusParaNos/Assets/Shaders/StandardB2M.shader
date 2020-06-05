Shader "Custom/StandardB2M"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
       
        [NoScaleOffset]_BumpMap ("Normal", 2D) = "bump" {}
        [NoScaleOffset]_AmbientOcclusion ("Ambient Occlusion", 2D) = "white" {}
        [NoScaleOffset]_GlossinessTex ("Glossiness texture", 2D) = "black" {}
        [NoScaleOffset]_MetallicTex ("Metallic Textura", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Glossiness", Range(0,1)) = 0.0
        _NormalStr ("Normal Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MetallicTex;
        sampler2D _GlossinessTex;
        sampler2D _AmbientOcclusion;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Metallic;
        half _Glossiness;
        half _NormalStr;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;  
            fixed m = tex2D(_MetallicTex, IN.uv_MainTex);        
            fixed g = tex2D(_GlossinessTex, IN.uv_MainTex);         
            fixed ao = tex2D(_AmbientOcclusion, IN.uv_MainTex);
            fixed3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Normal = lerp(float3(0, 0, 1), n, _NormalStr);
            o.Occlusion = ao;
            o.Metallic = _Metallic * m;
            o.Smoothness = saturate(_Glossiness + g);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
