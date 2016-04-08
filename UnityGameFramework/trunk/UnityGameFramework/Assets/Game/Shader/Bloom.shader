Shader "Custom/Bloom" {
	Properties 
	{
		_MainTex ("", 2D) = "" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	uniform sampler2D _MainTex;
	uniform float4	_MainTex_ST;
	
	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
	};
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		return o;
	}
	
	float4 frag (v2f pixelData) : COLOR
	{
		float4 sum = float4(0.0f, 0.0f, 0.0f, 0.0f);
		for(int i = -3; i <= 3; i++)
		{
			float texoffset = 0.01f * i;
			float2 texStep = float2(texoffset, texoffset);
			float currentWeight = 0.1f; //exp(i * i * Negative2Sigma2) * Weight;
			sum += tex2D(_MainTex, pixelData.uv+texStep) * currentWeight;
		}
			
		return sum;
	}
	
	ENDCG
	
	SubShader {
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
	} 
	FallBack ""
}
