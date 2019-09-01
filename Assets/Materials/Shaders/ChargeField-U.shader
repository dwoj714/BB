Shader "Unlit/ChargeField-U"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_charge("Charge Percentage", Range(0,1)) = 0.75
		_drag("Mouse Drag Magnitude", Range(0,1)) = 0.5
		_ringColor("Charge indicator color", Color) = (0,0,0.5,0.75)
		_ring("Charge indicator Thickness", float) = 0.05
		_aMult("Charged radius alpha multiplier", float) = 1.5
		_uaMult("UN-Charged radius alpha multiplier", float) = 0.5
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

				float4 _ringColor;
				float _charge;
				float _drag;
				float _ring;
				float _aMult;
				float _uaMult;

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
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);

					//return color if color is pure black and/or fully transparent
					if (col.a == 0 || (col.r == 0 && col.g == 0 && col.b == 0))
					{
						return col;
					}

					float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

					if (grad <= _drag + _ring / 2 && grad >= _drag - _ring / 2)
					{
						col = _ringColor;
					}

					if (grad < _charge)
					{
						col.a *= _aMult;
					}
					else
					{
						col.a *= _uaMult;
					}

					return col;

				}
				ENDCG
			}
		}
}
