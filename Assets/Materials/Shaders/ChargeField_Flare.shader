Shader "Unlit/ChargeField"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_color("Outer Color", Color) = (0,0,0,1)
		_charge("Charge Percentage", Range(0,1)) = 0.75
		_drag("Mouse Drag Magnitude", Range(0,1)) = 0.5
		_ringColor("Charge indicator color", Color) = (0,0,0.5,0.75)
		_ring("Charge indicator Thickness", float) = 0.05
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
			float4 _ringColor;
			float _charge;
			float _drag;
			float _ring;

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

				fixed4 col = _color;
				float grad = sqrt((i.uv.r - 0.5f)*(i.uv.r - 0.5f) * 4 + (i.uv.g - 0.5f)*(i.uv.g - 0.5f) * 4);

				//Makes smooth gradients in places
				float powSum = pow(grad, 4) - pow(grad, 8) - pow(_charge * grad, 4);

				col.a *= saturate(grad -(2 * (grad >= _charge)) * (.5 + _drag) * 1.618) + powSum;
				//saturate(col);
				//col.a *= saturate(grad - (1-grad) / (_drag + 0.15));

				col = lerp(col, _ringColor, saturate((1-abs(grad - _drag)*12)));

				return col;

			}
			ENDCG
		}
	}
}
