Shader"Custom/FadeVideoShader" {
    Properties {
        _MainTex ("Video Texture", 2D) = "white" {} 
        _Fade ("Fade", Range(0,1)) = 1.0
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass {
Blend SrcAlpha
OneMinusSrcAlpha
            Cull
Off
            ZWrite
Off

            CGPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
#include <UnityCG.cginc>

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
float _Fade;
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
    fixed4 col = tex2D(_MainTex, i.uv);
    col.a *= _Fade;
    return col;
}
            ENDCG
        }
    }
FallBack"Sprites/Default"
}
