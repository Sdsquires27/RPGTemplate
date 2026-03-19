Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Float) = 1
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _AlphaThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // Sample alpha at a given UV offset
            float sampleAlpha(float2 uv, float2 offset)
            {
                return tex2D(_MainTex, uv + offset).a;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // If pixel is transparent, return nothing
                if (col.a <= _AlphaThreshold)
                    return fixed4(0, 0, 0, 0);

                float tx = _OutlineThickness * _MainTex_TexelSize.x;
                float ty = _OutlineThickness * _MainTex_TexelSize.y;

                // Check if any neighbor is transparent
                bool onEdge =
                    tex2D(_MainTex, i.uv + float2(  tx,      0        )).a <= _AlphaThreshold ||
                    tex2D(_MainTex, i.uv + float2(  0.5*tx,  0.866*ty )).a <= _AlphaThreshold ||
                    tex2D(_MainTex, i.uv + float2( -0.5*tx,  0.866*ty )).a <= _AlphaThreshold ||
                    tex2D(_MainTex, i.uv + float2( -tx,      0        )).a <= _AlphaThreshold ||
                    tex2D(_MainTex, i.uv + float2( -0.5*tx, -0.866*ty )).a <= _AlphaThreshold ||
                    tex2D(_MainTex, i.uv + float2(  0.5*tx, -0.866*ty )).a <= _AlphaThreshold;

                if (onEdge)
                    return _OutlineColor;

                return col * i.color;
            }
            ENDCG
        }
    }
}