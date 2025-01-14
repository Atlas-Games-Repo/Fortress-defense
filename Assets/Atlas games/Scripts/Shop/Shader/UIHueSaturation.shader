Shader "Custom/HueSaturation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Hue ("Hue Shift", Range(0, 1)) = 0
        _Saturation ("Saturation", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Hue;
            float _Saturation;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hueToRGB(float p, float q, float t)
            {
                if (t < 0.0) t += 1.0;
                if (t > 1.0) t -= 1.0;
                if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
                if (t < 1.0 / 2.0) return q;
                if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
                return p;
            }

            float3 RGBToHSL(float3 color)
            {
                float maxColor = max(color.r, max(color.g, color.b));
                float minColor = min(color.r, min(color.g, color.b));
                float h = 0;
                float s = 0;
                float l = (maxColor + minColor) / 2.0;

                if (maxColor != minColor)
                {
                    float d = maxColor - minColor;
                    s = (l > 0.5) ? d / (2.0 - maxColor - minColor) : d / (maxColor + minColor);

                    if (maxColor == color.r)
                        h = (color.g - color.b) / d + (color.g < color.b ? 6.0 : 0.0);
                    else if (maxColor == color.g)
                        h = (color.b - color.r) / d + 2.0;
                    else
                        h = (color.r - color.g) / d + 4.0;

                    h /= 6.0;
                }
                return float3(h, s, l);
            }

            float3 HSLToRGB(float3 hsl)
            {
                float r, g, b;

                if (hsl.y == 0)
                {
                    r = g = b = hsl.z;
                }
                else
                {
                    float q = (hsl.z < 0.5) ? (hsl.z * (1.0 + hsl.y)) : (hsl.z + hsl.y - hsl.z * hsl.y);
                    float p = 2.0 * hsl.z - q;
                    r = hueToRGB(p, q, hsl.x + 1.0 / 3.0);
                    g = hueToRGB(p, q, hsl.x);
                    b = hueToRGB(p, q, hsl.x - 1.0 / 3.0);
                }
                return float3(r, g, b);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                float3 hsl = RGBToHSL(texColor.rgb);
                hsl.x += _Hue;
                hsl.y *= _Saturation;
                texColor.rgb = HSLToRGB(hsl);
                return texColor;
            }
            ENDCG
        }
    }
}
