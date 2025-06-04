using UnityEngine;

namespace OccaSoftware.RadialBlur.Runtime
{
    internal static class Params
    {
        public static int center = Shader.PropertyToID("_Center");
        public static int intensity = Shader.PropertyToID("_Intensity");
        public static int delay = Shader.PropertyToID("_Delay");
        public static int sampleCount = Shader.PropertyToID("_SampleCount");
    }
}