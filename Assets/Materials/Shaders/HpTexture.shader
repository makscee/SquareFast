// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HpTexture"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Hp ("HP", int) = 2
        _CurHp ("CurHP  ", int) = 2
        _Center("Center", float) = 0.5
        _Rot("Rot", float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            //#pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            int _Hp;
            int _CurHp;
            float _Center;
            float _Rot;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;

                return OUT;
            }

            sampler2D _MainTex;
            
            fixed2 rotate(fixed2 v, float a) {
                float s = sin(a);
                float c = cos(a);
                return mul(v, fixed2x2(c, -s, s, c));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.texcoord) * i.color;
                const float PI = 3.14159;
				fixed2 vec = i.texcoord - fixed2(0.5, _Center);
//				fixed t = max(abs(vec.x), abs(vec.y));
//				if (t > 0.45)
//				{
//				    c.rgb *= c.a;
//				    return c;
//				}
				
				vec = normalize(vec);
				
				
				// ??????????????
                if (vec.x > 0.9999) vec.x = 1;
                if (vec.x < -0.9999) vec.x = -1;
				
				float ang1 = acos(vec.x);
				if(vec.y < 0) ang1 = PI * 2 - ang1;
				ang1 -= PI / 2 + _Rot;
				if (ang1 < 0) ang1 += PI * 2;
				
				float delta = 0.03;
				if (_Hp > 1) {
				    vec = i.texcoord - fixed2(0.5, _Center);
				    [unroll(8)]
                    for (int k = 0; k <= _Hp; k++)
                    {
                        float ang2 = PI * 2 * k / _Hp;
                        if (ang1 <= ang2)
                        {
                            if (_CurHp < k)
                            {
                                c.rgb *= 0.2;
                                break;
                            } else
                            {
                                c.rgb *= 0.7 * k / _Hp + 0.3;
                                break;
                            }
                        }
                    }
				}
				
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}