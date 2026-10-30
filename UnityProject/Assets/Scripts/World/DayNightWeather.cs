using UnityEngine;

namespace Yemen4x4.World
{
    public sealed class DayNightWeather : MonoBehaviour
    {
        [SerializeField] private Light sun;
        [SerializeField] private Light[] headlights;
        [SerializeField] private Material skyMaterial;
        public void SetMorning(){if(sun){sun.intensity=1.2f;sun.color=new Color(1f,.92f,.78f);}RenderSettings.fogDensity=.0025f;ToggleLights(false);}
        public void SetNight(){if(sun)sun.intensity=.08f;RenderSettings.fogDensity=.006f;ToggleLights(true);}
        public void SetRain(){RenderSettings.fogDensity=.012f;}
        private void ToggleLights(bool on){foreach(var l in headlights)if(l)l.enabled=on;}
    }
}
