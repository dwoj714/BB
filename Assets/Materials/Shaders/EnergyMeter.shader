Shader "Unlit/EnergyMeter"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "" {}

		_color1("Color 1", Color) = (0,1,1,1)
		_color2("Color 2", Color) = (1,0,1,1)
		_color3("Color 3", Color) = (1,1,0,1)

		_seg("Segments", float) = 3
		_gap("Segment Gap", float) = 0.1
		_charge("Charge percentage", Range(0,1)) = 0.5
		_inner("Inner radius", Range(0,1)) = 0.9

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
				fixed4 _color3;
				float _gap;

				float _seg;
				float _charge;
				float _inner;

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

					//angular separation between segment start and endpoints
					float segLen = 1 / _seg;

					//half of the gap between segments
					float halfGap = _gap / 2;

					//the range of angles indicating the first incomplete charge segment
					float minSeg = (_charge < segLen) ? _charge : (_charge % segLen);


					//good luck interpreting this later
					if (grad > _inner)
					{
						//make the first halfgap of the circle white
						if(angle < halfGap)
						{
							col = white;
						}
						//fill the segment up to minseg with color1 if not fully charged, otherwise color2
						else if (angle < minSeg - halfGap)
						{
							col = _charge < 1 ? _color1 : _color2;
						}
						//draw a gap between the end of the first segment and the next
						else if (angle < minSeg + halfGap)
						{
							col = white;
						}
						else
						{
							for (float i = minSeg; i < (_charge - minSeg); i += segLen)
							{
								if(i != (_charge - minSeg) && angle > i + halfGap && angle < i + segLen - halfGap)
								{
									if(i + segLen >= _charge - minSeg)
										col = _color3;
									else
										col = _color2;
								}
							}

							for (float j = minSeg; j <= 1 - minSeg; j += segLen)
							{
								if (angle > j - halfGap + segLen && angle < j + halfGap + segLen)
									col = white;
							}

						}

						if(angle > 1 - halfGap)
							col = white;
					}
					
					return col;
				}
			ENDCG
			}
		}
}
