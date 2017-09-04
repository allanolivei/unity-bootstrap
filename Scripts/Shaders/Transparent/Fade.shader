// Copyright (c) 2017-2018 Allan Oliveira Marinho, Inc. All Rights Reserved.

Shader "Bootstrap/Transparent/Fade" 
{
	Properties 
	{
	    [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
	    _Color("Color", Color) = (1, 1, 1, 1)
	}
	 
	SubShader 
	{
	    Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
	    LOD 200
	   
	    Pass 
	    {
	        ZWrite On
	        ColorMask 0
	   
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	 
	        struct v2f 
	        {
	            float4 pos : SV_POSITION;
	        };
	 
	        v2f vert (appdata_base v)
	        {
	            v2f o;
	            o.pos = UnityObjectToClipPos (v.vertex);
	            return o;
	        }
	 
	        half4 frag (v2f i) : COLOR
	        {
	            return half4 (0,0,0,0);
	        }
	        ENDCG  
	    }

	    Pass
	    {

	    	Blend SrcAlpha OneMinusSrcAlpha

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
			uniform float4 _Color;

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

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb *= i.diff.rgb;
				return col * _Color;
			}
		    ENDCG
		}
	}
	 
	Fallback "Transparent/Diffuse"
}