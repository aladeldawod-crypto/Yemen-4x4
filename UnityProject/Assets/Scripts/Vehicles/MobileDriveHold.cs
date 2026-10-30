using UnityEngine;
using UnityEngine.EventSystems;

namespace Yemen4x4.Vehicles
{
    /// <summary>Continuous touch steering/throttle while a HUD control is held.</summary>
    public sealed class MobileDriveHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private Arcade4x4Controller vehicle;
        private float drive;
        private float turn;

        public void Configure(Arcade4x4Controller controller, float driveValue, float turnValue)
        {
            vehicle = controller;
            drive = Mathf.Clamp(driveValue, -1f, 1f);
            turn = Mathf.Clamp(turnValue, -1f, 1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (vehicle != null) vehicle.SetControls(drive, turn);
        }

        public void OnPointerUp(PointerEventData eventData) => Release();
        public void OnPointerExit(PointerEventData eventData) => Release();

        private void OnDisable() => Release();

        private void Release()
        {
            if (vehicle != null) vehicle.SetControls(0f, 0f);
        }
    }
}