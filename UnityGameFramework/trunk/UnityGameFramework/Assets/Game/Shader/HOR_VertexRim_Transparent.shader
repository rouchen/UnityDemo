Shader "HOR/VertexRim/Transparent" 
{
	Properties 
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower ("RimPower", Float) = 1.0
	}
	SubShader 
	{
		Tags { "Queue"="Transparent"  "IgnoreProjector"="True" "RenderType"="Transparent" }
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
			
			half4 _RimColor;
			float _RimPower;
						
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float4 rim : TEXCOORD1;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));	
				float NdotV = saturate(dot(viewDir, v.normal));
				float inv_NdotV = 1.0f - NdotV;
				
				output.rim = pow(_RimColor * inv_NdotV, _RimPower);
				
				return output;
			}
			
			half4 frag(vertexOutput input) : COLOR
			{
				half4 c;
				c = tex2D(_MainTex, input.uv);
				
				c = c * _Color + input.rim;
				
				return c;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
