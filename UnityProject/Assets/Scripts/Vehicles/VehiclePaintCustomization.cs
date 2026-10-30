using UnityEngine;
using UnityEngine.UI;
namespace Yemen4x4.Vehicles
{
 [System.Serializable] public sealed class PaintOption{public string displayName="أبيض";public Color color=Color.white;}
 public sealed class VehiclePaintCustomization:MonoBehaviour
 {
  private string vehicleId="vehicle";private Renderer[] paintRenderers;private PaintOption[] palette;private Image previewSwatch;private int selectedIndex;
  private string SaveKey=>"vehicle.paint."+vehicleId;
  public void Configure(string id,Renderer[] renderers,PaintOption[] options){vehicleId=id;paintRenderers=renderers;palette=options;selectedIndex=Mathf.Clamp(PlayerPrefs.GetInt(SaveKey,0),0,Mathf.Max(0,palette.Length-1));Apply(selectedIndex);}
  public void SelectColor(int index){if(palette==null||palette.Length==0)return;selectedIndex=Mathf.Clamp(index,0,palette.Length-1);Apply(selectedIndex);}
  public void ConfirmColor(){PlayerPrefs.SetInt(SaveKey,selectedIndex);PlayerPrefs.Save();}
  public void PreviewNext()=>SelectColor((selectedIndex+1)%Mathf.Max(1,palette.Length));
  public void PreviewPrevious()=>SelectColor((selectedIndex-1+Mathf.Max(1,palette.Length))%Mathf.Max(1,palette.Length));
  private void Apply(int index){if(palette==null||palette.Length==0)return;foreach(var r in paintRenderers){if(!r)continue;var b=new MaterialPropertyBlock();r.GetPropertyBlock(b);b.SetColor("_BaseColor",palette[index].color);b.SetColor("_Color",palette[index].color);r.SetPropertyBlock(b);}if(previewSwatch)previewSwatch.color=palette[index].color;}
 }
}
