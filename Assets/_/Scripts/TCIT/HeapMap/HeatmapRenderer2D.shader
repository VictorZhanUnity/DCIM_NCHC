
Shader "Custom/HeatmapRenderer2D"
{
    Properties
    {
        _PointCount ("Point Count", Int) = 0
        _Radius ("Radius", Float) = 5
        _MaxIntensity ("MaxIntensity", Float) = 1
        _GradientTex ("Gradient Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off // 通常透明物件要關
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define MAX_POINTS 32
            #include <UnityShaderUtilities.cginc>

            int _PointCount;
            float _Radius;
            float _MaxIntensity;
            float4 _Points[MAX_POINTS]; // xy: pos, z: intensity(0-1), w: unused

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
                float heatValue = 0;
                float minDist = 999999.0;

                for (int k = 0; k < _PointCount; k++)
                {
                    float2 sensorPos = _Points[k].xy;
                    float intensity = clamp(_Points[k].z, 0, _MaxIntensity);

                    float dist = distance(i.worldPos.xz, sensorPos);
                    minDist = min(minDist, dist);

                    float sigma = _Radius / 3.0;
                    heatValue += intensity * exp(-dist * dist / (2 * sigma * sigma));
                }

                // 半徑外透明
                if (minDist > _Radius)
                    return float4(0, 0, 0, 0);

                float heatNorm = saturate(heatValue / _MaxIntensity);
                float4 col = tex2D(_GradientTex, float2(heatNorm, 0.5));

                // 確保漸層色是完全不透明
                col.a = 1.0;
                return col;
            }
            ENDHLSL
        }
    }
}