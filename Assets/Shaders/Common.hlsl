struct Vertex {
    float3 position;
    float4 color;
};
RWStructuredBuffer<Vertex> _VertexDataRW;
StructuredBuffer<Vertex> _VertexDataRead;
