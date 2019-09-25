Shader "Image Effect/CamShake"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_noiseTex ("Noise pattern", 2D) = "white" {}
		_mag("Shake magnitude", float) = 0.1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _noiseTex;
			float _mag;

            fixed4 frag (v2f i) : SV_Target
            {
				float a = (1 + sin(_Time[1])) / 2;
				float b = (1 + cos(_Time[1])) / 2;
				
				float4 select1 = tex2D(_noiseTex, float2(a, b));
				float4 select2 = tex2D(_noiseTex, float2(b, a));
								
                fixed4 col = tex2D(_MainTex, i.uv + float2(select1[0] - 0.5f, select2[0] - 0.5f) * _mag);

                return col;
            }
            ENDCG
        }
    }
}
