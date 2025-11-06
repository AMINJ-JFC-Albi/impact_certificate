Shader "Custom/DeepSpace"
{
    Properties
    {
        _Brightness ("Brightness", Range(0,2)) = 1
        _TwinkleSpeed ("Twinkle Speed", Range(0,5)) = 1
        _ScrollSpeed ("Scroll Speed", Range(0,0.5)) = 0.05
        _TintColor ("Star Tint", Color) = (0.9,0.95,1,1)
        _GalaxyColor ("Nebula Color", Color) = (0.25,0.1,0.4,1)
        _NebulaStrength ("Nebula Strength", Range(0,2)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            float _Brightness, _TwinkleSpeed, _ScrollSpeed, _NebulaStrength;
            fixed4 _TintColor, _GalaxyColor;

            float hash(float2 p){ return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453123); }

            float noise(float2 p){
                float2 i=floor(p);
                float2 f=frac(p);
                f=f*f*(3.0-2.0*f);
                float a=hash(i);
                float b=hash(i+float2(1,0));
                float c=hash(i+float2(0,1));
                float d=hash(i+float2(1,1));
                return lerp(lerp(a,b,f.x),lerp(c,d,f.x),f.y);
            }

            float starfield(float2 uv, float t, float depth, out float3 col){
                float2 id=floor(uv);
                float2 gv=frac(uv)-0.5;
                float rnd=hash(id*depth);
                if(rnd>0.25) {col=0; return 0;}
                float s=smoothstep(0.02/depth,0.0,length(gv));
                float tw=0.6+0.4*sin(t*_TwinkleSpeed+rnd*6.2831);
                col=lerp(float3(0.9,0.9,1.0),float3(1.0,0.85,0.7),rnd);
                return s*tw;
            }

            v2f vert(appdata v){
                v2f o;
                o.vertex=UnityObjectToClipPos(v.vertex);
                o.uv=v.uv*10.0;
                return o;
            }

            fixed4 frag(v2f i):SV_Target
            {
                float t=_Time.y;
                float3 colTotal=0;

                // three depth layers, each scrolls at a different rate
                for(int layer=0; layer<3; layer++){
                    float depth=1.0+(layer*0.5);
                    float2 uv=i.uv*depth + float2(t*_ScrollSpeed/depth,0.0);
                    float3 c;
                    float s=starfield(uv,t,depth,c);
                    colTotal+=c*s/depth;
                }

                // nebula
                float2 nuv=i.uv*0.6;
                // Make the nebula slowly drift
                float2 scroll1 = nuv * 2.0 + float2(t * 0.2, t * 0.015);
                float2 scroll2 = nuv * 2.0 + float2(-t * 0.015, t * 0.01) + 10.0;
                
                float neb = noise(scroll1) * noise(scroll2);
                neb = pow(neb, 3.0) * _NebulaStrength;
                float3 nebCol=_GalaxyColor.rgb*neb;

                float3 final=(colTotal*_TintColor.rgb + nebCol)*_Brightness;
                return fixed4(final,1);
            }
            ENDCG
        }
    }
}