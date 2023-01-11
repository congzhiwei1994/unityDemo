using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.GostShadow
{
    /// <summary>
    /// 管理 GostShadowItem
    /// </summary>
    public class GostShadow : MonoBehaviour
    {
        private Dictionary<int, GostShadowItem> itemDic = new Dictionary<int, GostShadowItem>();

        public void Init(int id, Material material, Shader shader, Action<GostShadow> complate)
        {
            GameObject go = new GameObject("GostShadowItem");
            var item = go.AddComponent<GostShadowItem>();
            go.transform.SetParent(this.transform);
            itemDic.Add(id, item);
            
            item.Init(material, shader, () => complate(this));
        }

        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public void UpadeMesh(int id, Mesh mesh, Vector3 pos, Quaternion rot)
        {
            if (!itemDic.ContainsKey(id))
            {
                Debug.LogError("当前ID不存在,id" + id);
                return;
            }

            itemDic[id].UpadeMesh(mesh, pos, rot);
        }
    }
}