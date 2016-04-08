Shader "Left/Unlit-Mask"
{
	Properties
	{
		_Color ("Color Tint (RGBA)", Color) = (1, 1, 1, 0.5)
		_MainTex ("Base Texture (RGBA)", 2D) = "white" {}
		_MaskTex ("Mask Texture (RGBA)", 2D) = "white" {}
		_AMultiplier ("Layer Multiplier", Float) = 0.5
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZTest LEqual
		Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit vertex:vert alpha
		//#pragma only_renderers d3d9

		float4 _Color;
		sampler2D _MainTex;
		sampler2D _MaskTex;
		float _AMultiplier;

		inline half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo * atten, s.Alpha);
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_MaskTex;
			float4 color;
		};

		void vert (inout appdata_full v, out Input o)
		{
			o.uv_MainTex = v.texcoord;
			o.uv_MaskTex = v.texcoord1;
			o.color = _Color * _AMultiplier;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D (_MaskTex, IN.uv_MaskTex);
			c = c * c2;
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
