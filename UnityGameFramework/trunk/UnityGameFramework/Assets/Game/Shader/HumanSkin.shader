Shader "Custom/HumanSkin" 
{
	Properties 
	{
		_DiffuseColor ("Surface Diffuse", Color) = (0.9, 1.0, 0.9, 1.0)
		_SpecularColor ("SpecularColor", Color) = (0.7,0.7,1,1)
		_SpecularExp ("Specular Exponent", Range(1, 128)) = 6
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "white" {}
		_RimTex ("Rim ramp (GRB) Fresnel ramp (A)", 2D) = " grey" {}
		_WrapTex ("Wrap ramp (RGBA)", 2D) = "grey" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		pass
		{
			Tags {"LightMode" = "ForwardBase"}
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _DiffuseColor;
			uniform float4 _SpecularColor;
			uniform float _SpecularExp;
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _NormalMap;
			uniform float4 _NormalMap_ST;
			uniform sampler2D _RimTex;
			uniform float4 _RimTex_ST;
			uniform sampler2D _WrapTex;
			uniform float4 _WrapTex_ST;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord :  TEXCOORD0;
				float4 tangent  :  TANGENT;
			};
			
			struct vertexOutput
			{
				float4 pos      : SV_POSITION;
                float2 uv      : TEXCOORD0;
				
				float3 LightVec	: TEXCOORD1;
				float3 LocalNormal	: TEXCOORD2;
				float3 LocalView : TEXCOORD3;
			};
			
			vertexOutput vert(vertexInput v)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.uv = v.texcoord.xy;
				
				output.LocalNormal = v.normal;
				
				TANGENT_SPACE_ROTATION;
				
				output.LightVec = mul(rotation, ObjSpaceLightDir(v.vertex));
				output.LocalView = mul(rotation, ObjSpaceViewDir(v.vertex));
				return output;
			}
			
			float4 frag(vertexOutput input) :COLOR
			{
				float3 Ln = normalize(input.LightVec);
				float3 Nn = normalize(UnpackNormal(tex2D(_NormalMap, input.uv)).xyz);
				float3 Vn = normalize(input.LocalView);
				float3 Hn = normalize(Vn + Ln);
				
				float rimUV = dot(Nn, Vn);
				float4 rimColor = tex2D(_RimTex, float2(rimUV, rimUV));
				
				float diffuseUV = dot(Nn, Ln) * 0.5f + 0.5f;
				float4 deffuseColor = tex2D(_WrapTex, float2(diffuseUV, diffuseUV));
				deffuseColor.rgb *= rimColor.rgb * 4;
				
				float NdotH = saturate(dot(Hn, Nn));
				float spec = pow(NdotH, _SpecularExp);
				
				float3 map = tex2D(_MainTex,input.uv).xyz;
				
				float3 result = (map * deffuseColor + _SpecularColor.rgb * spec * rimColor.a);
				
				
				return float4(result, 1.0f);
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
