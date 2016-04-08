Shader "UI/TextureSlideH"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_SlideTex("Slide Texture", 2D) = "white"{}
		_MaskTex("Mask Texture", 2D) = "white"{}
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				half2 tex1		: TEXCOORD1;	
			};
			
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _SlideTex;
			float4 _SlideTex_ST;
			sampler2D _MaskTex; 
				
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.tex1 = TRANSFORM_TEX(IN.texcoord, _SlideTex);
				OUT.tex1.x -= (_Time.y - round(_Time.y)) * 2.0f;
				OUT.tex1.x *= 2.0f;
				
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 base = tex2D(_MainTex, IN.texcoord);
				half4 mask = tex2D(_MaskTex, IN.texcoord);
				half4 slide = tex2D(_SlideTex, IN.tex1);
				
				slide.rgb = slide.rgb * (slide.a * mask.a);
				slide.a = 0.0f;		
				half4 color = base * IN.color + slide;
				
				clip (color.a - 0.01);
				return color;
			}
		ENDCG
		}
	}
}
