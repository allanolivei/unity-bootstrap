// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

Shader "Bootstrap/FX/ClipSphere" 
{
    Properties 
    {
		_MainTex("Texture", 2D) = "white" {}
 
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", float) = 1.5
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
				float3 wpos : TEXCOORD1;
			};

        	uniform float _Radius;
        	uniform float3 _Center;
			sampler2D _MainTex;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				// World Position
				o.wpos = mul (unity_ObjectToWorld, v.vertex).xyz;

				// Diffuse
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;

				// Ambient
				o.diff.rgb += ShadeSH9(half4(worldNormal,1));

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// The Same thing
				//if(sign(_radius)*(dot((i.wpos - _ClippingCentre),(i.wpos - _ClippingCentre)) - _radius*_radius)>0) discard;
				clip(-sign(_Radius)*(dot((i.wpos - _Center),(i.wpos - _Center)) - _Radius*_Radius));
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= i.diff;
				return col;
			}
			ENDCG
		}
	}
    FallBack "Diffuse"
}