Shader "Custom/HeatmapColor"
{
    Properties
    {
        _MainTex("Heat Input", 2D) = "white" {}
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

            sampler2D _MainTex;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 HeatColor(float t)
            {
                float3 c =
                    lerp(float3(0,0,1), float3(0,1,0), saturate(t*2));          // Blue → Green
                c = lerp(c, float3(1,0,0), saturate(t*2 - 1));                  // Green → Red
                return float4(c, 0.8);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float v = tex2D(_MainTex, i.uv).r;
                return HeatColor(v);
            }
            ENDHLSL
        }
    }
}
