Shader "Custom/TransparentCube"
{
    Properties
    {
        _ColorBottom ("Color Bottom", Color) = (1,1,1,1)
        _ColorTop ("Color Top", Color) = (0,0,0,1)
        _Alpha ("Transparency", Range(0,1)) = 1
        _YScale ("Y Scale", Range(0.1, 10)) = 1
        _NoiseScale ("Noise Scale", Range(1, 50)) = 10
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.2
        _NoiseSpeed ("Noise Speed", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Cull Off // Vẽ cả hai mặt
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            fixed4 _ColorBottom;
            fixed4 _ColorTop;
            float _Alpha;
            float _YScale;
            float _NoiseScale;
            float _NoiseStrength;
            float _NoiseSpeed;

            float2 random2(float2 st)
            {
                st = float2(dot(st,float2(127.1,311.7)),
                           dot(st,float2(269.5,183.3)));
                return -1.0 + 2.0 * frac(sin(st)*43758.5453123);
            }

            float perlinNoise(float2 st) 
            {
                float2 i = floor(st);
                float2 f = frac(st);
                
                float2 u = f*f*(3.0-2.0*f);

                return lerp(
                    lerp(dot(random2(i + float2(0.0,0.0)), f - float2(0.0,0.0)),
                         dot(random2(i + float2(1.0,0.0)), f - float2(1.0,0.0)), u.x),
                    lerp(dot(random2(i + float2(0.0,1.0)), f - float2(0.0,1.0)),
                         dot(random2(i + float2(1.0,1.0)), f - float2(1.0,1.0)), u.x),
                    u.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 movingUV = i.worldPos.xz + _Time.y * _NoiseSpeed;
                float noise = perlinNoise(movingUV * _NoiseScale) * _NoiseStrength;
                
                float normalizedHeight = (i.worldPos.y - unity_ObjectToWorld._m13) / (unity_ObjectToWorld._m11 * _YScale);
                normalizedHeight = saturate(normalizedHeight + noise);
                
                fixed4 col = lerp(_ColorBottom, _ColorTop, normalizedHeight);
                col.a = _Alpha * normalizedHeight;
                return col;
            }
            ENDCG
        }
    }
}
