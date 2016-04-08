Shader "Hidden/RadialBlurFilter"
{
	Properties
	{
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	    _SampleDist ("SampleDist", Float) = 1
	    _SampleStrength ("SampleStrength", Float) = 2.2
		_PointPos ("PointPos", Vector) = (0.5, 0.5, 0, 0)			
	}

	SubShader
	{
	    Pass
	    {
	        ZTest Always Cull Off ZWrite Off
	        Fog { Mode off }

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _SampleDist;
			uniform float _SampleStrength;
			uniform float4 _PointPos;
			
			static const float samples[10] ={ -0.08, -0.05, -0.03, -0.02, -0.01, 0.01, 0.02, 0.03, 0.05, 0.08};
			
			
			struct v2f {
			    float4 pos : POSITION;
			    float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_img v)
			{
			    v2f o;
			    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			    o.uv = v.texcoord.xy;
			    return o;
			}

			float4 frag (v2f i) : COLOR
			{
				float2 texCoord = i.uv;

			    float2 dir = _PointPos.xy - texCoord;
			    float dist = length(dir);
			    dir /= dist;
			    float4 color = tex2D(_MainTex, texCoord);
			    float4 sum = color;
			    
			    for (int i = 0; i < 10; ++i)
				{
			        sum += tex2D(_MainTex, texCoord + dir * samples[i] * _SampleDist);
			    }

			    sum *= 1.0 / 11.0;
			    float t = saturate(dist * _SampleStrength);
			    return lerp(color, sum, t);
			}
			ENDCG
		}
	}
	Fallback off
}
