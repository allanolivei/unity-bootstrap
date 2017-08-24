Shader "Bootstrap/Lit/TextureAmbientDiffuseRim"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
	_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(0, 3)) = 1.0
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

	/*struct appdata
	{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	};*/

	sampler2D _MainTex;

	uniform float4 _RimColor;
	fixed _RimPower;

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

		// rim light
		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		float dotProduct = 1 - dot(v.normal, viewDir);
		fixed4 rim = smoothstep(1 - _RimPower, 1.0, dotProduct) * _RimColor;
		o.diff += rim;

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