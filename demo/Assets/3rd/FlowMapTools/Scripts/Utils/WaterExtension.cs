using UnityEngine;

namespace czw.FlowMapTool
{
    public static class WaterExtension
    {
        private static float prevRealTime;
        private static float lastDeltaTime;
        private static int renderdFrames;


        private static bool _isEditorTimeUpdated;

        public static void UpdateEditorDeltaTime()
        {
            if (Application.isPlaying)
                return;

            if (_isEditorTimeUpdated)
                return;

            _isEditorTimeUpdated = true;
            lastDeltaTime = Time.realtimeSinceStartup - prevRealTime;
            prevRealTime = Time.realtimeSinceStartup;
        }

        public static void SetEditorDeltaTime()
        {
            _isEditorTimeUpdated = false;
        }
    }
}