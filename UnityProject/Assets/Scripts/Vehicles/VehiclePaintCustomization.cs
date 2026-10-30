using UnityEngine;
using UnityEngine.UI;

namespace Yemen4x4.Vehicles
{
    [System.Serializable]
    public sealed class PaintOption
    {
        public string displayName = "أبيض";
        public Color color = Color.white;
    }

    public sealed class VehiclePaintCustomization : MonoBehaviour
    {
        [SerializeField] private string vehicleId = "hilux_2026";
        [SerializeField] private Renderer[] paintRenderers;
        [SerializeField] private PaintOption[] palette;
        [SerializeField] private Image previewSwatch;
        private int selectedIndex;

        private string SaveKey => "vehicle.paint." + vehicleId;

        private void Start()
        {
            selectedIndex = Mathf.Clamp(PlayerPrefs.GetInt(SaveKey, 0), 0, Mathf.Max(0, palette.Length - 1));
            Apply(selectedIndex);
        }

        public void SelectColor(int index)
        {
            if (palette == null || palette.Length == 0) return;
            selectedIndex = Mathf.Clamp(index, 0, palette.Length - 1);
            Apply(selectedIndex);
        }

        public void ConfirmColor()
        {
            PlayerPrefs.SetInt(SaveKey, selectedIndex);
            PlayerPrefs.Save();
        }

        public void PreviewNext() => SelectColor((selectedIndex + 1) % Mathf.Max(1, palette.Length));
        public void PreviewPrevious() => SelectColor((selectedIndex - 1 + Mathf.Max(1, palette.Length)) % Mathf.Max(1, palette.Length));

        private void Apply(int index)
        {
            if (palette == null || palette.Length == 0) return;
            Color color = palette[index].color;
            foreach (Renderer target in paintRenderers)
            {
                if (!target) continue;
                // MaterialPropertyBlock avoids creating a material instance per vehicle.
                var block = new MaterialPropertyBlock();
                target.GetPropertyBlock(block);
                block.SetColor("_BaseColor", color);
                block.SetColor("_Color", color);
                target.SetPropertyBlock(block);
            }
            if (previewSwatch) previewSwatch.color = color;
        }
    }
}
