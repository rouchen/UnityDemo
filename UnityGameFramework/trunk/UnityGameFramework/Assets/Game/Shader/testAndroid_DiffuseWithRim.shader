Shader "Custom/testAndroid_DiffuseWithRim" {
	Properties 
    {
		_Color ("MainColor", Color) = (1,1,1,1)
        _MainTex ("MainTexture (RGB)", 2D) = "white"{}
		
		_Shadow("Shadow", Float) = 0.5	
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimSize ("Rim Size", Float) = 0
		_RimPower ("Rim Power", Float) = 0
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }

        Pass 
        {      

            CGPROGRAM
            //#pragma glsl
			//#pragma only_renderers opengl 
          
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            //#include "UnityCG.glslinc"
			
			uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;	
			
            uniform float _Shadow;
			uniform float4 _RimColor;
			uniform float _RimSize;
			uniform float _RimPower;			
			
            struct vertexOutput 
            {
                float4 pos      : SV_POSITION;
                float2 tex      : TEXCOORD0;
				float4 color	: TEXCOORD1;
				float4 rim		: TEXCOORD2;
            };

            vertexOutput vert ( appdata_base v )
            {
                vertexOutput output;
                // position.
				output.pos      = mul(UNITY_MATRIX_MVP, v.vertex);
               	output.tex      = v.texcoord;
				// Light
				float3 LightDir = normalize(ObjSpaceLightDir(v.vertex));
				float3 Normal = normalize(v.normal);				
				float NdotL = max(dot(LightDir, Normal), _Shadow);
				
				output.color = _Color * NdotL;
				
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
					
				float NdotV = saturate(dot(viewDir, Normal));
				float NdotV_inv = 1.0f - NdotV;
				NdotV_inv = pow(NdotV_inv + _RimSize, _RimPower);
				output.rim = float4(_RimColor.rgb * NdotV_inv, _RimColor.a);
				
				return output;
            }

            float4 frag ( vertexOutput input ) : COLOR
            {
                float4 lMainTex         = tex2D(_MainTex, input.tex);
				float4 lMainChanged     = input.color * lMainTex  + input.rim;
                return lMainChanged;

			}

            ENDCG
		}		

	} 
	FallBack "Diffuse"
}
