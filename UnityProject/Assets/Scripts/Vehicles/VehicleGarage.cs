using System;
using UnityEngine;
using UnityEngine.UI;

namespace Yemen4x4.Vehicles
{
    public enum YemenVehicleModel { Hilux2026, LandCruiserVXR2026, LandCruiser70Shas2026, JeepQuarter }
    /// <summary>Four selectable 4x4 profiles with independently persisted paint colors.</summary>
    public sealed class VehicleGarage : MonoBehaviour
    {
        [Serializable] public struct ModelSpec { public string id; public string title; public Vector3 bodyScale; public Vector3 cabinScale; public float wheelBase; }
        private static readonly ModelSpec[] Specs = {
            new ModelSpec{id="hilux_2026",title="Toyota Hilux 2026",bodyScale=new Vector3(2f,.8f,4.2f),cabinScale=new Vector3(1.8f,.9f,2f),wheelBase=1.45f},
            new ModelSpec{id="lc_vxr_2026",title="Land Cruiser VXR 2026",bodyScale=new Vector3(2.05f,1f,4.8f),cabinScale=new Vector3(1.9f,1.15f,2.5f),wheelBase=1.65f},
            new ModelSpec{id="lc70_shas_2026",title="Land Cruiser 70 / Shas 2026",bodyScale=new Vector3(2f,1f,4.5f),cabinScale=new Vector3(1.8f,1.15f,2.1f),wheelBase=1.55f},
            new ModelSpec{id="jeep_quarter",title="Jeep Quarter",bodyScale=new Vector3(1.8f,.65f,3.4f),cabinScale=new Vector3(1.7f,.75f,1.8f),wheelBase=1.2f}
        };
        private Transform root; private Renderer bodyRenderer; private Text label; private int selected; private GameObject pickupBed,suvRear,shasSpare,jeepCage;
        private readonly Color[] palette={Color.white,new Color(.025f,.025f,.03f),new Color(.55f,.58f,.61f),new Color(.72f,.06f,.035f),new Color(.66f,.48f,.27f),new Color(.12f,.28f,.15f)};
        private int colorIndex;
        public void Initialize(Transform vehicleRoot, Renderer body, Transform ui)
        {
            root=vehicleRoot; bodyRenderer=body;pickupBed=root.Find("Hilux pickup bed")?.gameObject;suvRear=root.Find("VXR rear cabin")?.gameObject;shasSpare=root.Find("Shas rear spare")?.gameObject;jeepCage=root.Find("Jeep roll cage")?.gameObject;
            var g=new GameObject("Garage model label",typeof(RectTransform),typeof(Text));g.transform.SetParent(ui,false);
            var r=g.GetComponent<RectTransform>();r.anchorMin=new Vector2(.5f,.03f);r.anchorMax=new Vector2(.5f,.03f);r.sizeDelta=new Vector2(520,48);
            label=g.GetComponent<Text>();label.font=Resources.GetBuiltinResource<Font>("Arial.ttf");label.fontSize=22;label.alignment=TextAnchor.MiddleCenter;label.color=Color.white;
            ApplyModel(0);
        }
        public void SelectNext(){ApplyModel((selected+1)%Specs.Length);}
        public void SelectPrevious(){ApplyModel((selected+Specs.Length-1)%Specs.Length);}
        public void NextColor(){colorIndex=(colorIndex+1)%palette.Length;ApplyColor(false);}
        public void PreviousColor(){colorIndex=(colorIndex+palette.Length-1)%palette.Length;ApplyColor(false);}
        public void ConfirmColor(){PlayerPrefs.SetInt("vehicle.paint."+Specs[selected].id,colorIndex);PlayerPrefs.Save();}
        private void ApplyModel(int index)
        {
            selected=index; var spec=Specs[selected];if(pickupBed)pickupBed.SetActive(selected==0);if(suvRear)suvRear.SetActive(selected==1);if(shasSpare)shasSpare.SetActive(selected==2);if(jeepCage)jeepCage.SetActive(selected==3);
            root.localScale=Vector3.one;
            var body=root.Find("Vehicle paint body"); if(body)body.localScale=spec.bodyScale;
            var cabin=root.Find("Cabin");if(cabin){cabin.localScale=spec.cabinScale;cabin.localPosition=new Vector3(0,spec.bodyScale.y*.85f,-.45f);}
            colorIndex=Mathf.Clamp(PlayerPrefs.GetInt("vehicle.paint."+spec.id,0),0,palette.Length-1);
            ApplyColor(true);if(label)label.text=spec.title;
        }
        private void ApplyColor(bool persistLoad)
        {
            if(!bodyRenderer)return;var block=new MaterialPropertyBlock();bodyRenderer.GetPropertyBlock(block);
            block.SetColor("_BaseColor",palette[colorIndex]);block.SetColor("_Color",palette[colorIndex]);bodyRenderer.SetPropertyBlock(block);
            if(persistLoad && label) label.text=Specs[selected].title+" — اللون المحفوظ";
        }
    }
}