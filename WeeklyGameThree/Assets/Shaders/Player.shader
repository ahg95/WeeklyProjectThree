Shader "Custom/Player"
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SubTex ("RenderTexture", 2D) = "white" {}
        _FirstOutlineColor("FirstOutlineColor", Color) = (0,0,0,1)
        _SecondOultineColor("SecondOutlineColor", Color) = (1,1,1,1)
    }

    SubShader 
    {
        Tags { "RenderType"="TransparentCutout" }
        Blend SrcAlpha OneMinusSrcAlpha 
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
            float4 _MainTex_TexelSize;

            sampler2D _SubTex;
            float4 _SubTex_ST;
            float4 _SubTex_TexelSize;

            float4 _FirstOutlineColor;
            float4 _SecondOultineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_SubTex, i.uv);

                if (c.a > 0) {
                    return c;
                } else {

                    for (int j = 0; j < 4; j++)
                    {
                        // https://en.m.wikipedia.org/wiki/Triangle_wave

                        int nx = abs((j + 1) % 4 - 2) - 1;
                        int ny = abs(j % 4 - 2) - 1;

                        fixed4 nc = tex2D(_SubTex, i.uv + float2(_SubTex_TexelSize.x*nx, _SubTex_TexelSize.y*ny));

                        if (nc.a > 0)
                            return _FirstOutlineColor;
                    }

                    for (int k = 0; k < 8; k++)
                    {
                        int nx = abs((k + 2) % 8 - 4) - 2;
                        int ny = abs((k + 4) % 8 - 4) - 2;

                        fixed4 nc = tex2D(_SubTex, i.uv + float2(_SubTex_TexelSize.x*nx, _SubTex_TexelSize.y*ny));

                        if (nc.a > 0)
                            return _SecondOultineColor;
                    }

                    clip(-1);

                    return _FirstOutlineColor;
                }
            }


            ENDCG
        }
    }
}
