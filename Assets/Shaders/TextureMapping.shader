Shader "Custom/TextureMapping"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            //C for Graphics 
            CGPROGRAM
            //コンパイラに対して情報を渡すための命令
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
        //頂点
        float4 vertex : POSITION;//　←:のあとはセマンティクス
        //法線
        float4 normal : NORMAL;
    };

    //Vertex to Fragament Shader で実際に使うための変数
    struct v2f
    {
        //座標変換された後の頂点座標
        float4 vertex : SV_POSITION;
        //UV座標 UV展開のやつ
        float4 observerSpacePos : TEXCOORD0;
        float3 worldPos : TEXCOORD1;
        float3 worldNormal : TEXCOORD2;
    };

    //テクスチャ　C#側のTexture2Dと同じ
    sampler2D _ObserverTexture;
    //VP行列
    float4x4 _ObserverMatrixVP;
    //観測者座標
    float4 _ObserverPos;

    //頂点シェーダー
    v2f vert(appdata v)
    {
        v2f o;
        //3D空間からスクリーン上の位置への座標変換
        o.vertex = UnityObjectToClipPos(v.vertex);
        //モデル（Model）変換、ビュー（View）変換、プロジェクション（Projection）変換
        o.observerSpacePos = mul(mul(_ObserverMatrixVP, unity_ObjectToWorld), v.vertex);
        // xyを-w～wの値に変換する
        o.observerSpacePos = ComputeScreenPos(o.observerSpacePos);
        //法線
        o.worldNormal = UnityObjectToWorldNormal(v.normal);

        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        return o;
    }

    //フラグメントシェーダー
    fixed4 frag(v2f i) : SV_Target
    {
        half4 color = 1;
        //プロジェクション座標に変換
        i.observerSpacePos.xyz /= i.observerSpacePos.w;
        float2 uv = i.observerSpacePos.xy;
        //UV座標(uv)からテクスチャ(_ObserverTexture)上のピクセルの色を計算
        float4 observerTex = tex2D(_ObserverTexture, uv);
        // カメラの範囲外には適用しない(0-1の範囲だけ)// 0.0 if edge > x , else 1.0  T step(T edge, T x)
        fixed3 isOut = step((i.observerSpacePos - 0.5) * sign(i.observerSpacePos), 0.5);
        float alpha = isOut.x * isOut.y * isOut.z;
        // 観測者から見て裏側の面には適用しない
        alpha *= step(-dot(lerp(-_ObserverPos.xyz, _ObserverPos.xyz - i.worldPos, _ObserverPos.w), i.worldNormal), 0);
        
        if (alpha == 0)
        {

            return fixed4(1.0, 1.0, 1.0, 1.0);
        }
        else
        {
            return observerTex * alpha;

        }
    }
    ENDCG
}
    }
}