Shader "Left/Unlit-Mask2Layer-Overlay-Culloff"
{
	Properties
	{
		_Color ("Color Tint (RGBA)", Color) = (1, 1, 1, 0.5)
		_MainTex ("Base Texture (RGBA)", 2D) = "white" {}
        _MaskTex ("Mask Texture (RGBA)", 2D) = "white" {}
        _Main2Tex ("Base Texture 2 (RGBA)", 2D) = "black" {}
        _Mask2Tex ("Mask Texture 2 (RGBA)", 2D) = "white" {}
		_AMultiplier ("Layer Multiplier", Float) = 1.0
	}

	SubShader
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off 
		ZWrite Off 
		ZTest Always
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Fog { Color (0,0,0,0) }
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit vertex:vert alpha
		#pragma only_renderers d3d9

		float4 _Color;
		sampler2D _MainTex;
        sampler2D _MaskTex;
        sampler2D _Main2Tex;
        sampler2D _Mask2Tex;
		float _AMultiplier;

		inline half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo * atten, s.Alpha);
		}

		struct Input
		{
			float2 uv_MainTex;
            float2 uv_MaskTex;
            float2 uv_Main2Tex;
            float2 uv_Mask2Tex;
			float4 color;
		};

		void vert (inout appdata_full v, out Input o)
		{
			o.color = _Color * _AMultiplier;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c  = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 m1 = tex2D (_MaskTex, IN.uv_MaskTex);
            fixed4 c2 = tex2D (_Main2Tex, IN.uv_Main2Tex);
			fixed4 m2 = tex2D (_Mask2Tex, IN.uv_Mask2Tex);
			c = c * m1;
			c2 = c2 * m2;
			c = c + c2;

			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
