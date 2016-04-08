Shader "Toon/SpecularNormal" {
	Properties 
    {
		_Color ("MainColor", Color) = (1,1,1,1)
        
		_ShadowPower ("ShadowPower", Float) = 1
		
		_SpecularColor ("SpecularColor", Color) = (0.7,0.7,1,1)
		_SpecularExp ("Specular Exponent", Range(1, 128)) = 6
		
		_OutLineColor("OutLine Color", Color) = (0, 0, 0, 0)
		_OutLineSize("OutLine Size", Float) = 0.01
		
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimSize ("Rim Size", Float) = 0
		_RimPower ("Rim Power", Float) = 0
		
		_MainTex ("MainTexture (RGB)", 2D) = "white"{}
		_NormalMap ("Normal Map", 2D) = "white" {}
		_ShadowTex("ShadowTexture (RGB)", 2D) = "white"{}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }

        Pass 
        {
			Tags {"LightMode" = "ForwardBase"}	
			ZTest LEqual
			cull Back
			
			CGPROGRAM
          
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fwdbase
            
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			uniform float4 _LightColor0;
			
			uniform float4 _Color;
            uniform float _ShadowPower;
			
			uniform float4 _SpecularColor;
			uniform float _SpecularExp;
			
			uniform float4 _RimColor;
			uniform float _RimSize;
			uniform float _RimPower;
			
			uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
			
			uniform sampler2D _NormalMap;
			uniform float4 _NormalMap_ST;
			
			uniform sampler2D _ShadowTex;
			uniform float4 _ShadowTex_ST;
			
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
                float2 tex      : TEXCOORD0;
				float3 viewDir	: TEXCOORD1;
				float3 lightDir	: TEXCOORD2;
				LIGHTING_COORDS(3,4)
            };

            vertexOutput vert ( vertexInput v )
            {
                vertexOutput output;
                // position.
				output.pos      = mul(UNITY_MATRIX_MVP, v.vertex);
               	output.tex      = v.texcoord.xy;
				
				TANGENT_SPACE_ROTATION;
				
				output.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
				output.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
				
				TRANSFER_VERTEX_TO_FRAGMENT(output);
				
				return output;
            }

            float4 frag ( vertexOutput input ) : COLOR
            {
                float4 lMainTex         = tex2D(_MainTex, input.tex);                 
                
				float3 Ln = normalize(input.lightDir);
				float3 Nn = normalize(UnpackNormal(tex2D(_NormalMap, input.tex)).xyz);
				float3 Vn = normalize(input.viewDir);
				float3 Hn = normalize(Vn + Ln);
				
				float NdotL = max(dot(Ln, Nn), 0);
				//Shadow
				float tmp = (NdotL + 1.0f) * 0.5f;
				float4 shadowTex = tex2D(_ShadowTex, float2(tmp, 0.0f));
				float4 shadowEff = pow(shadowTex, _ShadowPower);
				
				// specular.
				//float4 litV = lit(dot(Ln,Nn),dot(Hn,Nn),_SpecularExp);
				//float spec = litV.y * litV.z;				
				//float4 specContrib = spec * _SpecularColor;
				
				//Rim Color
				float NdotV = saturate(dot(Vn, Nn));
				float NdotV_inv = 1.0f - NdotV;
				NdotV_inv = pow(NdotV_inv + _RimSize, _RimPower);
				float4 RimEff = float4(_RimColor.rgb * NdotV_inv, _RimColor.a);
				
				float  atten 			= LIGHT_ATTENUATION(input);
				
				float4 lMainChanged     = _Color * lMainTex * _LightColor0*2.0f * max(atten, 0.6f) * shadowEff + RimEff; // + specContrib;
				
				//specular
				float4 litV = lit(dot(Ln,Nn),dot(Hn,Nn),_SpecularExp);
				if(litV.z > 0.5f)
				{
					lMainChanged.rgb = _SpecularColor.a * _SpecularColor.rgb + (1.0f - _SpecularColor.a) * lMainChanged.rgb;
				}
				
			
				return lMainChanged;

			}

            ENDCG
		}
		
				
		pass
		{
			Tags {"LightMode" = "ForwardBase"}	
			cull Front
			ZTest Less
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _OutLineColor;
			uniform float _OutLineSize;
			
			struct vertexOutput
			{
				float4 pos : POSITION;
			};
			
			vertexOutput vert(appdata_base v)
			{
				vertexOutput output;
				
				float4 projPos = mul(UNITY_MATRIX_MVP, v.vertex);
				float4 projNormal = mul(UNITY_MATRIX_MVP, float4(v.normal, 0));
				
				half4 scaledNormal = _OutLineSize * 0.00285f * projNormal; // * projSpacePos.w;
				scaledNormal.z += 0.00001;
				
				output.pos = projPos + scaledNormal;
								
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				float4 FinalColor = _OutLineColor;
				
				return FinalColor;
			}
			
			ENDCG
		}
		

	} 
	FallBack "Diffuse"
}
