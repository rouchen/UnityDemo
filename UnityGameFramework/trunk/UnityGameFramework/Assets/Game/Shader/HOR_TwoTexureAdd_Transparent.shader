Shader "HOR/TwoTexureAdd/Transparent" 
{
	Properties 
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SubTex ("Sub (RGB)", 2D) = "white" {}
		_FadeSpeed("Fade Speed", Float) = 0.0
	}
	SubShader 
	{
		Tags { "Queue"="Geometry"  "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			half4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SubTex;
			float4 _SubTex_ST;
			
			float _FadeSpeed;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float fade : TEXCOORD2;
			};
			
			vertexOutput vert(vertexInput v)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				output.uv1 = TRANSFORM_TEX(v.texcoord1, _SubTex);
				
				output.fade = (cos(_Time.y * _FadeSpeed) + 1.0f) * 0.5f;
				
				return output;
			}
			
			half4 frag(vertexOutput input) : COLOR
			{
				half4 c,s;
				c = tex2D(_MainTex, input.uv);
				s = tex2D(_SubTex, input.uv1);
				c += s;
				c = c * _Color * input.fade;
								
				return c;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
