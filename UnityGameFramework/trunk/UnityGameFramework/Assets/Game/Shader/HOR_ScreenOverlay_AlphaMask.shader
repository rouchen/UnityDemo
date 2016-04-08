Shader "HOR/ScreenOverlay/AlphaMask" 
{
	Properties 
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_scaleX ("ScaleX", Float) = -0.1
		_scaleY ("ScaleY", Float) = 0.3
	}
	SubShader 
	{
		Tags { "Queue"="Overlay"  "IgnoreProjector"="True" "RenderType"="Transparent" }
		pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite OFF
			ZTest OFF
			cull Off
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			uniform half4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _MaskTex;
			uniform float4 _MaskTex_ST;
			uniform float _scaleX;
			uniform float _scaleY;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
				output.pos = float4(v.vertex.x * _scaleX, v.vertex.y * _scaleY, 0.0f, 1.0f);
				output.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				output.uv2 = TRANSFORM_TEX(v.texcoord.xy, _MaskTex);
				return output;
			}
			
			half4 frag(vertexOutput input) : COLOR
			{
				half4 c, m;
				c = tex2D(_MainTex, input.uv);
				m = tex2D(_MaskTex, input.uv2);
				c = c * _Color * m.a;
				
				return c;
			}
			
			ENDCG	
		}
	}
	FallBack "Diffuse"
}
