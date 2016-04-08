Shader "Unlit/Transparent(double)" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
 
SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    //LOD 200
    Cull Off
 
CGPROGRAM
#pragma surface surf Unlit alpha
 
		inline half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo * atten, s.Alpha);
		}
 
sampler2D _MainTex;
fixed4 _Color;
 
struct Input {
    float2 uv_MainTex;
};
 
void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

}
