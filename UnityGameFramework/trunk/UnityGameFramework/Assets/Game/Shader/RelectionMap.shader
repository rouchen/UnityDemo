Shader "Custom/RelectionMap" {
   Properties {
      _Cube("Reflection Map", Cube) = "" {}
   }
   SubShader {
      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
         // User-specified uniforms
         uniform samplerCUBE _Cube;   
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 normalDir : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            //float4x4 modelMatrix = _Object2World;
            //float4x4 modelMatrixInverse = _World2Object; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
			
			/*
            output.viewDir = float3(mul(modelMatrix, input.vertex) 
              - float4(_WorldSpaceCameraPos, 1.0));
            output.normalDir = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
			*/
			
			output.normalDir = input.normal;
			output.viewDir = ObjSpaceViewDir(input.vertex) * -1.0f;
			
			output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 reflectedDir = 
               reflect(input.viewDir, normalize(input.normalDir));
            return texCUBE(_Cube, reflectedDir);
         }
 
         ENDCG
      }
   }
}
