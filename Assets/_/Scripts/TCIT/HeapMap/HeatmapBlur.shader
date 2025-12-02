Shader "Custom/HeatmapBlur"
{
    Properties
    {
        _MainTex("Input", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 1.0
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass // X Blur
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include <HLSLSupport.cginc>
            #include <UnityShaderUtilities.cginc>

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy * _BlurSize;

                float v =
                    tex2D(_MainTex, i.uv - texel).r +
                    tex2D(_MainTex, i.uv).r +
                    tex2D(_MainTex, i.uv + texel).r;

                v /= 3;

                return float4(v, v, v, 1);
            }
            ENDHLSL
        }

        Pass // Y Blur
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include <UnityShaderUtilities.cginc>

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 texel = float2(0, _MainTex_TexelSize.y * _BlurSize);

                float v =
                    tex2D(_MainTex, i.uv - texel).r +
                    tex2D(_MainTex, i.uv).r +
                    tex2D(_MainTex, i.uv + texel).r;

                v /= 3;

                return float4(v, v, v, 1);
            }
            ENDHLSL
        }
    }
}
