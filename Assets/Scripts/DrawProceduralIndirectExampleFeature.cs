using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Collections.LowLevel.Unsafe;

public class DrawProceduralIndirectExampleFeature : ScriptableRendererFeature {
    public Material material;
    public ComputeShader kernels;
    DrawProceduralIndirectExamplePass _renderPass;

    public override void Create() {
        _renderPass = new DrawProceduralIndirectExamplePass(RenderPassEvent.AfterRenderingOpaques, kernels, material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(_renderPass);
    }
}

public class DrawProceduralIndirectExamplePass : ScriptableRenderPass {
    Material _material;
    ComputeShader _kernels;
    ComputeBuffer _vertexData;
    ComputeBuffer _indirectArgs;
    MaterialPropertyBlock _mpb;

    public DrawProceduralIndirectExamplePass(RenderPassEvent renderPassEvent, ComputeShader kernels, Material material) {
        _material = material;
        _kernels = kernels;
        _vertexData = new ComputeBuffer(3, UnsafeUtility.SizeOf<Vertex>(), ComputeBufferType.Structured);
        _indirectArgs = new ComputeBuffer(4, sizeof(uint), ComputeBufferType.IndirectArguments);
        _mpb = new MaterialPropertyBlock();
        this.renderPassEvent = renderPassEvent;
    }
    
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) { }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        var cmd = CommandBufferPool.Get("DrawProceduralIndirectExample");
        // populatet the mesh data buffer
        cmd.SetComputeBufferParam(_kernels, 0, "_VertexDataRW", _vertexData);
        cmd.DispatchCompute(_kernels, 0, 1, 1, 1);
        
        // populate the indirect args buffer
        cmd.SetComputeBufferParam(_kernels, 1, "_IndirectArgs", _indirectArgs);
        cmd.DispatchCompute(_kernels, 1, 1, 1, 1);
        
        // time to draw
        _mpb.SetBuffer("_VertexDataRead", _vertexData);
        cmd.DrawProceduralIndirect(Matrix4x4.identity, 
                                   _material,
                                   0,
                                   MeshTopology.Triangles,
                                   _indirectArgs,
                                   0,
                                   _mpb);
        
        context.ExecuteCommandBuffer(cmd);
        
        // clean up
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}


// needs to match the GPU-side version
[System.Serializable]
public struct Vertex {
    public Vector3 position;
    public Vector4 color;
}
