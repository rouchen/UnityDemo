Shader "Custom/UnityToon" 
{
   Properties 
   {
      _Color ("Diffuse Color", Color) = (1,1,1,1) 
      _UnlitColor ("Unlit Diffuse Color", Color) = (0.5,0.5,0.5,1) 
      _DiffuseThreshold ("Threshold for Diffuse Colors", Range(0,1)) 
         = 0.1 
      _OutlineColor ("Outline Color", Color) = (0,0,0,1)
      _LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1
      _UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) 
         = 0.4
      _SpecColor ("Specular Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
	  
	  _MainTex ("MainTexture (RGB)", 2D) = "white"{}
	  _NormalMap ("Normal Map", 2D) = "white" {}
	  _SpecularReflectionSampler("specular Map", 2D) = "white" {}
   }
   SubShader 
   {
      Pass 
	  {      
         Tags { "LightMode" = "ForwardBase" } 
            // pass for ambient light and first light source
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
		 #pragma target 3.0
         
		 #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _UnlitColor;
         uniform float _DiffuseThreshold;
         uniform float4 _OutlineColor;
         uniform float _LitOutlineThickness;
         uniform float _UnlitOutlineThickness;
         uniform float4 _SpecColor; 
         uniform float _Shininess;
		
		 uniform sampler2D _MainTex;
         uniform float4 _MainTex_ST;
		 uniform sampler2D _NormalMap;
		 uniform float4 _NormalMap_ST;
		uniform sampler2D _SpecularReflectionSampler;
		uniform float4 _SpecularReflectionSampler_ST;	
		
         struct vertexInput 
		 {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
			float4 texcoord :  TEXCOORD0;
			float4 tangent  :  TANGENT;
         };
		 
         struct vertexOutput 
		 {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
			float3 TangentViewDir	: TEXCOORD2;
			float3 TangentLightDir	: TEXCOORD3;
			float2 tex      : TEXCOORD4;
         };
 
         vertexOutput vert(vertexInput v) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.posWorld = mul(modelMatrix, v.vertex);
            output.normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			
			TANGENT_SPACE_ROTATION;
			
			output.TangentLightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
			output.TangentViewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
			output.tex = v.texcoord.xy;
		   return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
			float4 lMainTex = tex2D(_MainTex, input.tex);
			float3 TangentLn = normalize(input.TangentLightDir);
			float3 TangentNn = normalize(UnpackNormal(tex2D(_NormalMap, input.tex)).xyz);
			float3 TangentVn = normalize(input.TangentViewDir);
		 
            float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            // default: unlit 
            float3 fragmentColor = _UnlitColor.rgb * lMainTex.rgb; 
				
            // low priority: diffuse illumination
            if (attenuation 
               * max(0.0, dot(TangentNn, TangentLn)) 
               >= _DiffuseThreshold)
            {
               fragmentColor = _Color.rgb * lMainTex.rgb; 
            }
 
            // higher priority: outline
            if (dot(viewDirection, normalDirection) 
               < lerp(_UnlitOutlineThickness, _LitOutlineThickness, 
               max(0.0, dot(normalDirection, lightDirection))))
            {
               fragmentColor = _OutlineColor.a * _OutlineColor.rgb + (1.0f - _OutlineColor.a) * fragmentColor;
            }
 
            // highest priority: highlights
            if (dot(TangentNn, TangentLn) > 0.0 
               // light source on the right side?
               && attenuation *  pow(max(0.0, dot(
               reflect(-TangentLn, TangentNn), 
               TangentVn)), _Shininess) > 0.5) 
               // more than half highlight intensity? 
            {
               fragmentColor = _SpecColor.a 
                   * _SpecColor.rgb
                  + (1.0 - _SpecColor.a) * fragmentColor;
            }
/*			
			//unity chan
			float4 reflectionMaskColor = tex2D( _SpecularReflectionSampler, input.tex );
			float specularDot = dot( TangentNn, TangentVn );
			float4 lighting = lit( specularDot, specularDot, _Shininess );
			float3 specularColor = saturate( lighting.z ) * reflectionMaskColor.rgb * lMainTex.rgb;
			fragmentColor += specularColor;
*/			
            return float4(fragmentColor, 1.0);
         }
 
         ENDCG
      }
 /*
      Pass 
	  {      
         Tags { "LightMode" = "ForwardAdd" } 
            // pass for additional light sources
         Blend SrcAlpha OneMinusSrcAlpha 
            // blend specular highlights over framebuffer
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _UnlitColor;
         uniform float _DiffuseThreshold;
         uniform float4 _OutlineColor;
         uniform float _LitOutlineThickness;
         uniform float _UnlitOutlineThickness;
         uniform float4 _SpecColor; 
         uniform float _Shininess;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - float3(input.posWorld));
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(float3(_WorldSpaceLightPos0));
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  float3(_WorldSpaceLightPos0 - input.posWorld);
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float4 fragmentColor = float4(0.0, 0.0, 0.0, 0.0);
            if (dot(normalDirection, lightDirection) > 0.0 
               // light source on the right side?
               && attenuation *  pow(max(0.0, dot(
               reflect(-lightDirection, normalDirection), 
               viewDirection)), _Shininess) > 0.5) 
               // more than half highlight intensity? 
            {
               fragmentColor = 
                  float4(_LightColor0.rgb, 1.0) * _SpecColor;
            }
 
            return fragmentColor;
         }
 
         ENDCG
      }
 */
	  } 
  
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Specular"
}
