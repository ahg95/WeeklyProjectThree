Shader "Custom/Player"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SubTex ("RenderTexture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" }
        LOD 100

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

            sampler2D _SubTex;
            float4 _SubTex_ST;

            fixed _Cutoff;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_SubTex, i.uv);
                clip(col.a - _Cutoff);
                return col;
            }
            ENDCG
        }
    }
}
