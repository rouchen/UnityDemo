Shader "HOR/AlphaMask" 
{
	Properties 
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "Queue"="Transparent"  "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend srcAlpha oneMinusSrcAlpha
		
		pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			half4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				output.uv1 = TRANSFORM_TEX(v.texcoord, _MaskTex);
				
				return output;
			}
			
			half4 frag(vertexOutput input) : COLOR
			{
				half4 c, m;
				c = tex2D(_MainTex, input.uv);
				m = tex2D(_MaskTex, input.uv1);
				c = c * _Color * m.a;
				
				return c;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
