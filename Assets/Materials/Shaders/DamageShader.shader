﻿Shader "Unlit/DamageShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_noise ("Noise Pattern", 2D) = "white" {}
		_color1 ("Start Color", Color) = (1,1,1,1)
		_color2 ("Damaged Color", Color) = (1,0,0,1)
		_flash ("Flash color", Color) = (1,1,1,1)
		_flicker ("Flicker speed", float) = 10
		_health ("Health Percentage", Range(0,1)) = 1
		_trans ("Shake Transition Percentage", Range(0,1)) = .5
		_shakeInt("Max Shake Intensity", Range(0,250)) = 10
		_shakeVar("Min Shake Intensity", Range(-250,250)) = 30

		//_maxVibrance ("Maximize Vibrance", Boolean) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		
		//Tags { "RenderType"="Opaque" }

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
			sampler2D _noise;

			float4 _MainTex_ST;
			float _health;
			float4 _color1;
			float4 _color2;
			float4 _flash;
			float _flicker;
			float _trans;
			float _shakeInt;
			float _shakeVar;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				if(_health <= _trans)
				{
					float4 tex1 = tex2Dlod(_noise, float4(abs(sin(_Time[1])), 0, 0, 0));
					float4 tex2 = tex2Dlod(_noise, float4(0, abs(cos(_Time[1])), 0, 0));

					float intDamp = (1000 - _shakeInt);//Intensity Damping
					float2 offset = float2(tex1.r - 0.5f,tex2.r - 0.5f) * ( (_shakeInt/1000 - _shakeVar/1000) * (1 - _health) + _shakeVar/1000);
				  //float2 offset = float2(tex1.r - 0.5f,tex2.r - 0.5f)/(300 - (270 * (1 - _health)));

					o.vertex.r += offset.r;
					o.vertex.g += offset.g;
				}

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

				float4 _deltaColor = _color1 - _color2;
				fixed4 col = _color2 + _deltaColor * _health;
				
				if (_health <= 0 && floor(_Time[1] * _flicker) % 2==0)
				{
					col = _flash;
				}

				return col;
			}
			ENDCG
		}
	}
}
