Shader "Unlit/SelectiveRadius"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_color("Color", Color) = (1,1,1,1)
		_outer("Outer Radius", float) = 1
		_inner("Inner Radis", float) = 0.8
		_fade("Fade Length", float) = 0.2
		_width("Visible Arc Width", float) = 1
		
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			Tags{ "Queue" = "Transparent" }

			LOD 100

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
				float4 _MainTex_ST;

				float4 _color;
				float _inner;
				float _outer;
				float _width;
				float _fade;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

					float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

					float x = (i.uv.r - 0.5) * 2;
					float y = (i.uv.g - 0.5) * 2;

					float _angle = atan2(y, x);

					if(grad <= _outer && grad >= _inner && (_angle < _width && _angle > -_width))
					{
						float dist = abs(_angle / _width);
						_color.a *= 1 - saturate(dist);

					}
					else
						return fixed4(0,0,0,0);

					return _color;
				}
				ENDCG
			}
		}
}
