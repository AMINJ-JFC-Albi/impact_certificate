Shader "Custom/TeleporterCylinder"
{
    Properties
    {
        _MainTex ("Energy Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (0.3, 0.8, 1, 1)
        _Speed ("Scroll Speed", Float) = 1
        _Intensity ("Intensity", Float) = 2

        _Open ("Open Amount", Range(0,1)) = 1
        _Softness ("Edge Softness", Range(0.001,0.5)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Blend One One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;
            float _Intensity;
            float _Open;
            float _Softness;

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : TEXCOORD1;
        };


            v2f vert (appdata v)
            {
                v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scroll vertical
                float2 uv = i.uv;
                uv.y += _Time.y * _Speed;

                fixed4 tex = tex2D(_MainTex, uv);

                // --- IGNORER CAPS (haut / bas) ---
                float vertical = abs(i.normal.y);   // 1 sur les caps, 0 sur les côtés
                float capMask = step(vertical, 0.5);

                // --- MASQUE VERTICAL (bas -> haut) ---
                float height = i.uv.y;
                float mask = smoothstep(_Open, _Open - _Softness, height);

                // --- COMBINAISON ---
                float finalMask = mask * capMask;

                fixed4 col = tex * _Color * finalMask * _Intensity;
                return col;
            }
            ENDCG
        }
    }
}
