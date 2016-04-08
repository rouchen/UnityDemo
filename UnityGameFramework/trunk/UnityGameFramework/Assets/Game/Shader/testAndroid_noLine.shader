Shader "Custom/testAndroid_noLine" {
	Properties 
    {
		_Color ("MainColor", Color) = (1,1,1,1)
        _MainTex ("MainTexture (RGB)", 2D) = "white"{}
		
		_ShadowPower ("ShadowPower", Float) = 1
		_ShadowTex("ShadowTexture (RGB)", 2D) = "white"{}
		
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
			
			uniform float _ShadowPower;
            uniform sampler2D _ShadowTex;
			uniform float4 _ShadowTex_ST;
            
			uniform float4 _RimColor;
			uniform float _RimSize;
			uniform float _RimPower;			
			
            struct vertexOutput 
            {
                float4 pos      : SV_POSITION;
                float2 tex      : TEXCOORD0;
				float2 tex2		: TEXCOORD1;
				float3 viewDir	: TEXCOORD2;
				float3 normal	: TEXCOORD3;				
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
				float NdotL = max(dot(LightDir, Normal), 0);
				//Shadow
				float tmp = (NdotL + 1.0f) * 0.5f;
				
				output.tex2 = float2(tmp, 0.0f);
				
				output.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				output.normal = Normal;
				
				return output;
            }

            float4 frag ( vertexOutput input ) : COLOR
            {
                float4 lMainTex         = tex2D(_MainTex, input.tex);                 
                float4 shadowTex		= tex2D(_ShadowTex, input.tex2);
				
				float4 shadowEff = pow(shadowTex, _ShadowPower);
			
				//Rim Color
				float NdotV = saturate(dot(input.viewDir, input.normal));
				float NdotV_inv = 1.0f - NdotV;
				NdotV_inv = pow(NdotV_inv + _RimSize, _RimPower);
				float4 RimEff = float4(_RimColor.rgb * NdotV_inv, _RimColor.a);
				
				float4 lMainChanged     = _Color * lMainTex * shadowEff + RimEff;
				
                return lMainChanged;

			}

            ENDCG
		}		

	} 
	FallBack "Diffuse"
}
