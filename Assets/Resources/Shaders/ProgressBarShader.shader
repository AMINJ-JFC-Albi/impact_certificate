Shader "Custom/StarfieldProgressBar_Additive"
{
    Properties
    {
        _UVAspect ("UV Aspect (X/Y)", Vector) = (15, 1, 0, 0)
        _Progress     ("Progress", Range(0,1)) = 0.0
        _StarColor    ("Star Color", Color) = (0.8, 0.9, 1, 1)
        _StarSize ("Star Size", Range(0.6, 1.8)) = 1.2
        _StarDensity  ("Star Density", Range(10,300)) = 100
        _TwinkleSpeed ("Twinkle Speed", Range(0,10)) = 2
        _GlowStrength ("Glow Strength", Range(0,2)) = 0.6
        _GlowWidth    ("Glow Width", Range(0.001,0.2)) = 0.02
        _Alpha        ("Global Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZWrite Off
        Blend One One  // <-- additive blend, brightens space behind
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };
            
            float2 _UVAspect;
            float _Progress;
            float4 _StarColor;
            float _StarSize;
            float _StarDensity;
            float _TwinkleSpeed;
            float _GlowStrength;
            float _GlowWidth;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float x = i.uv.x;
                float y = i.uv.y;
                float t = _Time.x;
                float progress = saturate(_Progress);
                float mask = step(x, progress);

                float3 col = 0; // no opaque background

                if (mask > 0.5)
                {
                    float2 uv = i.uv * _StarDensity;
                    uv = uv * _UVAspect.xy;
                    float2 cell = floor(uv);
                    float2 local = frac(uv);
                    float rnd = hash(cell);

                   if (rnd > 0.98)
                    {
                        float2 starPos = frac(float2(sin(rnd*123.4), cos(rnd*456.7)));
                        float dist = length(local - starPos);
                        float size = lerp(0.05, _StarSize, rnd);
                        float brightness = smoothstep(size, 0.0, dist);
                        float twinkle = 0.7 + 0.3 * sin(t * _TwinkleSpeed + rnd * 10.0);
                        col += _StarColor.rgb * brightness * twinkle;
                    }
                }

                float glow = exp(-pow((x - progress)/_GlowWidth, 2.0)) * _GlowStrength;
                col += _StarColor.rgb * glow;

                return float4(col * _Alpha, _Alpha);
            }
            ENDCG
        }
    }
}
