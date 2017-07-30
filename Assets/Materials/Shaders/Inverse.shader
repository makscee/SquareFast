Shader "Inverse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Progress ("Progress", Range (0, 1)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _Progress;

			float4 frag(v2f_img i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				if (abs(i.uv.y - 0.5) > _Progress / 2) return c; 
				float4 result = float4(1 - c.r, 1 - c.g, 1 - c.b, c.a);
				return result;
			}
			ENDCG
		}
	}
}