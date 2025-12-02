Shader "Custom/CenterToEdgeColor"
{
    Properties
    {
        _MaxDistance ("Max Distance", Float) = 1
        _ColorCenter ("Center Color", Color) = (1,0,0,1)
        _ColorEdge ("Edge Color", Color) = (0,1,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include <UnityShaderUtilities.cginc>

            float _MaxDistance;
            float4 _ColorCenter;
            float4 _ColorEdge;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz; // local space
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float dist = length(i.localPos);
                float t = saturate(dist / _MaxDistance);
                return lerp(_ColorCenter, _ColorEdge, t);
            }

            ENDHLSL
        }
    }
}
