Shader "Custom/AlienMetal_CorrectedWaves"
{
    Properties
    {
        _BaseColor ("Metal Base Color", Color) = (0.15, 0.8, 0.6, 1)
        _WaveColor ("Wave Highlight Color", Color) = (1, 0.3, 0.8, 1)
        _Metallic ("Metallic", Range(0,1)) = 0.9
        _Smoothness ("Smoothness", Range(0,1)) = 0.8
        _WaveIntensity ("Wave Intensity", Range(0,3)) = 1.0
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _NoiseScale ("Noise Scale", Float) = 2.0
        _NormalDistortion ("Normal Distortion", Range(0,1)) = 0.2
        _FresnelPower ("Fresnel Power", Range(0,8)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #include "UnityCG.cginc"

        fixed4 _BaseColor;
        fixed4 _WaveColor;
        half _Metallic;
        half _Smoothness;
        float _WaveIntensity;
        float _WaveSpeed;
        float _NoiseScale;
        float _NormalDistortion;
        float _FresnelPower;

        struct Input
        {
            float3 worldPos;
            float3 viewDir;
        };

        float hash(float3 p)
        {
            p = frac(p * 0.3183099 + 0.1);
            p *= 17.0;
            return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
        }

        float noise(float3 p)
        {
            float3 i = floor(p);
            float3 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);
            return lerp(
                lerp(
                    lerp(hash(i + float3(0,0,0)), hash(i + float3(1,0,0)), f.x),
                    lerp(hash(i + float3(0,1,0)), hash(i + float3(1,1,0)), f.x),
                    f.y
                ),
                lerp(
                    lerp(hash(i + float3(0,0,1)), hash(i + float3(1,0,1)), f.x),
                    lerp(hash(i + float3(0,1,1)), hash(i + float3(1,1,1)), f.x),
                    f.y
                ),
                f.z
            );
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 p = IN.worldPos * _NoiseScale + _Time.y * _WaveSpeed;

            // Génération du motif de vague
            float wave = noise(p * 0.6) + noise(p * 1.2 + 5.0);
            wave = smoothstep(0.55, 0.8, wave); // seuil plus clair = moins de bruit, plus visible

            // Perturbation de la normale pour accentuer les reflets
            float3 grad;
            grad.x = noise(p + float3(0.1,0,0)) - noise(p - float3(0.1,0,0));
            grad.y = noise(p + float3(0,0.1,0)) - noise(p - float3(0,0.1,0));
            grad.z = noise(p + float3(0,0,0.1)) - noise(p - float3(0,0,0.1));
            o.Normal = normalize(o.Normal + grad * _NormalDistortion);

            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);

            float highlight = saturate((1.0-wave) * _WaveIntensity + fresnel * 0.25);

            fixed3 col = lerp(_BaseColor.rgb, _WaveColor.rgb, highlight);

            o.Albedo = saturate(col);
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Emission = _WaveColor.rgb * highlight * 0.25; // brillance sur les zones wave
        }
        ENDCG
    }

    FallBack "Metallic"
}
