Shader "ColorSwitch" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Progress ("Progress", Range (0, 1)) = 0
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _Progress;
			uniform float _InvProgress;
			uniform float4 _Color;

			float4 frag(v2f_img i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				if (c.r > 0.98) return c;
				if (abs(i.uv.y - 0.5) < _InvProgress / 2) return c; 
				if (_Progress <= 0 || abs(i.uv.x - 0.5) < (1 - _Progress) / 2) return c * _Color; 
				float4 result = float4(1, 1, 1, 1);
				return result;
			}
			ENDCG
		}
	}
}