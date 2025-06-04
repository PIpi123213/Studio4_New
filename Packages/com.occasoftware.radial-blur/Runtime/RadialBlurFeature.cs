using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.RadialBlur.Runtime
{
    public class RadialBlurFeature : ScriptableRendererFeature
    {
        RadialBlurPass radialBlurPass;

        public override void Create()
        {
            radialBlurPass = new RadialBlurPass();
            radialBlurPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.Reflection)
                return;

            if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                return;

            if (!renderingData.cameraData.postProcessEnabled)
                return;

            if (!radialBlurPass.RegisterStackComponent())
                return;

            if (!radialBlurPass.SetupMaterial())
                return;

            renderer.EnqueuePass(radialBlurPass);
        }


        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            radialBlurPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
            //radialBlurPass.SetTarget(renderer.cameraColorTargetHandle);
        }
    }
}
