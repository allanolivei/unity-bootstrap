// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

Shader "Bootstrap/FX/RimFocus"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
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
			};

			sampler2D _MainTex;
			uniform float4 _RimColor;
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

				// rim light
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float dotProduct = 1 - dot(v.normal, viewDir);
				float _RimPower = pingpong * 0.5f + 0.6f;
				fixed4 rim = smoothstep(1 - _RimPower, 1.0, dotProduct) * _RimColor;
				o.diff += rim * _FXScale;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= i.diff;
				return col;
			}
			ENDCG
		}
	}
}