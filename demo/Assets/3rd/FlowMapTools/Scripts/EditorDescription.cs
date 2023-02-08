using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.FlowMapTool
{
    public class EditorDescription : MonoBehaviour
    {
        public static readonly string FlowingEditorUsage = "\"Left Mouse Click\" for painting " + Environment.NewLine +
                                                           "Hold \"Ctrl Button\" for erase mode" + Environment.NewLine +
                                                           "Use \"Mouse Wheel\" for brush size";

        public static readonly string LoadLastSave = "是否加载上一次保存的纹理?";
    }
}