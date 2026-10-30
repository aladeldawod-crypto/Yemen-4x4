using System;
using System.IO;
using UnityEngine;
using GLTFast;

namespace Yemen4x4.Vehicles
{
    public sealed class GlbVehicleRuntimeLoader : MonoBehaviour
    {
        [SerializeField] private string fileName = "Player_Sedan.glb";
        [SerializeField, Min(0.5f)] private float targetLength = 4.6f;
        [SerializeField] private Vector3 localRotationEuler;
        [SerializeField] private Vector3 localOffset;
        public void Configure(string file, float length, Vector3 rotation)
        { fileName = file; targetLength = length; localRotationEuler = rotation; }

        private async void Start()
        {
            var uri = new Uri(Path.Combine(Application.streamingAssetsPath, "Vehicles", fileName)).AbsoluteUri;
            var importer = new GltfImport();
            try
            {
                if (!await importer.Load(uri))
                { Debug.LogError("تعذر تحميل نموذج المركبة: " + fileName, this); return; }
                var model = new GameObject("GLB Model - " + fileName);
                model.transform.SetParent(transform, false);
                model.transform.localPosition = localOffset;
                model.transform.localRotation = Quaternion.Euler(localRotationEuler);
                if (!await importer.InstantiateMainSceneAsync(model.transform))
                { Debug.LogError("تعذر إنشاء مجسم المركبة: " + fileName, this); Destroy(model); importer.Dispose(); return; }
                var renderers = model.GetComponentsInChildren<Renderer>(true);
                if (renderers.Length == 0) return;
                var bounds = renderers[0].bounds;
                foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                float longest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                if (longest > .001f) model.transform.localScale = Vector3.one * (targetLength / longest);
                foreach (var collider in model.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
            }
            catch (Exception ex) { Debug.LogException(ex, this); }
        }
    }
}