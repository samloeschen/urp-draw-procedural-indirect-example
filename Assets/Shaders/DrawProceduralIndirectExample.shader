Shader "Unlit/DrawProceduralIndirectExample" {
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0

            #include "Common.hlsl"
            #include "UnityCG.cginc"

            struct v2f {
                float4 position: SV_POSITION;
                float4 color: TEXCOORD0;
            };

            v2f vert (uint vertexID: SV_VertexID) {
                v2f o;
                Vertex vert = _VertexDataRead[vertexID];
                o.position = UnityObjectToClipPos(vert.position);
                o.color = vert.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }
}