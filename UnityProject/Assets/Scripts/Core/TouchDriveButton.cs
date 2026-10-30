using UnityEngine;
using UnityEngine.EventSystems;
using Yemen4x4.Vehicles;

namespace Yemen4x4.Core
{
    public sealed class TouchDriveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Arcade4x4Controller target;
        public float throttle;
        public float steering;
        public void OnPointerDown(PointerEventData eventData){if(target)target.SetControls(throttle,steering);}
        public void OnPointerUp(PointerEventData eventData){if(target)target.SetControls(0,0);}
        public void OnPointerExit(PointerEventData eventData){if(target)target.SetControls(0,0);}
    }
}
