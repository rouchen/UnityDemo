Shader "Custom/GoochyGloss" {
	Properties 
	{
		_BrightColor ("Bright Surface Color", Color) = (0.8,0.5,0.1,1)
		_DarkColor ("Dark Surface Color", Color) = (0.0,0.0,0.0,1)
		_WarmTone ("Gooch warm tone", Color) = (0.5,0.4,0.005,1)
		_CoolTone ("Gooch cool tone", Color) = (0.05,0.05,0.6,1)
	
		_SpecularColor ("SpecularColor", Color) = (0.7,0.7,1,1)
		_SpecularSize ("Specular", Range(0.0, 1.0)) = 0.05
		_SpecularExp ("Specular Exponent", Range(1, 128)) = 6
		_GlossTop ("Bright Glossy Edge", Range(0.2, 1.0)) = 0.46
		_GlossBot ("Dim Glossy Edge", Range(0.05, 0.95)) = 0.41
		_GlossDrop ("Glossy Brightness Drop", Range(0.0, 1.0)) = 0.25
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "white" {}
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
			
			uniform float4 _BrightColor;
			uniform float4 _DarkColor;
			uniform float4 _WarmTone;
			uniform float4 _CoolTone;
			
			uniform float4 _SpecularColor;
			uniform float _SpecularSize;
			uniform float _SpecularExp;
			uniform float _GlossTop;
			uniform float _GlossBot;
			uniform float _GlossDrop;
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			uniform sampler2D _NormalMap;
			uniform float4 _NormalMap_ST;
			
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
			
			float glossy_drop(float v, float top, float bot, float drop)
			{
				return (drop+smoothstep(bot,top,v)*(1.0-drop));
			}
			
			float4 frag(vertexOutput input) :COLOR
			{
				float3 Ln = normalize(input.LightVec);
				//float3 Nn = normalize(input.LocalNormal);
				float3 Nn = normalize(UnpackNormal(tex2D(_NormalMap, input.uv)).xyz);
				float3 Vn = normalize(input.LocalView);
				float3 Hn = normalize(Vn + Ln);
				
				float hdn = pow(max(0, dot(Hn, Nn)), _SpecularExp);
				hdn *= glossy_drop(hdn, _GlossTop, _GlossBot, _GlossDrop);
				float4 specContrib = hdn * _SpecularColor;
				
				float ldn = dot(Ln, Nn);
				float mixer = 0.5 * (ldn + 1.0);
				float3 surfColor = lerp(_DarkColor, _BrightColor, mixer);
				float3 toneColor = lerp(_CoolTone, _WarmTone, mixer);
				float4 diffContrib = float4((surfColor + toneColor), 1);
				
				float3 map = tex2D(_MainTex,input.uv).xyz;
				float3 result = map * diffContrib + specContrib;
				
				return float4(result, 1.0f);			
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
