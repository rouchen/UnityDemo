Shader "Custom/GlossyWetHiLightReflect" {
	Properties 
	{
		_MainColor ("MainColor", Color) = (1,1,1,1)
		_DiffusePower("DiffusePower", Range(0.0, 1.0)) = 0.2
		_SpecularColor ("SpecularColor", Color) = (0.7,0.7,1,1)
		_SpecularSize ("Specular", Range(0.0, 1.0)) = 0.05
		_SpecularExp ("Specular Exponent", Range(1, 128)) = 6
		_GlossTop ("Bright Glossy Edge", Range(0.2, 1.0)) = 0.46
		_GlossBot ("Dim Glossy Edge", Range(0.05, 0.95)) = 0.41
		_GlossDrop ("Glossy Brightness Drop", Range(0.0, 1.0)) = 0.25
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "white" {}
		_CubeMap ("Cube Map", Cube) = "" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		pass
		{
			Tags {"LightMode" = "ForwardBase"}
			
			CGPROGRAM
			
			#pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			
			uniform float4 _MainColor;
			uniform float _DiffusePower;
			
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
			uniform samplerCUBE _CubeMap;
			uniform float4 _CubeMap_ST;
			
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
				float3 localRealView : TEXCOORD4;
			};
			
			vertexOutput vert(vertexInput v)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.uv = v.texcoord.xy;
				
				output.LocalNormal = v.normal;
				output.localRealView = ObjSpaceViewDir(v.vertex) * -1.0f;
				
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
				
				float4 litV = lit(dot(Ln,Nn),dot(Hn,Nn),_SpecularExp);
				float spec = litV.y * litV.z;
				float GlossTop = max(_GlossTop,_GlossBot);
				float GlossBot = min(_GlossTop,_GlossBot);
				spec *= (_SpecularSize * glossy_drop(spec,GlossTop,GlossBot,_GlossDrop));
				
				float3 diffContrib =  max(litV.y, _DiffusePower) * _MainColor.rgb;
				float3 specContrib = spec * _SpecularColor.rgb;
				
				float3 reflectedDir = reflect(input.localRealView, normalize(input.LocalNormal));
				float3 reflectColor = texCUBE(_CubeMap, reflectedDir).rgb;
				
				float3 map = tex2D(_MainTex,input.uv).xyz;
				float3 result = specContrib + (map * diffContrib) + litV.y * reflectColor * 0.5f;
				
				return float4(result, 1.0f);	
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
