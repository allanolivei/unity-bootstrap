Shader "Bootstrap/FX/TintFocus"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_Tint("Tint Color", Color) = (1, 1, 1, 1)
		_FXScale("FX Scale", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR1;
			};

			sampler2D _MainTex;
			uniform float4 _Tint;
			uniform fixed _FXScale;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				// diffuse
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;

				// ambient light
				o.diff.rgb += ShadeSH9(half4(worldNormal,1));

				//ping pong 0 - 1. (the value multiply time change speed)
				float pingpong = (sin(_Time.y * 3.0) + 1) * 0.5;

				// tint
				//o.diff += _Tint * pingpong * _FXScale;
				o.color = (_Tint * pingpong * 0.3 + _Tint * 0.2) * _FXScale;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * (1 - i.color.r) + i.color;
				col *= i.diff;
				return col;
			}
			ENDCG
		}
	}
}