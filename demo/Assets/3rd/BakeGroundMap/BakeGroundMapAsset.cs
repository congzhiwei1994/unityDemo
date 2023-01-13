using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine.Serialization;


namespace Tools.BakeGroundMap
{
    public enum TexSize
    {
        _4096 = 4096,
        _2048 = 2048,
        _1024 = 1024,
        _720 = 720,
        _512 = 512,
        _256 = 256,
    }

    [Serializable]
    public class BakeGroundMapAsset : ScriptableObject
    {
        [SerializeField] public List<GameObject> terrainObjects;
        [SerializeField] public Bounds bounds;
        [SerializeField] public Vector4 uv;
        [SerializeField] public bool useLayers;
        [SerializeField] public LayerMask layerMask;
        [SerializeField] public int resolution;
        [SerializeField] public Texture2D texture;
        
        
        public void SetLayerMask(int id)
        {
            if (useLayers)
                layerMask = ~(1 << id);
            else
                layerMask = -1;
        }

        public static BakeGroundMapAsset CreatNewAsset(string assetPath, string sceneName)
        {
            BakeGroundMapAsset asset = ScriptableObject.CreateInstance<BakeGroundMapAsset>();
            if (!EditorUtility.IsPersistent(asset))
            {
                AssetDatabase.CreateAsset(asset, assetPath + "/" + sceneName + ".asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        public void SetGrounds(List<GameObject> gos)
        {
            for (int i = 0; i < gos.Count; i++)
            {
                if (!terrainObjects.Contains(gos[i]))
                {
                    terrainObjects.Add(gos[i]);
                }
            }
        }
    }
}