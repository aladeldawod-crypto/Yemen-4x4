using UnityEngine;
using Yemen4x4.Vehicles;

namespace Yemen4x4.World
{
    public sealed class WorldLifeSpawner : MonoBehaviour
    {
        private Transform player;
        private int trafficVehicleIndex;
        private Material skin,cloth,vehicleMat,animalMat;
        public void Build(Transform playerTransform,Material roadMaterial,Material sandMaterial)
        {
            player=playerTransform;skin=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard"));skin.color=new Color(.48f,.31f,.2f);
            cloth=new Material(skin);cloth.color=new Color(.22f,.3f,.38f);vehicleMat=new Material(roadMaterial);vehicleMat.color=new Color(.12f,.2f,.24f);animalMat=new Material(sandMaterial);animalMat.color=new Color(.48f,.39f,.23f);
            for(int i=0;i<9;i++)CreatePerson(new Vector3(-12+(i%3)*4,.0f,-7+(i/3)*3));
            for(int i=0;i<5;i++)CreateTraffic(new Vector3((i%2==0)?-5:5,.55f,55+i*32));
            for(int i=0;i<5;i++)CreateAnimal(new Vector3(-20+(i%2)*40,.45f,100+i*31));
            CreateFuelStation(new Vector3(-13,0,72));
        }
        private void CreatePerson(Vector3 p)
        {
            var root=new GameObject("Passenger / pedestrian");root.transform.position=p;
            var torso=GameObject.CreatePrimitive(PrimitiveType.Capsule);torso.name="Clothed body";torso.transform.SetParent(root.transform);torso.transform.localPosition=new Vector3(0,.95f,0);torso.transform.localScale=new Vector3(.42f,.72f,.3f);torso.GetComponent<Renderer>().sharedMaterial=cloth;torso.GetComponent<Collider>().enabled=false;
            var head=GameObject.CreatePrimitive(PrimitiveType.Sphere);head.transform.SetParent(root.transform);head.transform.localPosition=new Vector3(0,1.68f,0);head.transform.localScale=Vector3.one*.34f;head.GetComponent<Renderer>().sharedMaterial=skin;head.GetComponent<Collider>().enabled=false;
        }
        private void CreateTraffic(Vector3 p)
        {
            var root = new GameObject("AI traffic GLB vehicle");
            root.transform.position = p;
            bool useTruck = (trafficVehicleIndex++ % 2) == 0;
            var loader = root.AddComponent<GlbVehicleRuntimeLoader>();
            loader.Configure(useTruck ? "Traffic_DumpTruck.glb" : "Traffic_ArmoredVehicle.glb",
                useTruck ? 6.0f : 4.8f, Vector3.zero);
            var mover = root.AddComponent<TrafficMover>();
            mover.Configure(player, 12f + (p.z % 8f), p.z);
        }
        private void CreateAnimal(Vector3 p)
        {
            var root=new GameObject("Yemeni goat / camel roadside animal");root.transform.position=p;
            var body=GameObject.CreatePrimitive(PrimitiveType.Capsule);body.transform.SetParent(root.transform);body.transform.localPosition=new Vector3(0,.65f,0);body.transform.localScale=new Vector3(.48f,.75f,1.1f);body.GetComponent<Renderer>().sharedMaterial=animalMat;body.GetComponent<Collider>().enabled=false;
            for(int i=0;i<4;i++){var leg=GameObject.CreatePrimitive(PrimitiveType.Cylinder);leg.transform.SetParent(root.transform);leg.transform.localPosition=new Vector3(i%2==0?-.25f:.25f,.25f,i<2?-.35f:.35f);leg.transform.localScale=new Vector3(.12f,.5f,.12f);leg.GetComponent<Renderer>().sharedMaterial=animalMat;leg.GetComponent<Collider>().enabled=false;}
        }
        private void CreateFuelStation(Vector3 p)
        {
            var root=new GameObject("Desert fuel station");root.transform.position=p;
            var canopy=GameObject.CreatePrimitive(PrimitiveType.Cube);canopy.transform.SetParent(root.transform);canopy.transform.localPosition=new Vector3(0,3,0);canopy.transform.localScale=new Vector3(9,.35f,6);
            for(int i=-1;i<=1;i+=2){var post=GameObject.CreatePrimitive(PrimitiveType.Cube);post.transform.SetParent(root.transform);post.transform.localPosition=new Vector3(i*3,1.5f,0);post.transform.localScale=new Vector3(.25f,3,.25f);}
            var pump=GameObject.CreatePrimitive(PrimitiveType.Cube);pump.transform.SetParent(root.transform);pump.transform.localPosition=new Vector3(0,.75f,2);pump.transform.localScale=new Vector3(.7f,1.5f,.6f);
            var trigger=root.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.size=new Vector3(10,3,8);root.AddComponent<FuelStationTrigger>();
        }
    }
    public sealed class TrafficMover:MonoBehaviour
    {
        private Transform player;private float speed;private float baseZ;
        public void Configure(Transform target,float velocity,float z){player=target;speed=velocity;baseZ=z;}
        private void Update(){transform.Translate(Vector3.forward*speed*Time.deltaTime,Space.World);if(transform.position.z>baseZ+75)transform.position=new Vector3(transform.position.x,.55f,baseZ-30);if(player&&Vector3.Distance(player.position,transform.position)<3.2f){}}
    }
    public sealed class FuelStationTrigger:MonoBehaviour
    {
        private void OnTriggerEnter(Collider other){var fuel=other.GetComponentInParent<FuelSystem>();if(fuel)fuel.FillTank();}
    }
}