Shader "Hidden/PostEffectGroup" 
{
	Properties 
	{
		_MainTex ("", 2D) = "" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _CameraDepthTexture;
	
	uniform sampler2D _MainTex;
	uniform sampler2D _BloomBlurTex;
	
	float4 _MainTex_TexelSize;

	//!  ChromaticAberration.
	uniform float _EnableChromaticAberration;
	uniform float2 _ChromaticAberrationDirection;
	
	//!  Color Adjust.
	uniform float _EnableColorAdjust;
	uniform float _ColorAdjustBrightness;
	uniform float _ColorAdjustContrast;
	uniform float _ColorAdjustSaturation = 1.0f;
	uniform float _ColorAdjustGamma = 1.0f;
	
	//! Blur.
	uniform float _blurOffset = 1.0f;
	
	//! Bloom.
	uniform float _EnableBloom;
	uniform float _BloomImageWeight = 1.0f;
	uniform float _BloomBlurWeight = 0.65f;

	//! DOF.
	uniform float _EnableDOF;
	uniform	float _DOFBlurOffset;
	uniform int _DOFBlurIterations;
	uniform float _DOFFocusPlane;
	
	//! Motion Blur.
	uniform	float _EnableMotionBlur;
	uniform float _BlurValue;
	
	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
		float2 uv1  : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float2 uv3 : TEXCOORD3;
		float4 uv4 : TEXCOORD4;
	};
			
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		o.uv1= float2(0.0f, 0.0f);
		o.uv2= float2(0.0f, 0.0f);
		o.uv3= float2(0.0f, 0.0f);
		o.uv4= float4(0.0f, 0.0f, 0.0f, 0.0f);
		return o;
	}
	
	half4 brightness(half4 i)
	{
		half4 c = i - 1;
		half4 bright4= -6 * c * c + 2;
		half bright = dot( bright4, half4( 0.333333, 0.333333, 0.333333, 0) );
		c += bright + 0.6;
		c = max(c, half4(0.0f, 0.0f, 0.0f, 0.0f));
		return c;	
	}
	
	float4 frag (v2f pixelData) : COLOR
	{
		half4 finalColor = tex2D(_MainTex, pixelData.uv);
		
		
		//!  ChromaticAberration.
		if(_EnableChromaticAberration)
		{
			float offset = _ChromaticAberrationDirection * 0.01f;
			finalColor.r = tex2D(_MainTex, pixelData.uv + offset).r;
			finalColor.g = tex2D(_MainTex, pixelData.uv - offset).g;
		}
		
		//! ColorAdjust.
		if(_EnableColorAdjust)
		{
			//! brightness.
			finalColor += _ColorAdjustBrightness;
			
			//! Contrast.
			_ColorAdjustContrast = clamp(_ColorAdjustContrast, -1.0f, 1.0f);
			float factor = (1.015f * (_ColorAdjustContrast + 1.0f)) / (1.015f - _ColorAdjustContrast);
			finalColor.rgb = (finalColor.rgb - 0.5f) * factor + 0.5f;
			
			//!  Saturation.
			float p = sqrt((finalColor.r * finalColor.r * 0.299f) + (finalColor.g * finalColor.g * 0.578f) + (finalColor.b * finalColor.b * 0.114f));
			finalColor.rgb = p + (finalColor.rgb - p) * _ColorAdjustSaturation;
			
			//! Gama
			float GammaCorr = 1.0f / _ColorAdjustGamma;
			finalColor.rgb = pow(finalColor.rgb, GammaCorr);
			
		}
		
		//! Bloom.
		if(_EnableBloom)
		{
			half4 bightColor = brightness(finalColor);
			half4 bloomblurColor = tex2D(_BloomBlurTex, pixelData.uv);
			finalColor = bloomblurColor * _BloomBlurWeight + finalColor /*+ bightColor*/ * _BloomImageWeight;	
		}
		
		return finalColor;
	}	
	
	
	// Blur Pass.
	
	v2f blurvert(appdata_img v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		float2 uv = v.texcoord.xy;
		float2 _offset = _MainTex_TexelSize.xy * _blurOffset;
		
		o.uv = uv + float2(_offset.x, _offset.y);
		o.uv1 = uv + float2(_offset.x, -_offset.y);
		o.uv2 = uv + float2(-_offset.x, _offset.y);
		o.uv3 = uv + float2(-_offset.x, -_offset.y);
		o.uv4 = float4(0.0f, 0.0f, 0.0f, 0.0f);
		return o;
	}
	
	half4 blurfrag(v2f i) : COLOR
	{
		half4 c;
		c = tex2D(_MainTex, i.uv);
		c += tex2D(_MainTex, i.uv1);
		c += tex2D(_MainTex, i.uv2);
		c += tex2D(_MainTex, i.uv3);
		c /= 4;
		c.a = 1.0f;
		
		return c;
	}
	
	//! Brightness.
	half4 brightfrag(v2f i) : COLOR
	{
		half4 c;
		c = tex2D(_MainTex, i.uv);
		c = brightness(c);
		return c;
	}
	
	//! dof
	half4 doffrag(v2f pixelData) : COLOR
	{
		//! DOF.		
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, pixelData.uv));
		d = Linear01Depth(d);
		
		d = abs(d - _DOFFocusPlane);	
		
		half4 c = half4(0.0f, 0.0f, 0.0f, 1.0f);
		for(int i = 0; i < _DOFBlurIterations; i++)
		{
			float2 _offset = _MainTex_TexelSize.xy * _DOFBlurOffset * _DOFBlurIterations * i *d;
			half4 c2;
			c2 = tex2D(_MainTex, pixelData.uv + float2(_offset.x, _offset.y));
			c2 += tex2D(_MainTex, pixelData.uv + float2(_offset.x, -_offset.y));
			c2 += tex2D(_MainTex, pixelData.uv + float2(-_offset.x, _offset.y));
			c2 += tex2D(_MainTex, pixelData.uv + float2(-_offset.x, -_offset.y));
			
			c2 /= 4;
			c+=c2;
		}
		c /= _DOFBlurIterations;
		c.a = 1.0f;
		
		return c;
	}
	
	//! motionBlur.
	half4 motionBlurfrag(v2f pixelData) : COLOR
	{
		half4 c = half4(tex2D(_MainTex, pixelData.uv).rgb, _BlurValue);
		
		return c;
	}
	
	ENDCG
	SubShader 
	{
		//! pass0.
		Pass 
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma exclude_renderers flash
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}	
		
		//! pass1.
		Pass 
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex blurvert
			#pragma fragment blurfrag
			ENDCG
		}
		
		//! pass2.
		pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert
			#pragma fragment brightfrag
			ENDCG
		}
		
		//! pass3.
		pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert
			#pragma fragment doffrag
			ENDCG
		}
		
		//! pass4.
		pass
		{
			ZTest Always Cull Off ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert
			#pragma fragment motionBlurfrag
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
