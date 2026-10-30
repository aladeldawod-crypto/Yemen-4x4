using UnityEngine;

namespace Yemen4x4.World
{
    /// <summary>Generated low-poly terrain backdrop; road strips remain separate drivable surfaces.</summary>
    public sealed class ProceduralYemenTerrain : MonoBehaviour
    {
        public void Build(Material rock, Material sand)
        {
            BuildRidge("Western mountain ridge",new Vector3(-34,0,190),new Vector3(48,0,280),rock,19f,54);
            BuildRidge("Eastern mountain ridge",new Vector3(38,0,160),new Vector3(76,0,270),rock,15f,48);
            BuildDunes(sand);
        }
        private void BuildRidge(string title,Vector3 start,Vector3 end,Material mat,float height,int seed)
        {
            const int nx=28,nz=42;var mesh=new Mesh();var v=new Vector3[(nx+1)*(nz+1)];var uv=new Vector2[v.Length];var tri=new int[nx*nz*6];
            for(int z=0;z<=nz;z++)for(int x=0;x<=nx;x++){float u=(float)x/nx,w=(float)z/nz;float ridge=Mathf.Sin(u*8f+seed)*.5f+.5f;float noise=Mathf.PerlinNoise(u*5f+seed,w*4f+seed);float y=2f+height*(.25f*ridge+.75f*noise)*Mathf.Sin(w*Mathf.PI);v[z*(nx+1)+x]=new Vector3(Mathf.Lerp(start.x,end.x,u),y,Mathf.Lerp(start.z,end.z,w));uv[z*(nx+1)+x]=new Vector2(u*4,w*4);}
            int k=0;for(int z=0;z<nz;z++)for(int x=0;x<nx;x++){int a=z*(nx+1)+x,b=a+1,c=a+nx+1,d=c+1;tri[k++]=a;tri[k++]=c;tri[k++]=b;tri[k++]=b;tri[k++]=c;tri[k++]=d;}
            mesh.vertices=v;mesh.triangles=tri;mesh.uv=uv;mesh.RecalculateNormals();var go=new GameObject(title);go.transform.SetParent(transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=mat;go.AddComponent<MeshCollider>().sharedMesh=mesh;
        }
        private void BuildDunes(Material mat)
        {
            for(int i=0;i<12;i++){var dune=GameObject.CreatePrimitive(PrimitiveType.Sphere);dune.name="Wind shaped desert dune";dune.transform.SetParent(transform,false);dune.transform.position=new Vector3(Mathf.Sin(i*1.7f)*38f,-1f,90+i*7f);dune.transform.localScale=new Vector3(24,3+Mathf.PerlinNoise(i,.3f)*4,14);dune.GetComponent<Renderer>().sharedMaterial=mat;}
        }
    }
}