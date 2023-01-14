using System;
using System.Collections.Generic;
using UnityEngine;

namespace Render.AfterImagPool
{
    // 管理多个Skin Mesh
    public class AfterImag : MonoBehaviour
    {
        private SkinnedMeshRenderer[] skinRender;
        private Dictionary<int, AfterImagItem> itemDic = new Dictionary<int, AfterImagItem>();

        public void Init(int id, Material material, Shader shader)
        {
            var item = CreatAfterImagItem();
            item.Init(material, shader);
            itemDic.Add(id, item);
        }

        private AfterImagItem CreatAfterImagItem()
        {
            GameObject go = new GameObject("AfterImagItem");
            var item = go.AddComponent<AfterImagItem>();
            go.transform.SetParent(this.transform);
            return item;
        }

        public void UpdateMesh(int id, Mesh mesh, Vector3 pos, Quaternion rot, Action complete)
        {
            if (itemDic.ContainsKey(id))
            {
                itemDic[id].UpdateMesh(mesh, pos, rot, complete);
            }
        }

        public void SetActive(bool enable)
        {
            this.gameObject.SetActive(enable);
        }
    }
}