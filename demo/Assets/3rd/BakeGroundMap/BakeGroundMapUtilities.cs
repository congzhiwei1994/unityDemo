using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;

namespace Tools.BakeGroundMap
{
    public class BakeGroundMapUtilities
    {
        private BakeGroundMapMono mono;
        private BakeGroundMapAsset asset;
        private float originalTerrainHeight;
        private Scene scene;
        private RenderTexture rt;
        private Texture2D texture;

        public void Bake(BakeGroundMapMono mono, BakeGroundMapAsset asset, Scene scene)
        {
            this.mono = mono;
            this.asset = asset;
            this.scene = scene;
            Setup();
            Restore();
        }

        public void Setup()
        {
            SetupCamera();
            SetupRenderTexture();
        }

        public void Restore()
        {
            RenderTexture.DestroyImmediate(rt);
            GameObject.DestroyImmediate(mono.renderCam.gameObject);
            mono.renderCam = null;
        }


        public BakeGroundMapAsset GetAsset(Scene scene)
        {
            var folder = GetAssetFolder(scene);
            var path = folder + "/" + scene.name + ".asset";
            var _asset = (BakeGroundMapAsset)AssetDatabase.LoadAssetAtPath(path, typeof(BakeGroundMapAsset));
            if (_asset == null)
                _asset = BakeGroundMapAsset.CreatNewAsset(folder, scene.name);
            return _asset;
        }

        private string GetAssetFolder(Scene scene)
        {
            var folder = Path.GetDirectoryName(scene.path);
            folder = folder.Replace('\\', '/');
            folder = folder + "/" + BakeGroundMapData.ASSET_FOLDER;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        public List<GameObject> GetGrounds(GameObject go)
        {
            if (go == null)
                return null;
            List<GameObject> grounds = new List<GameObject>();

            var childCound = go.transform.childCount;
            for (int i = 0; i < childCound; i++)
            {
                var child = go.transform.GetChild(i);
                grounds.Add(child.gameObject);
            }

            return grounds;
        }

        public BakeGroundMapMono GetMono(GameObject parent)
        {
            var mono = parent.GetComponent<BakeGroundMapMono>();
            if (mono == null)
                mono = parent.AddComponent<BakeGroundMapMono>();
            return mono;
        }

        public Vector4 BoundsToUV(Bounds b)
        {
            Vector4 uv = new Vector4();

            //Origin position
            uv.x = b.min.x;
            uv.y = b.min.z;
            //Scale factor
            uv.z = 1f / b.size.x;
            uv.w = 0f;
            return uv;
        }

        public Bounds GetTerrainBounds(List<GameObject> terrainObjects)
        {
            Vector3 minSum = Vector3.one * 4096;
            Vector3 maxSum = Vector3.zero;

            foreach (GameObject item in terrainObjects)
            {
                if (item == null)
                    continue;

                Terrain t = item.GetComponent<Terrain>();
                MeshRenderer r = t ? null : item.GetComponent<MeshRenderer>();

                if (t != null)
                {
                    var pos = t.transform.position;
                    //Min/max bounds corners in world-space
                    Vector3 min = pos;
                    //Note, size is slightly more correct in height than bounds
                    Vector3 max = pos + t.terrainData.size;

                    if (min.x < minSum.x || min.y < minSum.y || min.z < minSum.z)
                        minSum = min;
                    if (max.x >= maxSum.x || max.y >= maxSum.y || max.z >= maxSum.z)
                        maxSum = max;
                }

                if (r != null)
                {
                    //World-space bounds corners
                    Vector3 min = r.bounds.min;
                    Vector3 max = r.bounds.max;

                    if (max.x > maxSum.x || max.y > maxSum.y || max.z > maxSum.z)
                        maxSum = max;
                    if (min.x < minSum.x || min.y < minSum.y || min.z < minSum.z)
                        minSum = min;
                }
            }

            Bounds b = new Bounds(Vector3.zero, Vector3.zero);

            b.SetMinMax(minSum, maxSum);

            //Increase bounds height for flat terrains
            if (b.size.y < 2f)
            {
                b.Encapsulate(new Vector3(b.center.x, b.center.y + 1f, b.center.z));
                b.Encapsulate(new Vector3(b.center.x, b.center.y - 1f, b.center.z));
            }

            return b;
        }


        private void SetupRenderTexture()
        {
            var resolution = asset.resolution;
            var rtFormat = RenderTextureFormat.ARGB32;
            rt = new RenderTexture(resolution, resolution, 0, rtFormat, RenderTextureReadWrite.sRGB);

            mono.renderCam.targetTexture = rt;
            RenderTexture.active = rt;
            mono.renderCam.Render();
            Graphics.SetRenderTarget(rt);
            texture = CreaTexture(resolution, TextureFormat.ARGB32);
            mono.groundTex = texture;
            SaveTexture(texture);
        }


        private Texture2D CreaTexture(int texSize, TextureFormat format)
        {
            Texture2D texture = new Texture2D(texSize, texSize, format, true, false);
            texture.ReadPixels(new Rect(0, 0, texSize, texSize), 0, 0);
            texture.Apply();
            // texture.Compress(false);
            texture.name = SetName();
            return texture;
        }

        private void SetupCamera()
        {
            if (mono.renderCam == null)
                mono.renderCam = new GameObject().AddComponent<Camera>();

            mono.renderCam.name = "Bake Ground renderCam";
            mono.renderCam.enabled = false;

            //Camera set up
            mono.renderCam.orthographic = true;
            mono.renderCam.orthographicSize = (mono.bounds.size.x / 2);
            mono.renderCam.farClipPlane = mono.bounds.size.y + BakeGroundMapData.CLIP_PADDING;
            mono.renderCam.clearFlags = CameraClearFlags.Color;
            mono.renderCam.backgroundColor = Color.red;
            mono.renderCam.cullingMask = asset.useLayers ? (int)asset.layerMask : -1;

            var x = asset.bounds.center.x;
            var y = asset.bounds.center.y + asset.bounds.extents.y + BakeGroundMapData.CLIP_PADDING +
                    (asset.useLayers ? 0f : BakeGroundMapData.HEIGHT_OFFSET);
            var z = asset.bounds.center.z;
            mono.renderCam.transform.position = new Vector3(x, y, z);
        }

        private string SetName()
        {
            string name = null;
            if (scene.name == "")
                name = "Untitled";

            return BakeGroundMapData.GRASS_MAP_NAME + "_" + name + "_" + asset.resolution;
        }

        private void SaveTexture(Texture2D texture)
        {
            byte[] bytes = new byte[] { };
            bytes = texture.EncodeToPNG();
            if (bytes != null)
            {
                var path = GetTextureSavePath();
                System.IO.File.WriteAllBytes(path, bytes);
                Debug.Log("Capture Image Finish-->" + path);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }

        private string GetTextureSavePath()
        {
            var folder = Path.GetDirectoryName(scene.path);
            folder = folder.Replace('\\', '/');

            var fullPath = folder + "/" + asset.texture.name + ".png";
            return fullPath;
        }
    }
}