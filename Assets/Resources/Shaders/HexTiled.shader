Shader "Custom/HexTiledLitFlatTopCrisp"
{
    Properties
    {
        _TileSize("Tile Size", Float) = 1.0
        _BorderSize("Border Size", Range(-0.5,0.5)) = 0.05
        _TileColor("Tile Color", Color) = (1,1,1,1)
        _BorderColor("Border Color", Color) = (0,0,0,1)
        _Metallic("Metallic", Range(0,1)) = 0.0
        _InvertMetallic("Invert Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #include "UnityCG.cginc"

        struct Input
        {
            float3 worldPos;
        };

        float _TileSize;
        float _BorderSize;
        float4 _TileColor;
        float4 _BorderColor;
        float _Metallic;
        float _InvertMetallic;
        float _Smoothness;

        // Flat-top hex math
        float2 pixel_to_axial(float2 p, float size)
        {
            float q = (2.0/3.0 * p.x) / size;
            float r = (-1.0/3.0 * p.x + sqrt(3.0)/3.0 * p.y) / size;
            return float2(q, r);
        }

        float2 axial_round(float2 ar)
        {
            float q = ar.x;
            float r = ar.y;
            float s = -q - r;

            float rq = round(q);
            float rr = round(r);
            float rs = round(s);

            float q_diff = abs(rq - q);
            float r_diff = abs(rr - r);
            float s_diff = abs(rs - s);

            if (q_diff > r_diff && q_diff > s_diff)
                rq = -rr - rs;
            else if (r_diff > s_diff)
                rr = -rq - rs;

            return float2(rq, rr);
        }

        float2 axial_to_pixel(float2 a, float size)
        {
            float q = a.x;
            float r = a.y;
            float x = size * 3.0/2.0 * q;
            float y = size * sqrt(3.0) * (r + q/2.0);
            return float2(x, y);
        }

        float sdHexagon(float2 p, float radius)
        {
            p = abs(p);
            return max(dot(float2(0.5, sqrt(3)/2.0), p), p.x) - radius;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 p = IN.worldPos.xz;

            // Hex grid math
            float2 axial = pixel_to_axial(p, _TileSize);
            float2 nearestAxial = axial_round(axial);
            float2 center = axial_to_pixel(nearestAxial, _TileSize);

            float2 local = p - center;
            float d = sdHexagon(local, _TileSize * 0.5);

            // Hard border (no blur)
            float border = _BorderSize * _TileSize;
            float tileMask = (d <= -border) ? 1.0 : 0.0;

            float3 color = lerp(_BorderColor.rgb, _TileColor.rgb, tileMask);

            o.Albedo = color;
            o.Metallic = tileMask-_InvertMetallic * _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
