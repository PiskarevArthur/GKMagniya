using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class LinksDetector : MonoBehaviour
{
    [SerializeField] string Worked = "Not Linked";
    [SerializeField] string LinkedTime = "0";
    [ContextMenu("Link!")]
    private void Reset()
    {
        DetectLink();
    }
    void DetectLink()
    { 
        float T = Time.realtimeSinceStartup;
        List<link> Links = new List<link>();
        int linkcount=0;
        Links.Clear(); 

         
            for (int i = 0; i < transform.childCount; i++)
            {
              if(  transform.GetChild(i).gameObject.GetComponent<link>()==null)   transform.GetChild(i).gameObject.AddComponent<link>();
                if (transform.GetChild(i).gameObject.GetComponent<Rigidbody>() == null) transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
                if (transform.GetChild(i).gameObject.GetComponent<MeshCollider>() == null) transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
                if (transform.GetChild(i).gameObject.GetComponent<BoxCollider>()) DestroyImmediate(transform.GetChild(i).gameObject.GetComponent<BoxCollider>());
                if (transform.GetChild(i).gameObject.GetComponent<SphereCollider>()) DestroyImmediate(transform.GetChild(i).gameObject.GetComponent<SphereCollider>());
                transform.GetChild(i).gameObject.GetComponent<MeshCollider>().convex = true;
            }
        Links.AddRange(GetComponentsInChildren<link>());
        List<MeshFilter> meshes = new List<MeshFilter>();
        meshes.AddRange(GetComponentsInChildren<MeshFilter>());
        List <List<Vector3>> Bufs = new List <List<Vector3>>();
        for (int i = 0; i < Links.Count; i++)
        {
            Links[i].links=new List<link>();
            Links[i].links?.Clear();
            List<Vector3> vertexBuffer = new List<Vector3>();
            meshes[i].sharedMesh.GetVertices(vertexBuffer);

            for (int j = 0; j < vertexBuffer.Count; j++)
            {
                vertexBuffer[j] = meshes[i].transform.TransformPoint(vertexBuffer[j]);
            }

            Bufs.Add(vertexBuffer);
        }

        for (int i = 0; i < Bufs.Count; i++)
        {
            for (int j = 0; j < Bufs.Count; j++)
            {
                if (i != j)
                {
                    // Debug.Log("processing: " + i.ToString());
                    if (CompareLists(Bufs[i], Bufs[j]) > 3)
                    {
                        
                        Links[i].links.Add(Links[j]);
                        Worked = "links: " + ++linkcount;
                    }
                }
                
            }
        }
         LinkedTime="Просчёт занял : "+(Time.realtimeSinceStartup-T).ToString()+"Sec";
    }
    int CompareLists(List <Vector3> a, List <Vector3> b)
    {
        int sum = 0;
        for (int i = 0; i < a.Count; i++)
        {
            for (int j = 0; j < b.Count; j++)
            {
                float dist = Vector3.SqrMagnitude(a[i]- b[j]);
                if (dist < 0.001f)
                {
                    sum++;
                    if (sum > 2) return 4;
                }
            }
        }

        return 0;
    }
  
}
