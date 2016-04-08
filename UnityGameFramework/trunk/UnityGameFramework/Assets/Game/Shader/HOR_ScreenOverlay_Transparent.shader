Shader "HOR/ScreenOverlay/Transparent" 
{
	Properties 
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Quad ("Quad", Vector) = (0,0,128,128)
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
			//uniform float4 _Quad;
			uniform float _scaleX;
			uniform float _scaleY;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
			/*	
				float2 rasterPosition = float2( _Quad.x + _ScreenParams.x / 2.0 + _Quad.z * (v.vertex.x + 0.5),
												_Quad.y + _ScreenParams.y / 2.0 + _Quad.w * (v.vertex.y + 0.5));
				output.pos = float4( 2.0 * rasterPosition.x / _ScreenParams.x - 1.0,
									 2.0 * rasterPosition.y / _ScreenParams.y - 1.0,
									 0.0, 
									 1.0);
				output.uv = float2( v.vertex.x + 0.5, 
							v.vertex.y + 0.5);
			*/
				output.pos = float4(v.vertex.x * _scaleX, v.vertex.y * _scaleY, 0.0f, 1.0f);
				output.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);	
				return output;
			}
			
			half4 frag(vertexOutput input) : COLOR
			{
				half4 c;
				c = tex2D(_MainTex, input.uv);
				c *= _Color;
				return c;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
