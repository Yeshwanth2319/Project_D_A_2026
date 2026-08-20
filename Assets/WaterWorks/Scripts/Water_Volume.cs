using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;

        private Material material;

        // RTHandle instead of RenderTargetHandle
        private RTHandle renderTarget;

        public CustomRenderPass(Material mat)
        {
            material = mat;

            // Allocate RTHandle with shader property
            renderTarget = RTHandles.Alloc(
                "_TemporaryColourTexture",
                name: "_TemporaryColourTexture"
            );
        }

        public override void Configure(
            CommandBuffer cmd,
            RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Allocate temporary RT using shader property ID
            cmd.GetTemporaryRT(
                Shader.PropertyToID(renderTarget.name),
                cameraTextureDescriptor,
                FilterMode.Bilinear
            );

            // Configure target using RTHandle
            ConfigureTarget(renderTarget);
        }

        public override void Execute(
            ScriptableRenderContext context,
            ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Water Volume");

            // Source -> Temp
            Blit(cmd, source, renderTarget, material);

            // Temp -> Source
            Blit(cmd, renderTarget, source);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                return;

            // Release temporary RT using shader property ID
            cmd.ReleaseTemporaryRT(
                Shader.PropertyToID(renderTarget.name)
            );
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass =
            RenderPassEvent.AfterRenderingSkybox;
    }

    public Settings settings = new Settings();

    private CustomRenderPass scriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material =
                (Material)Resources.Load("Water_Volume");
        }

        scriptablePass = new CustomRenderPass(settings.material);
        scriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        // New URP API
        scriptablePass.source = renderer.cameraColorTargetHandle;

        renderer.EnqueuePass(scriptablePass);
    }
}