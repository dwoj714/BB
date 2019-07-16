Shader "Unlit/GravFieldFX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_startTime("Start time", float) = 0
		_timeScale("Time scale", float) = 1
		_color("Color", Color) = (1,1,1,1)
		_flip("Flip Gradient", Range(0,1)) = 1
		_freq("Frequency", float) = 0.5
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Tags{ "Queue" = "Transparent"}

		LOD 100

		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			float _startTime;
			float _timeScale;
			float _flip;
			float4 _color;
			float _freq;

			float4 _MainTex_ST;


			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float _time = (_Time[1] - _startTime) * _timeScale;

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				col = _color;

				//Should be the pixel's distance from the center
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

				//col.a *= ((grad + _time) % _freq) / grad;
				//col.a *= pow((_freq - (grad + _time) % _freq + .1f) / grad, 1);
				col.a *= (_freq - (grad + _time) % _freq + .1f) / (grad + (grad/2));

				return col;
			}
			ENDCG
		}
	}
}
