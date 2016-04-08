Shader "HOR/BillBoard/Transparent" 
{
	Properties 
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			half4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
				float4 posTmp = mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) - float4(v.vertex.x, v.vertex.z, 0.0, 0.0);
				output.pos = mul(UNITY_MATRIX_P, posTmp);
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
}
