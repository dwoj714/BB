Shader "Unlit/PulseFX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_startTime("Start time", float) = 0
		_timeScale("Time scale", float) = 1
		_min("Minimum alpha value", Range(0,1)) = .25
		_color("Color", Color) = (1,1,1,1)
		_charge("Recahrge Percentage", Range(0,1)) = 0
		_ring("Indicator range", Range(0,1)) = .95
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Tags{ "Queue" = "Transparent" }

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
			float _min;
			float _charge;
			float _ring;
			float4 _color;

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

				//Should be the pixel's distance from the center
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

				fixed4 col = _color;

				col.a = clamp(_color.a - _time, _min, 1);

				if ((grad > _charge - _ring/2 && grad < _charge + _ring/2) || grad > 1 - _ring)
				{
					col = _color;
				}

				return col;
			}
			ENDCG
		}
	}
}
