using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;


#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace OccaSoftware.RadialBlur.Runtime
{
    public class RadialBlurPass : ScriptableRenderPass
    {
        private const string profilerTag = "[Radial Blur] Pass";

        private const string radialBlurTargetName = "Radial Blur Target";
        private const string radialBlurShaderId = "OccaSoftware/RadialBlur/Blur";
        private Material radialBlurMaterial = null;

        private static readonly string sourceID = "_Source";

        //private RTHandle source;
        private RTHandle target;

        private RadialBlurPostProcess radialBlur = null;

        public RadialBlurPass()
        {
            target = RTHandles.Alloc(Shader.PropertyToID(radialBlurTargetName), name: radialBlurTargetName);
        }

        internal bool RegisterStackComponent()
        {
            radialBlur = VolumeManager.instance.stack.GetComponent<RadialBlurPostProcess>();

            if (radialBlur == null)
                return false;

            return radialBlur.IsActive();
        }

        internal bool SetupMaterial()
        {
            if (radialBlurMaterial == null)
            {
                Shader shader = Shader.Find(radialBlurShaderId);
                if (shader != null)
                {
                    radialBlurMaterial = CoreUtils.CreateEngineMaterial(radialBlurShaderId);
                }
            }

            return radialBlurMaterial != null;
        }

#if UNITY_2023_3_OR_NEWER
        private class PassData
        {
            internal TextureHandle source;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Setting up the render pass in RenderGraph
            using (var builder = renderGraph.AddUnsafePass<PassData>(profilerTag, out var passData))
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                TextureHandle rtHandle = UniversalRenderer.CreateRenderGraphTexture(
                     renderGraph,
                     ConfigurePass(cameraData.cameraTargetDescriptor),
                     radialBlurTargetName,
                     false
                 );

                passData.source = resourceData.cameraColor;

                builder.UseTexture(passData.source, AccessFlags.ReadWrite);

                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) => ExecutePass(data, context));
            }
        }

        private void ExecutePass(PassData data, UnsafeGraphContext context)
        {
            CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

            ExecutePass(cmd, data.source);
        }
#endif

#if UNITY_2023_3_OR_NEWER
        [Obsolete]
#endif
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigurePass(renderingData.cameraData.cameraTargetDescriptor);
        }

        public RenderTextureDescriptor ConfigurePass(RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor rtDescriptor = cameraTextureDescriptor;
            rtDescriptor.width = Mathf.Max(1, rtDescriptor.width);
            rtDescriptor.height = Mathf.Max(1, rtDescriptor.height);
            rtDescriptor.msaaSamples = 1;
            rtDescriptor.depthBufferBits = 0;
            rtDescriptor.sRGB = false;

            RenderingUtilsHelper.ReAllocateIfNeeded(ref target, rtDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: radialBlurTargetName);
            return rtDescriptor;
        }

#if UNITY_2023_3_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
           
            ExecutePass(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void ExecutePass(CommandBuffer cmd, RTHandle source)
        {
            SetShaderParams();
            cmd.SetGlobalTexture(sourceID, source);
            Blitter.BlitCameraTexture(cmd, source, target, radialBlurMaterial, 0);
            Blitter.BlitCameraTexture(cmd, target, source);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {

        }

        private void SetShaderParams()
        {
            radialBlurMaterial.SetVector(Params.center, radialBlur.GetCenter());
            radialBlurMaterial.SetFloat(Params.intensity, radialBlur.GetIntensity());
            radialBlurMaterial.SetFloat(Params.delay, radialBlur.GetDelay());
            radialBlurMaterial.SetInt(Params.sampleCount, radialBlur.GetSampleCount());
        }
    }
}