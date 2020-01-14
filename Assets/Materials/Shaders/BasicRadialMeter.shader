Shader "Unlit/BasicRadialMeter"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "" {}

		_color1("Color 1", Color) = (0,1,1,1)
		_color2("Color 2", Color) = (1,0,1,1)

		_charge("percentage", Range(0,1)) = 0.5
		_inner("Inner radius", Range(0,1)) = 0.9
		_outer("Outer radius", Range(0,1)) = 1

		_flip("Clockwise? (bool)", Range(0,1)) = 1
		_sat("Saturate? (bool)", Range(0,1)) = 1

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

				fixed4 _color1;
				fixed4 _color2;

				float _charge;
				float _inner;
				float _outer;
				float _flip;
				float _sat;
				

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
					fixed4 white = fixed4(1,1,1,1);

					//Should be the pixel's distance from the center
					float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

					//pixel's x and y positions
					float x = (i.uv.r - 0.5) * 2;
					float y = (i.uv.g - 0.5) * 2;

					//the angle of the pixel, counterclockwise from the top, 0-1 range
					float angle = .5 + atan2(x, -y) / 6.283185;

					if (grad <= _outer && grad > _inner && (_flip < 0.5 ? angle : 1 - angle) <= _charge)
					{
						float4 _deltaColor = _color1 - _color2;

						//Sets color based on percentage
						col = _color2 + _deltaColor * _charge;

						//Saturates intermediate colors
						if(_sat > 0.5f)
						{
							//Get the largest rgb value
							float mod = col.r;
							if(mod < col.g) mod = col.g;
							if(mod < col.b) mod = col.b;
							saturate(mod);
						
							//make it into a scalar that saturates the top value
							mod = 1/mod;

							col.r *= mod;
							col.g *= mod;
							col.b *= mod;
						}

					}

					return col;
				}
			ENDCG
			}
		}
}
