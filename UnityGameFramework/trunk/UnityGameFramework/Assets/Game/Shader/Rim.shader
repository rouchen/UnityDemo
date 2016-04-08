Shader "Custom/Rim" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Power("Power", Float) = 4
		_Size("Size", Float) = 0
	}
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 200
		
		pass
		{
			Blend srcAlpha oneMinusSrcAlpha
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			uniform half4 _Color;
			uniform float _Power;
			uniform float _Size;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput o;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.normal = normalize(v.normal);
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				
				return o;
			}
			
			half4 frag(vertexOutput i):COLOR
			{
				half4 c = tex2D(_MainTex, i.uv) * _Color;
						
				float NDotV = saturate(dot(i.normal, i.viewDir));
				float NDotV_inv = (1.0f - NDotV) + _Size;				
				NDotV_inv = pow(NDotV_inv, _Power);
				
				c.a = c.a * NDotV_inv;
				
				return c;
			}
			
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
