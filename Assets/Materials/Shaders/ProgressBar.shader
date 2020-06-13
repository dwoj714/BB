Shader "Unlit/ProgressBar"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "" {}

		_color1("Empty Color", Color) = (0,1,1,1)
		_color2("Filled Color", Color) = (1,0,1,1)

		_seg("Segments", float) = 3
		_fill("Filled Segments", float) = 1
		_gap("Segment Gap", float) = 0.1

		[PerRendererData] _StencilComp("Stencil Comparison", float) = 8.000000
		[PerRendererData] _Stencil("Stencil ID", float) = 0.000000
		[PerRendererData] _StencilOp("Stencil Operation", float) = 0.000000
		[PerRendererData] _StencilWriteMask("Stencil Write Mask", float) = 255.000000
		[PerRendererData] _StencilReadMask("Stencil Read Mask", float) = 255.000000
		[PerRendererData] _ColorMask("Color Mask", float) = 15.000000
		[Toggle(UNITY_UI_ALPHACLIP)]  _UseUIAlphaClip("Use Alpha Clip", float) = 0.000000

	}
		SubShader
		{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "true" }
			//ZTest[unity_GUIZTestMode]

			Stencil {
				Ref[_Stencil]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
				Comp[_StencilComp]
				Pass[_StencilOp]
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]


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
				float _gap;
				float _seg;
				float _fill;

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

					//pixel's x position
					float x = i.uv.r;

					//separation between segment start and endpoints
					float segLen = 1 / _seg;

					//half of the gap between segments
					float halfGap = _gap / 2;

					for (float i = 0; i <= 1; i += segLen)
					{
						
						if (x < halfGap || x > 1 - halfGap)
						{
							col = (x < _fill / _seg) ? _color2 : _color1;
						}

						if (x >= i + halfGap && x <= i + segLen - halfGap)
						{
							col = (x < _fill / _seg) ? _color2 : _color1;
						}
					}

					return col;
				}
			ENDCG
			}
		}
}
