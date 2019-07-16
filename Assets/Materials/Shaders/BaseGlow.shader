﻿Shader "Unlit/BaseGlow"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_startTime("Start time", float) = 0
		_timeScale("Time scale", float) = 1
		_color("Color", Color) = (1,1,1,1)

		_core("Core FX color", Color) = (1,1,0,1)
		_depth("Core radius", Range(0, 1)) = 0.25
		_thing("Frequency thingy", float) = 1

		_glow("Glow Color", Color) = (1,1,1,0.25)
		_dist("Glow Distance", Range(0,1)) = 0.1
		_angle("Impact Angle", float) = 3.14
		_acc("Fade Acceleration", float) = 3
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
			float4 _color;
			float4 _core;
			float _thing;
			float _depth;
			float4 _glow;
			float _dist;
			float _angle;
			float _acc;

			float4 _MainTex_ST;


			v2f vert(appdata v)
			{
				float _time = (_Time[1] - _startTime) * _timeScale;

				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _color;
				
				float _time = (_Time[1] - _startTime) * _timeScale;

				//Should be the pixel's distance from the center
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

				float x = (i.uv.r - 0.5) * 2;
				float y = (i.uv.g - 0.5) * 2;

				if (grad > (1-_dist) && atan2(y, x) < _angle + _time && atan2(y, x) > _angle - _time)
				{
					col = lerp(_glow, _color, saturate(_time * _acc));
				}

				//code for the core FX (visible during weapon selection)

				float4 coreCol = _core;
				coreCol.a *= sin( 16.18 / pow (pow(x,4) * (cos(6.28 + _thing * (x * y))/2 + .5), pow(y,4)) + _Time[1]);
				//coreCol.a *= sin(16.18 / pow(pow(x, 4) * (cos(_thing * (x * y)) / 2 + .5), pow(y, 4)) + _Time[1]);
				if (grad < _depth)
				{
					col = coreCol;
				}
				else
				{
					float share = saturate((grad - _depth) * 6);
					col = (1 - share) * saturate(coreCol) + share * col;
				}

				return col;
			}
		ENDCG
		}
	}
}
