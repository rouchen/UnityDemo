Shader "Left/WaterPlane"
{
	Properties
	{
		_Color ("Color Tint (RGB)", Color) = (1, 1, 1, 1)
		_WrapAmount ("Wrapped Lambert Amount", Range(0.0, 1.0)) = 0
		_MainTex ("Base Texture (RGBA)", 2D) = "white" {}
		_DetailTex ("2nd layer (RGBA)", 2D) = "white" {}
		_ReflectionTex ("Internal Reflection (RGB)", 2D) = "gray" { TexGen ObjectLinear }
		_BumpMap ("Normal Map (RGB)", 2D) = "bump" {}
		_GlossColor ("Base Specular Color Tint (RGB)", Color) = (0.5, 0.5, 0.5, 1.0)
		_Shininess ("Specular Shininess", Range (0, 1)) = 1
		_ScrollX ("Base layer Scroll speed X", Float) = 1.0
		_ScrollY ("Base layer Scroll speed Y", Float) = 0.0
		_Scroll2X ("2nd layer Scroll speed X", Float) = 1.0
		_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
		_AMultiplier ("Layer Multiplier", Float) = 0.5
		_WaveAmp ("Wave Amp", Float) = 2
		_WaveAmp2 ("Wave Amp2", Float) = 4
		_WaveFreq ("Wave Freq", Float) = 20
		_WaveFreq2 ("Wave Freq2", Float) = 20
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf WrapBlinnPhong vertex:vert
		#pragma only_renderers d3d9
		#pragma target 3.0

		float4 _Color;
		float _WrapAmount;
		sampler2D _MainTex;
		sampler2D _DetailTex;
		sampler2D _ReflectionTex;
		sampler2D _BumpMap;
		float4 _GlossColor;
		float _Shininess;
		float _ScrollX;
		float _ScrollY;
		float _Scroll2X;
		float _Scroll2Y;
		float _AMultiplier;
		float _WaveAmp;
		float _WaveAmp2;
		float _WaveFreq;
		float _WaveFreq2;

		struct SurfaceOutputSpecColor
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half3 Gloss;
			half Specular;
			half Alpha;
		};

		// forward lighting
		half4 LightingWrapBlinnPhong (SurfaceOutputSpecColor s, half3 lightDir, half3 viewDir, half atten)
		{
			half diff = max (0, dot (s.Normal, lightDir) * (1.0 - _WrapAmount) + _WrapAmount);
			half3 h = normalize (lightDir + viewDir);
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, _Shininess * 128);

			half4 c;
			c.rgb = _LightColor0.rgb * (s.Albedo * diff + s.Gloss * spec) * (atten * 2);
			c.a = s.Alpha;
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_DetailTex;
			float2 uv_ReflectionTex;
			float2 uv_BumpMap;
			float4 screenPos;

			float4 scroll;
		};

		void vert (inout appdata_full v, out Input o)
		{
			v.vertex.y += sin(_Time * v.texcoord.x * _WaveFreq).y * _WaveAmp;
			v.vertex.y += sin(_Time * v.texcoord.y * _WaveFreq2).y * _WaveAmp2;

			o.scroll = frac(_Time * float4(_ScrollX, _ScrollY, _Scroll2X, _Scroll2Y));
		}

		void surf (Input IN, inout SurfaceOutputSpecColor o)
		{
			float2 uv1 = IN.uv_MainTex + IN.scroll.xy;
			float2 uv2 = IN.uv_DetailTex + IN.scroll.zw;
			float2 uv1b = IN.uv_BumpMap + IN.scroll.xy;
			float2 uv2b = IN.uv_BumpMap + IN.scroll.zw;
			fixed4 c1 = tex2D (_MainTex, uv1);
			fixed4 c2 = tex2D (_DetailTex, uv2);

			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			//screenUV.xy += nor * 0.2;
			//screenUV *= float2(8,6);
			//fixed4 refl = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(IN.screenPos) );

			o.Albedo = (c1.rgb * c2.rgb) * _Color.rgb * _AMultiplier;
			o.Albedo *= tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(IN.screenPos) ) * 2;
			o.Normal = normalize( UnpackNormal( frac(tex2D(_BumpMap, uv1b) + tex2D(_BumpMap, uv2b)) ) );
			o.Gloss = _GlossColor.rgb;
			o.Specular = _GlossColor.a;
			//o.Emission = refl.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
