Shader "HOR/VertexRimReflect/Transparent" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}		
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Float) = 1
		_CubeTex ("Cube", Cube) = "" {}
		_ReflectPower ("Reflect Power", Float) = 1
		_UVRotSpeed("UV Rotate Speed", Float) = 0
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
			samplerCUBE _CubeTex;
			float _ReflectPower;
			float _UVRotSpeed;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float4 rim : TEXCOORD1;
				float3 reflectDir : TEXCOORD2;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				//output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				// UV Rotation.
				float rotTime = _Time.y * _UVRotSpeed;
				float s = sin(rotTime);
				float c = cos(rotTime);
				
				float2x2 rotMatrix = float2x2(c, -s, s, c);
				rotMatrix = rotMatrix * 0.5f;
				rotMatrix = rotMatrix + 0.5f;
				rotMatrix = rotMatrix *2.0f -1.0f;
				float2 tex = v.texcoord.xy - 0.5f;
				output.uv = mul(rotMatrix, tex);
				output.uv = output.uv + 0.5f;
			
				
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float NdotV = saturate(dot(viewDir, v.normal));
				float inv_NdotV = 1.0f - NdotV;
				
				output.rim = pow(_RimColor * inv_NdotV, _RimPower);
				output.reflectDir = normalize(reflect(-viewDir, v.normal));
				
				return output;
			}
			
			half4 frag(vertexOutput input) : COLOR
			{
				half4 c, r;
				c = tex2D(_MainTex, input.uv);
				r = texCUBE(_CubeTex, input.reflectDir) * _ReflectPower;
				c = c * _Color + r + input.rim;
				
				return c;
			}
			
			ENDCG	
		}
	} 
	FallBack "Diffuse"
}
