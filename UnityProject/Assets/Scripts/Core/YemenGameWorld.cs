using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yemen4x4.Missions;
using Yemen4x4.Vehicles;

namespace Yemen4x4.Core
{
    public sealed class YemenGameWorld : MonoBehaviour
    {
        private Material asphalt, sand, dirt, mud, stone, plaster, trim, blue, glass, rubber, paint;
        private MissionFlow mission; private FuelSystem fuel; private Arcade4x4Controller car;
        private VehiclePaintCustomization paintSystem;
        private Transform carRoot, cameraPivot;
        private Text status, fuelText, routeText; private Slider fuelBar;
        private readonly List<GameObject> routeMarkers = new List<GameObject>();

        private void Start(){CreateMaterials();BuildDepot();BuildRoadWorld();BuildVehicle();BuildCamera();BuildUI();mission=gameObject.AddComponent<MissionFlow>();SetStatus("اختر وجهتك من إحدى البوابات");}
        private Material Mat(Color c,float metallic=0f,float smooth=.25f){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard"));m.color=c;m.SetFloat("_Metallic",metallic);m.SetFloat("_Smoothness",smooth);return m;}
        private void CreateMaterials(){asphalt=Mat(new Color(.16f,.17f,.18f));sand=Mat(new Color(.67f,.48f,.27f));dirt=Mat(new Color(.38f,.25f,.15f));mud=Mat(new Color(.19f,.14f,.10f));stone=Mat(new Color(.38f,.36f,.32f));plaster=Mat(new Color(.78f,.69f,.53f));trim=Mat(new Color(.30f,.19f,.12f));blue=Mat(new Color(.05f,.55f,1f),.1f,.65f);glass=Mat(new Color(.08f,.18f,.22f),.25f,.8f);rubber=Mat(new Color(.025f,.025f,.025f));paint=Mat(new Color(.72f,.08f,.045f),.35f,.55f);}
        private GameObject Cube(string n,Vector3 p,Vector3 s,Material m,Transform parent=null){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(parent);g.transform.position=p;g.transform.localScale=s;if(m)g.GetComponent<Renderer>().sharedMaterial=m;return g;}
        private GameObject Sphere(string n,Vector3 p,Vector3 s,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=n;g.transform.position=p;g.transform.localScale=s;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
        private void BuildDepot()
        {
            RenderSettings.ambientLight=new Color(.62f,.59f,.53f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.72f,.66f,.56f);RenderSettings.fogDensity=.0035f;
            var sun=new GameObject("Sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.25f;sun.transform.rotation=Quaternion.Euler(48,-32,0);
            Cube("Sanaa depot ground",new Vector3(0,-.35f,0),new Vector3(90,.6f,70),dirt);Cube("Depot asphalt apron",new Vector3(0,-.02f,7),new Vector3(66,.12f,23),asphalt);
            for(int i=0;i<5;i++){float x=-32+i*16;Cube("Sanaani tower house",new Vector3(x,4,-23),new Vector3(10,8,8),plaster);Cube("Upper storey",new Vector3(x,9,-23),new Vector3(8,2,7),trim);Cube("Roof parapet",new Vector3(x,10.3f,-23),new Vector3(10,.6f,8),plaster);for(int w=-1;w<=1;w++)Cube("Decorative window",new Vector3(x+w*2.5f,5,-18.9f),new Vector3(1.1f,1.8f,.12f),glass);}
            string[] names={"محافظة المحويت","محافظة عدن","محافظة تعز"};
            for(int i=0;i<3;i++){float x=(i-1)*20;Cube("Gate pillar",new Vector3(x-5,2,15),new Vector3(1,4,1),plaster);Cube("Gate pillar",new Vector3(x+5,2,15),new Vector3(1,4,1),plaster);Cube("Governorate gate sign",new Vector3(x,4.2f,15),new Vector3(11,1.2f,.6f),trim);var label=WorldText(names[i],new Vector3(x,4.2f,14.6f),Color.white);label.transform.rotation=Quaternion.identity;Cube("Gate lane",new Vector3(x,-.01f,25),new Vector3(9,.12f,23),asphalt);var trigger=new GameObject("Choose "+names[i]);trigger.transform.position=new Vector3(x,1,24);var col=trigger.AddComponent<BoxCollider>();col.isTrigger=true;col.size=new Vector3(8,3,7);trigger.AddComponent<RouteGate>().Setup(this,(GovernorateRoute)i);}
            var boarding=Sphere("Passenger boarding blue circle",new Vector3(0,.15f,-1),new Vector3(5,.15f,5),blue);boarding.GetComponent<Collider>().isTrigger=true;boarding.AddComponent<MissionZone>().Configure(this,MissionZoneType.Boarding);
            var depotReturn=Cube("Return to Sanaa blue marker",new Vector3(8,.12f,-2),new Vector3(5,.15f,5),blue);var returnCollider=depotReturn.GetComponent<Collider>();returnCollider.isTrigger=true;depotReturn.AddComponent<MissionZone>().Configure(this,MissionZoneType.Depot);
            Cube("Depot canopy roof",new Vector3(0,6,2),new Vector3(26,.5f,12),trim);for(int x=-12;x<=12;x+=24)for(int z=-3;z<=7;z+=10)Cube("Canopy support",new Vector3(x,3,z),new Vector3(.35f,6,.35f),plaster);
        }
        private TextMesh WorldText(string value,Vector3 pos,Color color){var go=new GameObject("Arabic gate sign");go.transform.position=pos;var t=go.AddComponent<TextMesh>();t.text=value;t.fontSize=64;t.characterSize=.035f;t.color=color;t.anchor=TextAnchor.MiddleCenter;t.alignment=TextAlignment.Center;return t;}
        private void BuildRoadWorld()
        {
            RoadStrip("City asphalt road",new Vector3(0,0,39),new Vector3(0,0,82),9,asphalt);RoadStrip("Desert sand track",new Vector3(0,0,82),new Vector3(13,1,142),7,sand);RoadStrip("Mountain dirt climb",new Vector3(13,1,142),new Vector3(-9,13,215),6,dirt);RoadStrip("Mountain descent",new Vector3(-9,13,215),new Vector3(20,5,285),6,dirt);RoadStrip("Rare mud patch",new Vector3(1,8,181),new Vector3(2,8.1f,187),5,mud);
            for(int i=0;i<32;i++){float z=70+i*7.5f,x=Mathf.Sin(i*.77f)*24;Sphere("Desert and mountain boulder",new Vector3(x,Random.Range(1f,5f),z),new Vector3(Random.Range(4,10),Random.Range(3,9),Random.Range(4,11)),stone);}
            for(int i=0;i<22;i++){float z=95+i*8,x=Random.Range(-18f,18f);Sphere("Desert scrub",new Vector3(x,.5f,z),new Vector3(1.5f,.8f,1.4f),dirt);}
            routeMarkers.Add(Sphere("Mahweet destination",new Vector3(-9,13,215),new Vector3(5,.2f,5),blue));routeMarkers.Add(Sphere("Aden destination",new Vector3(20,5,285),new Vector3(5,.2f,5),blue));routeMarkers.Add(Sphere("Taiz destination",new Vector3(7,8,245),new Vector3(5,.2f,5),blue));for(int i=0;i<routeMarkers.Count;i++){var marker=routeMarkers[i];marker.GetComponent<Collider>().isTrigger=true;marker.AddComponent<MissionZone>().Configure(this,MissionZoneType.Destination);marker.SetActive(false);}
        }
        private void RoadStrip(string n,Vector3 a,Vector3 b,float width,Material mat){Vector3 d=(b-a).normalized,side=Vector3.Cross(d,Vector3.up).normalized*width*.5f;var mesh=new Mesh();mesh.vertices=new[]{a-side,a+side,b-side,b+side};mesh.triangles=new[]{0,2,1,2,3,1};mesh.RecalculateNormals();var go=new GameObject(n);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=mat;go.AddComponent<MeshCollider>().sharedMesh=mesh;}
        private void BuildVehicle()
        {
            carRoot=new GameObject("4x4 pickup prototype");carRoot.transform.position=new Vector3(0,.8f,-12);var rb=carRoot.AddComponent<Rigidbody>();rb.mass=2100;rb.drag=.05f;rb.angularDrag=2.5f;rb.centerOfMass=new Vector3(0,-.35f,0);
            carRoot.AddComponent<BoxCollider>().size=new Vector3(2,1,4.5f);
            var body=Cube("Vehicle paint body",carRoot.position+new Vector3(0,.35f,0),new Vector3(2,.8f,4.2f),paint,carRoot.transform);
            Cube("Cabin",carRoot.position+new Vector3(0,1.05f,-.45f),new Vector3(1.8f,.9f,2f),glass,carRoot.transform);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){var w=Sphere("All terrain tyre",carRoot.position+new Vector3(x*1.05f,-.1f,z*1.45f),new Vector3(.72f,.72f,.38f),rubber);w.transform.SetParent(carRoot.transform);}
            car=carRoot.AddComponent<Arcade4x4Controller>();fuel=carRoot.AddComponent<FuelSystem>();
            paintSystem=carRoot.AddComponent<VehiclePaintCustomization>();
            paintSystem.Configure("hilux_2026",new[]{body.GetComponent<Renderer>()},new[]{new PaintOption{displayName="أبيض",color=Color.white},new PaintOption{displayName="أسود",color=new Color(.025f,.025f,.03f)},new PaintOption{displayName="فضي",color=new Color(.55f,.58f,.61f)},new PaintOption{displayName="أحمر",color=new Color(.72f,.06f,.035f)},new PaintOption{displayName="رملي",color=new Color(.66f,.48f,.27f)},new PaintOption{displayName="أخضر",color=new Color(.12f,.28f,.15f)}});
        }
        private void BuildCamera(){var c=Camera.main;if(!c){var g=new GameObject("Main Camera");c=g.AddComponent<Camera>();g.tag="MainCamera";}cameraPivot=new GameObject("Camera follow").transform;c.transform.SetParent(cameraPivot);c.transform.localPosition=new Vector3(0,5,-9);c.transform.localRotation=Quaternion.Euler(18,0,0);}
        private void LateUpdate(){if(cameraPivot&&carRoot)cameraPivot.position=Vector3.Lerp(cameraPivot.position,carRoot.position,Time.deltaTime*4f);if(fuelBar&&fuel)fuelBar.value=fuel.Fuel/80f;if(fuelText&&fuel)fuelText.text="الوقود "+Mathf.CeilToInt(fuel.Fuel)+" L";}
        private void BuildUI()
        {
            if(FindObjectOfType<EventSystem>()==null){var eventSystem=new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));}
            var go=new GameObject("Game HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;go.GetComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;go.GetComponent<CanvasScaler>().referenceResolution=new Vector2(1920,1080);
            status=MakeText(canvas.transform,"Status",new Vector2(.5f,.93f),28);fuelText=MakeText(canvas.transform,"Fuel",new Vector2(.12f,.92f),24);routeText=MakeText(canvas.transform,"Route",new Vector2(.83f,.92f),24);fuelBar=MakeSlider(canvas.transform,new Vector2(.12f,.86f));
            MakeButton(canvas.transform,"لون السابق",new Vector2(.86f,.12f),()=>paintSystem.PreviewPrevious());MakeButton(canvas.transform,"لون التالي",new Vector2(.86f,.22f),()=>paintSystem.PreviewNext());MakeButton(canvas.transform,"اعتماد اللون",new Vector2(.86f,.32f),()=>paintSystem.ConfirmColor());
            MakeButton(canvas.transform,"تحميل الركاب",new Vector2(.5f,.12f),()=>mission.LoadPassengers());MakeButton(canvas.transform,"تسليم الركاب",new Vector2(.5f,.22f),()=>mission.ArriveAtDestination());MakeButton(canvas.transform,"العودة للفرزة",new Vector2(.5f,.32f),()=>mission.ReturnToDepot());MakeButton(canvas.transform,"تعبئة الوقود",new Vector2(.12f,.12f),()=>fuel.FillTank());
            MakeDriveButton(canvas.transform,"يمين",new Vector2(.68f,.3f),0,1);MakeDriveButton(canvas.transform,"يسار",new Vector2(.57f,.3f),0,-1);MakeDriveButton(canvas.transform,"تقدم",new Vector2(.68f,.45f),1,0);MakeDriveButton(canvas.transform,"فرامل",new Vector2(.57f,.45f),-1,0);
        }
        private Text MakeText(Transform p,string n,Vector2 a,int size){var g=new GameObject(n,typeof(RectTransform),typeof(Text));g.transform.SetParent(p,false);var r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=new Vector2(520,70);var t=g.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return t;}
        private Slider MakeSlider(Transform p,Vector2 a){var g=new GameObject("Fuel slider",typeof(RectTransform),typeof(Slider));g.transform.SetParent(p,false);var r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=new Vector2(260,24);var s=g.GetComponent<Slider>();s.minValue=0;s.maxValue=1;s.value=1;return s;}
        private void MakeDriveButton(Transform p,string label,Vector2 a,float drive,float turn){var g=CreateButton(p,label,a);var hold=g.AddComponent<MobileDriveHold>();hold.Configure(car,drive,turn);}
        private GameObject CreateButton(Transform p,string label,Vector2 a){var g=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));g.transform.SetParent(p,false);var r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=new Vector2(220,76);g.GetComponent<Image>().color=new Color(.04f,.18f,.28f,.88f);var t=MakeText(g.transform,"Label",new Vector2(.5f,.5f),21);t.text=label;return g;}
        private void MakeButton(Transform p,string label,Vector2 a,UnityEngine.Events.UnityAction action){var g=CreateButton(p,label,a);g.GetComponent<Button>().onClick.AddListener(action);}
        public void SelectRoute(GovernorateRoute route){mission.ChooseRoute(route);if(routeText)routeText.text=route==GovernorateRoute.Mahweet?"المسار: المحويت":route==GovernorateRoute.Aden?"المسار: عدن":"المسار: تعز";SetStatus("تم اختيار الوجهة. توجه إلى الدائرة الزرقاء لتحميل الركاب");for(int i=0;i<routeMarkers.Count;i++)routeMarkers[i].SetActive(i==(int)route);}
        public void OnMissionZoneEntered(MissionZoneType zone){if(mission==null)return;if(zone==MissionZoneType.Boarding){mission.LoadPassengers();SetStatus("تم تحميل الركاب. اتبع الطريق إلى وجهتك");}else if(zone==MissionZoneType.Destination){mission.ArriveAtDestination();SetStatus("وصلت إلى الوجهة! تم تسليم الركاب واستلام المكافأة. عد إلى صنعاء");}else if(zone==MissionZoneType.Depot){mission.ReturnToDepot();mission.StartNextMission();SetStatus("عدت إلى فرزة صنعاء. اختر وجهة جديدة");}}
        public void SetStatus(string value){if(status)status.text=value;}
    }
    public sealed class RouteGate:MonoBehaviour
    {
        private YemenGameWorld world;private GovernorateRoute route;
        public void Setup(YemenGameWorld w,GovernorateRoute r){world=w;route=r;}
        private void OnTriggerEnter(Collider other){if(other.GetComponentInParent<Arcade4x4Controller>())world.SelectRoute(route);}
    }
}
