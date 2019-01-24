Shader "Unlit/RadialFX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_startTime("Start time", float) = 0
		_timeScale("Time scale", float) = 1
		_outer("Outer edge thickness", float) = 0.5
		_color("Color", Color) = (1,1,1,1)
		_bg("Background Color", Color) = (1,1,1,0.25)
		_flip("State (0/1)", Range(0,1)) = 1
		_offset("Start time offset", float) = 0.5
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
			float _outer;
			float _flip;
			float _offset;
			float4 _color;
			float4 _bg;

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
				float _time = (_Time[1] - (_startTime - _offset)) * _timeScale;

				fixed4 col = _color;

				//Should be the pixel's distance from the center
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

				if(_flip < 0.5)
				{
					_time = cos(_time * _timeScale * 5)/4 + 1;
					col *= cos(grad * _time / (_time * sin(_time * 2)));
				}
				else
				{
					col *= sin(grad * _time / (_time * cos(_time)));
				}
				

				col *= clamp(abs(_time/_timeScale * 0.3) * 2/grad,0,1);

				if (_flip < 0.5)
				{
					col.r += _color.r * grad / 3;
					col.g += _color.g * grad / 3;
					col.b += _color.b * grad / 3;
					col.a /= 2;
				}

				if(_flip < 0.5)
					col *= clamp(grad * 2,0,1) * 2;

				return col;
			}
			ENDCG
		}
	}
}
