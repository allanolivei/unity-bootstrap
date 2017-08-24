// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Bootstrap/Others/SpecularVertex"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 10
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

			uniform half4 _Color;
			uniform float4 _LightColor0;
			uniform float _Shininess;
			uniform float4 _SpecColor;

			struct vertInput
			{
				float4 pos : POSITION;
				float3 nor : NORMAL;
			};

			struct vertOutput
			{
				float4 pos : SV_POSITION;
				half4 col : COLOR;
			};

			vertOutput vert(vertInput input)
			{
				vertOutput o;

				float4 normal = float4(input.nor, 0.0);
				float3 n = normalize(mul(normal, unity_WorldToObject));
				float3 l = normalize(_WorldSpaceLightPos0);
				float3 v = normalize(_WorldSpaceCameraPos);

				float NdotL = max(0.0, dot(n, l));
				float3 a = UNITY_LIGHTMODEL_AMBIENT * _Color;
				float3 d = NdotL * _LightColor0 * _Color;
				float3 r = reflect(-l, n);
				float RdotV = max(0.0, dot(r, v));
				float3 s = float3(0,0,0);
				if (dot(n, l) > 0.0)
					s = _LightColor0 * _SpecColor * pow(RdotV, _Shininess);

				float4 c = float4(d + a + s, 1.0);
				o.col = c;
				o.pos = UnityObjectToClipPos(input.pos);

				return o;
			}

			half4 frag(vertOutput input) : COLOR
			{
				return saturate(input.col);
			}

			ENDCG
		}
	}
}