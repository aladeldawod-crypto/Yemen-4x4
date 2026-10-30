using UnityEngine;

namespace Yemen4x4.World
{
    public sealed class DayNightWeather : MonoBehaviour
    {
        [SerializeField] private Light sun; [SerializeField] private Light[] headlights;
        [SerializeField] private float dayLengthSeconds=480f;
        private ParticleSystem rain; private float clock=.28f; private bool raining;
        public void Initialize(Light directional,Transform vehicle)
        {
            sun=directional;
            if(vehicle){headlights=new Light[2];for(int i=0;i<2;i++){var g=new GameObject("Vehicle headlight "+i);g.transform.SetParent(vehicle,false);g.transform.localPosition=new Vector3(i==0?-.72f:.72f,.25f,2.05f);g.transform.localRotation=Quaternion.Euler(4,0,0);var l=g.AddComponent<Light>();l.type=LightType.Spot;l.range=48;l.spotAngle=62;l.intensity=4.5f;headlights[i]=l;}}
            var rainObject=new GameObject("Rain weather particles");rainObject.transform.SetParent(transform,false);rain=rainObject.AddComponent<ParticleSystem>();
            var main=rain.main;main.startLifetime=1.4f;main.startSpeed=24f;main.startSize=.045f;main.maxParticles=3500;main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=rain.emission;emission.rateOverTime=0;var shape=rain.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(35,1,35);
        }
        private void Update()
        {
            clock=(clock+Time.deltaTime/dayLengthSeconds)%1f;float angle=clock*360f-90f;
            if(sun){sun.transform.rotation=Quaternion.Euler(angle,35,0);float daylight=Mathf.Clamp01(Mathf.Sin(clock*Mathf.PI*2f)*.5f+.5f);sun.intensity=Mathf.Lerp(.08f,1.25f,daylight);RenderSettings.ambientIntensity=Mathf.Lerp(.18f,1f,daylight);}
            bool night=clock<.22f||clock>.78f;if(headlights!=null)foreach(var l in headlights)if(l)l.enabled=night;
        }
        public void SetRain(bool enabled)
        {
            raining=enabled;if(!rain)return;var emission=rain.emission;emission.rateOverTime=raining?1100f:0f;
            if(raining&&!rain.isPlaying)rain.Play();if(!raining&&rain.isPlaying)rain.Stop();
        }
        public void ToggleRain()=>SetRain(!raining);
    }
}