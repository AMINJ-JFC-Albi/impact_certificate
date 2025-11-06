Shader "Custom/DistanceFadePixelGrid_Uniform_Metallic"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _MinDistance ("Min Visible Distance", Float) = 2.0
        _MaxDistance ("Max Invisible Distance", Float) = 10.0
        _GridSize ("Grid Size", Float) = 30.0
        _LineWidth ("Line Width", Range(0,1)) = 0.05
        _InvertFade ("Invert Fade", Range(0,1)) = 0.0
        _Rotation ("Grid Rotation (deg)", Range(0,360)) = 0.0
        _Metallic ("Metallic", Range(0,1)) = 1.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        fixed4 _Color;
        fixed4 _GridColor;
        float _MinDistance;
        float _MaxDistance;
        float _GridSize;
        float _LineWidth;
        float _InvertFade;
        float _Rotation; // rotation en degrés
        float3 _PlayerPos;
        float3 _ObjectPos;
        half _Metallic;
        half _Smoothness;

        struct Input
        {
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Distance fade
            float dist = distance(_ObjectPos, _PlayerPos);
            float fade = saturate(1.0 - smoothstep(_MinDistance, _MaxDistance, dist));
            fade = lerp(fade, 1.0 - fade, saturate(_InvertFade));

            // Grille en world-space avec rotation
            float rad = radians(_Rotation);
            float cosR = cos(rad);
            float sinR = sin(rad);

            float2 pos = IN.worldPos.xz;
            float2 rotatedPos;
            rotatedPos.x = pos.x * cosR - pos.y * sinR;
            rotatedPos.y = pos.x * sinR + pos.y * cosR;

            float2 grid = rotatedPos * _GridSize;
            float2 cell = frac(grid);

            // Masque des lignes
            float lineX = step(_LineWidth, cell.x) * step(cell.x, 1.0 - _LineWidth);
            float lineY = step(_LineWidth, cell.y) * step(cell.y, 1.0 - _LineWidth);
            float cellMask = lineX * lineY; // 1 = espace/carré, 0 = ligne

            // Fade uniquement sur les espaces
            cellMask = lerp(cellMask, 1.0, fade); 

            // Couleur finale
            fixed4 finalColor = _Color;
            finalColor.rgb = lerp(_GridColor.rgb, finalColor.rgb, cellMask);
            finalColor.a = 1.0;
            finalColor.a *= cellMask;

            // PBR
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
