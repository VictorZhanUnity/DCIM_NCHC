Shader "Custom/HeatmapRenderer2D_GaussianEdges_Simplex"
{
    Properties
    {
        _PointCount ("Point Count", Int) = 0
        _Radius ("Base Radius", Float) = 5.0
        _MaxIntensity ("MaxIntensity", Float) = 1.0
        _GradientTex ("Gradient Texture", 2D) = "white" {}

        // Noise Controls
        _NoiseStrength ("Noise Strength", Float) = 0.15
        _NoiseSpeed ("Noise Speed", Float) = 0.3
        _NoiseScale ("Noise Scale", Float) = 1.0

        [Toggle] _UseDynamic ("Use Noise Dynamics", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define MAX_POINTS 256
            #include <UnityShaderUtilities.cginc>

            int _PointCount;
            float _Radius;
            float _MaxIntensity;

            float _NoiseStrength;
            float _NoiseSpeed;
            float _NoiseScale;
            float _UseDynamic;

            float4 _Points[MAX_POINTS];   // xy = pos, z = intensity, w = phase

            sampler2D _GradientTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            //------------------------------------------------------------------
            // Simplex Noise 2D
            // 超精簡快速版
            //------------------------------------------------------------------

            float2 mod289(float2 x) { return x - floor(x / 289.0) * 289.0; }
            float3 mod289_3(float3 x) { return x - floor(x / 289.0) * 289.0; }

            float3 permute(float3 x)  { return mod289_3((x * 34.0 + 1.0) * x); }

            float snoise(float2 v)
            {
                const float F2 = 0.36602540378; // 0.5*(sqrt(3.0)-1.0)
                const float G2 = 0.2113248654;  // (3.0-sqrt(3.0))/6.0

                float2 i = floor(v + dot(v, float2(F2, F2)));
                float2 x0 = v - i + dot(i, float2(G2, G2));

                float2 i1;
                i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);

                float2 x1 = x0 - i1 + float2(G2, G2);
                float2 x2 = x0 - 1.0 + 2.0 * float2(G2, G2);

                i = mod289(i);
                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0))
                                 +    i.x + float3(0.0, i1.x, 1.0));

                float3 m = max(0.5 - float3(dot(x0,x0), dot(x1,x1), dot(x2,x2)), 0.0);
                m = m * m;
                m = m * m;

                float3 x = float3(dot(x0, float2(0.7453, 0.6666)),
                                  dot(x1, float2(0.7453, 0.6666)),
                                  dot(x2, float2(0.7453, 0.6666)));

                return dot(m, x) * 35.0;
            }

            //------------------------------------------------------------------

            float4 frag(v2f i) : SV_Target
            {
                float heatValue = 0.0;

                for (int k = 0; k < _PointCount; k++)
                {
                    float2 sensorPos = _Points[k].xy;
                    float intensity = clamp(_Points[k].z, 0.0, _MaxIntensity);
                    float phase = _Points[k].w;

                    float dist = distance(i.worldPos.xz, sensorPos);

                    float sigma = _Radius * 0.5;

                    if (_UseDynamic > 0.5)
                    {
                        // time-based noise coord
                        float2 noiseUV = float2(
                            sensorPos.x * _NoiseScale + phase * 5.17,
                            sensorPos.y * _NoiseScale + _Time.y * _NoiseSpeed
                        );

                        float n = snoise(noiseUV);   // -1 ~ 1
                        n = (n * 0.5 + 0.5);         // normalize → 0 ~ 1

                        float dynamicFactor = 1.0 + (n - 0.5) * 2.0 * _NoiseStrength;
                        sigma *= dynamicFactor;
                    }

                    float g = exp(-(dist * dist) / (2.0 * sigma * sigma));
                    heatValue += intensity * g;
                }

                float heatNorm = saturate(heatValue / _MaxIntensity);

                float alpha = pow(heatNorm, 1.2);
                if (alpha <= 0.001)
                    return float4(0, 0, 0, 0);

                float4 col = tex2D(_GradientTex, float2(heatNorm, 0.5));
                col.a = alpha;

                return col;
            }

            ENDHLSL
        }
    }
}
