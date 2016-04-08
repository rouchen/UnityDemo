Shader "Custom/ToonDeferred" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Bump map", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf DeferredLight
		#pragma debug
		fixed4 LightingDeferredLight(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
		{
			half3 h = normalize (lightDir + viewDir);
			
			fixed diff = max (0, dot (s.Normal, lightDir));
			
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0) * s.Gloss;
			
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
			c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
			return c;
		}
		
		fixed4 LightingDeferredLight_PrePass (SurfaceOutput s, half4 light)
		{
			fixed spec = light.a * s.Gloss;
	
			fixed4 c;
			c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
			c.a = s.Alpha + spec * _SpecColor.a;
			return c;
		}

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));			
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
			o.Specular = _Shininess;
			o.Gloss = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
