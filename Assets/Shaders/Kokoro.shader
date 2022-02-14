Shader "ScreenDraw/Kokoro"
{
    Properties
    {
        _K("K", Range(0, 1)) = 0
        _Crimp("Crimp", Range(0, 1)) = 0
        [Space(10)]
        [HDR]_Color("Color", Color) = (1, 0, 0, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 1)
        _Size("Size", Range(5, 20)) = 5
        _Thickness("Thickness", Range(0, 3)) = 1
        [Space(10)]
        _A("A", Range(0, 1)) = 0.66666
        _B("B", Range(0, 15)) = 10
        _C("C", Range(0, 5)) = 1.5
        _D("D", Range(0, 2)) = .5
        _Expansion("Expansion", Range(0, 3)) = 1
        _Curve("Curve", Range(0, 1)) = .2
    }
    SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            float4 _Vertice[4];
            float _Aspect;
            float _K;
            float _Crimp;
            fixed4 _Color;
            fixed4 _BackgroundColor;
            float _Size;
            float _Thickness;
            float _A;
            float _B;
            float _C;
            float _D;
            float _Expansion;
            float _Curve;

            //x^(((2)/(3)))+(10-x^(2))^(0.5) sin(¦Ð x a)
            inline float CalcHeart(float2 uv)
            {
                float y = lerp(uv.y, uv.y + .4, pow(_Crimp, .5));
                uv.y = pow(y, pow(sin(uv.x * 1.57 + 0.785), -_Crimp * 10));

                uv.x = uv.x * _Size * 2 - _Size;
                uv.y = uv.y * _Size * 2 - _Size;
                uv.x *= _Aspect;
                float A = lerp(0, _A, _K);
                float B = lerp(0, _B, _K);
                float C = lerp(.1, _C, _K);
                float D = lerp(0, _D, _K);
                float res = pow(abs(uv.x), A) + pow(B - pow(abs(uv.x), _C), D) * sin(UNITY_PI * uv.x * _K * 5);
                [flatten]
                if (abs(uv.x) >= 4.64 * _K)//isnan(res)
                {
                    float expansion = lerp(_Expansion * sin(_Time.y * .5) * .5, 0, _K);
                    float curve = lerp(0, _Curve, 1 - _Crimp);
                    float lineY = lerp(1, 2.46, _K) + sin(uv.x * expansion + _Time.y) * curve * abs(uv.x);
                    float thickness = _Thickness * lerp(1, 0, _K);
                    res = 1 - step(thickness, distance(float2(uv.x, lineY), uv));
                }
                else
                {
                    res = 1 - step(_Thickness, distance(float2(uv.x, res), uv));
                }
                return res;
            }

            v2f vert (uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
            {
                v2f o;
                o.vertex = _Vertice[vertexID];
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                fixed4 col = _BackgroundColor;
                float res = CalcHeart(uv);
                col = lerp(col, _Color, res);
                return col;
            }
            ENDCG
        }
    }
}