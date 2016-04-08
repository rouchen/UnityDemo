Shader "Custom/testAndroid_Overlay2UV" {
	Properties 
    {
		_Color ("MainColor", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTexture (RGB)", 2D) = "white"{}
		_LightMapTex("ShadowTexture (RGB)", 2D) = "white"{}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque"}

        Pass 
        {      
			Tags {"LightMode" = "ForwardBase"}
			
            CGPROGRAM
                      
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase
			//#pragma fragmentoption ARB_precision_hint_fastest
			
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
			
            uniform sampler2D _LightMapTex;
			uniform float4 _LightMapTex_ST;
			   
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord :  TEXCOORD0;
				float4 texcoord1 :  TEXCOORD1;
			};
			
			
            struct vertexOutput 
            {
                float4 pos      : SV_POSITION;
                float4 tex      : TEXCOORD0;
				LIGHTING_COORDS(1,2)
            };

            vertexOutput vert ( vertexInput v )
            {
                vertexOutput output;
                // position.
				output.pos      = mul(UNITY_MATRIX_MVP, v.vertex);
               	output.tex.xy   = v.texcoord.xy;
				output.tex.zw	= v.texcoord1.xy;
				
				TRANSFER_VERTEX_TO_FRAGMENT(output);
				
				return output;
            }
			
			float SoftLightBlend(float mainColor, float maskColor)
			{
				float finalColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				if(maskColor <= 0.5f)
				{
					finalColor = 2.0f * mainColor * maskColor + pow(mainColor, 2) * (1.0f - 2.0f * maskColor);
				}
				else
				{
					finalColor = 2.0f * mainColor * (1.0f - maskColor) + pow(mainColor, 0.5f) * (2.0f * maskColor - 1.0f);
				}
				
				return finalColor;
			}
			
			float OverlayBlend(float mainColor, float maskColor)
			{
				float finalColor;
				if(mainColor <= 0.5f)
				{
					finalColor = 2.0f * mainColor * maskColor;
				}
				else
				{
					finalColor = 1.0f - 2.0f * (1.0f - mainColor) * (1.0f - maskColor);
				}
				
				return finalColor;
			}
			
            float4 frag ( vertexOutput input ) : COLOR
            {
                float4 lMainTex         = tex2D(_MainTex, input.tex.xy);                 
                float4 lightMapTex		= tex2D(_LightMapTex, input.tex.zw);
				
				float4 MainColor 		= float4(1.0f, 1.0f, 1.0f, 1.0f);
				MainColor.r 			= OverlayBlend(lMainTex.r, lightMapTex.r);
				MainColor.g 			= OverlayBlend(lMainTex.g, lightMapTex.g);
				MainColor.b 			= OverlayBlend(lMainTex.b, lightMapTex.b);
				
				float  atten 			= LIGHT_ATTENUATION(input);
							
				float4 FinalColor     	= _Color * MainColor * atten;
                return FinalColor;

			}
			
            ENDCG
		}

	} 
	FallBack "VertexLit"
}

