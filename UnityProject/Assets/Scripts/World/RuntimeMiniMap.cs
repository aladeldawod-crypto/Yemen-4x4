using UnityEngine;
using UnityEngine.UI;

namespace Yemen4x4.World
{
    /// <summary>Runtime top-down minimap with player marker and destination pin.</summary>
    public sealed class RuntimeMiniMap : MonoBehaviour
    {
        private Camera mapCamera;
        private RenderTexture texture;
        private Transform player;
        private Transform target;
        private RectTransform arrow;
        public void Initialize(Transform playerTransform, Transform destination, Transform uiParent)
        {
            player = playerTransform; target = destination;
            texture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
            texture.name = "Yemen4x4_Minimap_RT";
            var cameraObject = new GameObject("Minimap Camera");
            mapCamera = cameraObject.AddComponent<Camera>();
            mapCamera.orthographic = true; mapCamera.orthographicSize = 42f;
            mapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            mapCamera.clearFlags = CameraClearFlags.SolidColor;
            mapCamera.backgroundColor = new Color(.12f,.16f,.13f);
            mapCamera.cullingMask = ~0; mapCamera.targetTexture = texture;
            var panel = new GameObject("Minimap Panel", typeof(RectTransform), typeof(RawImage), typeof(Outline));
            panel.transform.SetParent(uiParent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(.82f,.70f); rect.anchorMax = new Vector2(.98f,.96f);
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            panel.GetComponent<RawImage>().texture = texture;
            panel.GetComponent<Outline>().effectColor = Color.white;
            var pin = new GameObject("Player map marker", typeof(RectTransform), typeof(Text));
            pin.transform.SetParent(panel.transform,false);
            var pr = pin.GetComponent<RectTransform>(); pr.anchorMin=pr.anchorMax=new Vector2(.5f,.5f); pr.sizeDelta=new Vector2(50,50);
            var text = pin.GetComponent<Text>(); text.text="▲"; text.alignment=TextAnchor.MiddleCenter; text.font=Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize=36; text.color=Color.yellow;
            arrow = pr;
        }
        private void LateUpdate()
        {
            if (!mapCamera || !player) return;
            Vector3 p=player.position; mapCamera.transform.position=new Vector3(p.x, p.y+95f, p.z);
            mapCamera.transform.rotation=Quaternion.Euler(90f, player.eulerAngles.y, 0f);
            if (arrow) arrow.localRotation=Quaternion.Euler(0,0,-player.eulerAngles.y);
        }
        public void SetDestination(Transform destination) { target=destination; }
        private void OnDestroy(){if(mapCamera)Destroy(mapCamera.gameObject);if(texture){texture.Release();Destroy(texture);}}
    }
}