Shader "Unlit/ChromaKeyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (0, 1, 0, 1) // 透過させる色 (デフォルトは緑)
        [Range(0, 1)]
        _Threshold ("Threshold", Float) = 0.4 // 色の許容範囲
        [Range(0, 1)]
        _Smoothing ("Smoothing", Float) = 0.1 // 境界の滑らかさ
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // アルファブレンディングを有効にする
        Cull Off // ポリゴンの裏側も描画する

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
            fixed4 _KeyColor;
            float _Threshold;
            float _Smoothing;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // キーカラーとピクセルカラーの差を計算
                float distance = distance(col.rgb, _KeyColor.rgb);

                // smoothstepを使って、しきい値とスムージングに基づいてアルファ値を計算
                // これにより、境界がギザギザにならず、滑らかになる
                float alpha = smoothstep(_Threshold - _Smoothing, _Threshold + _Smoothing, distance);
                
                // 計算したアルファ値を適用
                col.a = alpha;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}