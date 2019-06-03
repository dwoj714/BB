Shader "Unlit/BurnFieldFX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_startTime("Start time", float) = 0
		_timeScale("Time scale", float) = 1
		_loop("Loop Duration", float) = 5
		_delay("Light-up delay", float) = 0.2
		_color("Color", Color) = (1,1,1,1)
		_flip("Pre/post launch", Range(0,1)) = 0
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
				float _loop;
				float _delay;
				float _flip;
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
					fixed4 col = fixed4(0,0,0,0);

					if(_flip)
					{
						col = _color;

						float _time = (_Time[1] - _startTime) * _timeScale - _delay;

						_loop -= _delay;
	
						//The pixel's distance from the center
						float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

						float flicker = (1 + sin(_time * 40 * (_time / _loop)) / 30);

						//Controls when the flare's size decreses
						grad = 1 - grad / saturate(1.618 * _loop * sin((_time / _loop) * 3.142f)) * flicker;

						//Controls when the flare starts to fade out
						col.a *= (grad * (2 - grad)) * saturate(_loop * sin((_time / _loop) * 3.142f));
					}
					return col;
				}
				ENDCG
			}
		}
}
