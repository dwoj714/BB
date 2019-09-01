Shader "Unlit/ExplosionFX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_startTime("Start time", float) = 0
		_timeScale("Time scale", float) = 1
		_outer("Outer edge thickness", float) = 0.5
		_color("Color", Color) = (1,1,1,1)
		_bgScale("Background lightness/transparency scale", Range(0,1)) = 0.25
		_flip("Flip Gradient", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
			float _outer;
			float _flip;
			float4 _color;
			float _bgScale;

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
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4 );

				if(_flip > .5)
					grad = 1-grad;

				col.a = grad/_time - _time;

				//The golden ratio sets a perfect base level for having the gradient finish fading out as the effect reaches its maximum radius
				if(col.a >= _outer + 1.618 - _time*1.618)
				{
					if(_timeScale>0)
					{
						col.r += _bgScale;
						col.g += _bgScale;
						col.b += _bgScale;

						col.a = _color.a * _bgScale;
					}
				}

				col.a = clamp(col.a, 0, _color.a);

				return col;
			}
			ENDCG
		}
	}
}
