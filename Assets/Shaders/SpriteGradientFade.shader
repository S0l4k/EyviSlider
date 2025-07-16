Shader "Custom/SpriteEdgeGradient"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1
        _EdgeSoftness ("Edge Softness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Lighting Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _Color;
            float _EdgeWidth;
            float _EdgeSoftness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the sprite texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Multiply by the SpriteRenderer's color (including its alpha)
                // This allows you to animate fade in/out via the _Color parameter.
                col.rgb *= _Color.rgb;
                col.a *= _Color.a;

                // Calculate gradient effect on left and right edges.
                // We use the UV's distance from the horizontal center (0.5) of the sprite.
                float gradient = 1.0 - smoothstep(0.5 - _EdgeWidth, 0.5, abs(i.uv.x - 0.5));

                // Clamp the gradient using the softness parameter.
                gradient = clamp(gradient * _EdgeSoftness, 0.0, 1.0);

                // Multiply the current alpha by the gradient.
                col.a *= gradient;

                return col;
            }
            ENDCG
        }
    }
}
