Shader "SeaCellular"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2 random2(float2 st)
            {
                st = float2(dot(st, float2(127.1, 311.7)),
                            dot(st, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
            }


            float cellularnoise(float2 st, float n) {
                st *= n;

                float2 ist = floor(st);
                float2 fst = frac(st);

                float distance = 5;

                for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                    {
                        float2 neighbor = float2(x, y);
                        float2 p = 0.5 + 0.5 * sin(_Time.y + 6.2831 * random2(ist + neighbor));
                        float2 diff = neighbor + p - fst;
                        distance = min(distance, length(diff));
                    }

                float color = distance * 0.2;

                return color;
            }


            fixed4 frag(v2f i) :  SV_Target{

                return cellularnoise(i.uv, 16) * float4(1, 2, -2, 1) + float4(0.1,0.5,1.5,1);
            }
            ENDCG
        }
    }
}