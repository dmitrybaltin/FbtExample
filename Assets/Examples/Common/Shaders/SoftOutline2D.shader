Shader "Custom/SoftOutline2D"
{
    Properties
    {
        _Color("Fill Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _OutlineColor;
            float _OutlineThickness;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // URP 2D: правильно преобразуем в clip space
                //OUT.positionCS = UnityObjectToClipPos(IN.positionOS); 

                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Центр UV для кружка
                float2 center = float2(0.5,0.5);
                float dist = distance(IN.uv, center);

                // Обводка
                float outline = smoothstep(0.5, 0.5 - _OutlineThickness, dist);

                // Цвет: обводка → заливка
                float4 color = lerp(_OutlineColor, _Color, outline);
                return color;
            }

            ENDHLSL
        }
    }
}
