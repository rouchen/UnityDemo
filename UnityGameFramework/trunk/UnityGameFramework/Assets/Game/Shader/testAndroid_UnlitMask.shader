Shader "Custom/testAndroid_UnlitMask" 
{
	Properties 
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_AMultiplier ("Layer Multiplier", Float) = 0.5
	}
	SubShader 
	{
		Tags { "Queue"="Transparent"}
		pass
		{
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off ZWrite Off
		
			CGPROGRAM
			
			#include "UnityCG.cginc"
				
			#pragma vertex vert
			#pragma fragment frag
			
			half4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			
			half _AMultiplier;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
				float2 tex1 : TEXCOORD1;
				half4 color : COLOR;
			};
			
			vertexOutput vert(vertexInput v)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				output.tex1 = TRANSFORM_TEX(v.texcoord1, _MaskTex);
				output.color = _Color * _AMultiplier;
				
				return output;
			}
			
			half4 frag(vertexOutput input) :COLOR
			{
				half4 c, m;
				c = tex2D(_MainTex, input.tex);
				m = tex2D(_MaskTex, input.tex1);
				
				c = c * m;
				
				c.rgb = c.rgb * input.color.rgb;
				c.a = c.a * _Color.a;
				
				return c;
			}
			
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
