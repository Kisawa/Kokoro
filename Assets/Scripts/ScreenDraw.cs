using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenDraw : ScriptableRendererFeature
{
    public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
    public Material UseMat;

    class ScreenDrawRenderPass : ScriptableRenderPass
    {
        static readonly string RenderTag = "ScreenDraw";
        static readonly int _Vertice = Shader.PropertyToID("_Vertice");
        static readonly int _Aspect = Shader.PropertyToID("_Aspect");

        Material mat;

        public ScreenDrawRenderPass(Material mat)
        {
            this.mat = mat;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (mat == null)
                return;
            Vector4[] vertices = new Vector4[]
                {
                    new Vector4(1, 1, 0, .5f),
                    new Vector4(-1, 1, 0, .5f),
                    new Vector4(-1, -1, 0, .5f),
                    new Vector4(1, -1, 0, .5f)
                };
            mat.SetVectorArray(_Vertice, vertices);
            mat.SetFloat(_Aspect, renderingData.cameraData.camera.aspect);
            CommandBuffer cmd = CommandBufferPool.Get(RenderTag);
            cmd.DrawProcedural(Matrix4x4.identity, mat, 0, MeshTopology.Quads, 4);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    ScreenDrawRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new ScreenDrawRenderPass(UseMat);
        m_ScriptablePass.renderPassEvent = Event;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


