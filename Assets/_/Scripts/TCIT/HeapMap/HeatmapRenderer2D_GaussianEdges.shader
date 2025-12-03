Shader "Custom/HeatmapRenderer2D_GaussianEdges"
{
    Properties
    {
        _PointCount ("Point Count", Int) = 0
        _Radius ("Radius", Float) = 5.0
        _MaxIntensity ("MaxIntensity", Float) = 1.0
        _GradientTex ("Gradient Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
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
            float4 _Points[MAX_POINTS]; // xy: pos (world xz), z: intensity (0..1)

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

            float4 frag(v2f i) : SV_Target
            {
                float heatValue = 0.0;

                float sigma = _Radius * 0.5; // Gaussian 寬度

                for (int k = 0; k < _PointCount; k++)
                {
                    float2 sensorPos = _Points[k].xy;
                    float intensity = clamp(_Points[k].z, 0.0, _MaxIntensity);

                    float dist = distance(i.worldPos.xz, sensorPos);

                    // Gaussian without any hard cut
                    float g = exp(-(dist * dist) / (2.0 * sigma * sigma));

                    heatValue += intensity * g;
                }

                float heatNorm = saturate(heatValue / _MaxIntensity);

                // heat 就是 alpha
                float alpha = heatNorm;
                alpha = pow(alpha, 1.2);
                
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