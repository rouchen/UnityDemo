Shader "Custom/testAndroid_Unlit_Add" 
{
	Properties 
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}		
		_AMultiplier ("Layer Multiplier", Float) = 0.5
	}
	SubShader 
	{
		Tags { "Queue"="Transparent +1"}
		pass
		{
			Blend SrcAlpha one
			Cull Off Lighting Off ZWrite Off
		
			CGPROGRAM
			
			#include "UnityCG.cginc"
				
			#pragma vertex vert
			#pragma fragment frag
			
			half4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _AMultiplier;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};
			
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
				half4 color : COLOR;
			};
			
			vertexOutput vert(vertexInput v)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				output.color = _Color * _AMultiplier;
				
				return output;
			}
			
			half4 frag(vertexOutput input) :COLOR
			{
				half4 c;
				c = tex2D(_MainTex, input.tex);
								
				c.rgb = c.rgb * input.color.rgb;
				c.a = c.a * _Color.a;
				
				return c;
			}
			
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
