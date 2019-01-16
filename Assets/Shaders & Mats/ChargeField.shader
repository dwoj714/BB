Shader "Unlit/ChargeField"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_outerColor("Outer Color", Color) = (0,0,0,1)
		_innerColor("Inner Color", Color) = (1,1,1,1)
		_charge("Charge Percentage", Range(0,1)) = 0.75
		_drag("Mouse Drag Magnitude", Range(0,1)) = 0.5
		_ringColor("Charge indicator color", Color) = (0,0,0.5,0.75)
		_ring("Charge indicator Thickness", float) = 0.05
		_trans("Transition radius", Range(0,1)) = 0.9
		_aMult("Charged radius alpha multiplier", float) = 3
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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

			float4 _outerColor;
			float4 _innerColor;
			float4 _ringColor;
			float _charge;
			float _drag;
			float _ring;
			float _trans;
			float _aMult;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

				if (grad > _trans)
				{
					col = _outerColor;
				}
				else
				{
					col = _innerColor;
				}

				if (grad <= _drag + _ring / 2 && grad >= _drag - _ring / 2)
				{
					col = _ringColor;
				}

				if (grad < _charge)
				{
					col.a *= _aMult;
				}
				return col;

			}
			ENDCG
		}
	}
}
