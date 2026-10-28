Shader "Custom/ChunkShader"
{
    Properties
    {
        _MainTex   ("Base Texture", 2D) = "white" {}
        _SecondTex ("Light Texture", 2D) = "white" {}
        _Opacity ("Light Opacity", Range(0,1)) = 1.0    
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SecondTex;
            float _Opacity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0    : TEXCOORD0;
                float2 uv1    : TEXCOORD1;
                float2 uv2    : TEXCOORD2;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv0;
                o.uv1 = v.uv1;
                o.uv2 = v.uv2;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c0 = tex2D(_MainTex,   i.uv0);
                fixed4 c1 = tex2D(_SecondTex, i.uv1);
                fixed4 c2 = tex2D(_SecondTex, i.uv2);
                fixed overlayAlpha1 = c1.a;
                fixed overlayAlpha2 = lerp(c2.a, 0.99, 1 - _Opacity);
                fixed overlayAlpha = min(overlayAlpha1, overlayAlpha2);
                fixed3 rgb= lerp(c0.rgb, (overlayAlpha2 < overlayAlpha1) ? c2.rgb : c1.rgb, overlayAlpha);

                fixed a = c0.a + overlayAlpha * (1.0 - c0.a);

                return fixed4(rgb, a);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
