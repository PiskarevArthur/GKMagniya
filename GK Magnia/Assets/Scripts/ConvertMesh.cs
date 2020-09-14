using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if UNITY_EDITOR



public class ConvertMesh : EditorWindow
{
 

    [ExecuteInEditMode] [MenuItem("Window/ConvertMesh")]

    
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConvertMesh));
    }

    public void OnGUI()
    {
        
        if (GUILayout.Button("CONVERT shader standard to vertexlightmap"))
        {
            ChangeShader();
        }
        if (GUILayout.Button("Change to Standard"))
        {
            JustChangeToStandard();
        }
        if (GUILayout.Button("Change to VertexLightmap"))
        {
            JustChangeToVertexLightmap();
        }
        if (GUILayout.Button("-----Warning on Down------"))
        {

        }
        //if (GUILayout.Button("!!!DELETE OLD MESHES!!!"))
        //{
        //   // DeleteMeshes();
        //}
        //if (GUILayout.Button("Recovery old Meshes"))
        //{
        //  //  RecoveryOldMeshes();
        //}
        if (GUILayout.Button("Save new Mesh"))
        {
            SaveNewMesh();
        }
       
    }

    //private static MeshRenderer meshRender;

    //private Mesh newMesh;
    //private Mesh mesh;

    private static List<GameObject> activeGO;
    private static List<MeshRenderer> meshRender;
    private static List<Mesh> oldMeshs;
    private static List<Mesh> oldMeshsRecovery;
    private static List<MeshFilter> mesh;
    private static Mesh[] newMesh;
    private static int undoID;

    private static void ChangeShader()
    {
        activeGO = new List<GameObject>();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
           
                activeGO.Add(Selection.gameObjects[i]);

            if(Selection.gameObjects[i].transform.childCount>0)
            {
                for (int j = 0; j < Selection.gameObjects[i].transform.childCount; j++)
                {
                    activeGO.Add(Selection.gameObjects[i].transform.GetChild(j).gameObject);

                    if (Selection.gameObjects[i].transform.GetChild(j).childCount > 0)
                    {
                        for (int a = 0; a < Selection.gameObjects[i].transform.GetChild(j).childCount; a++)
                        {
                            activeGO.Add(Selection.gameObjects[i].transform.GetChild(j).GetChild(a).gameObject);

                            if (Selection.gameObjects[i].transform.GetChild(j).GetChild(a).childCount > 0)
                            {
                                for (int b = 0;
                                    b < Selection.gameObjects[i].transform.GetChild(j).GetChild(a).childCount;
                                    b++)
                                {
                                    activeGO.Add(Selection.gameObjects[i].transform.GetChild(j).GetChild(a).GetChild(b)
                                        .gameObject);
                                }
                            }
                        }
                    }
                }

            }
        }
        meshRender=new List<MeshRenderer>();
        mesh=new List<MeshFilter>();
        oldMeshs=new List<Mesh>();
        for (int i = 0; i < activeGO.Count; i++)
        {
            meshRender.Add(activeGO[i].GetComponent<MeshRenderer>());
            mesh.Add(activeGO[i].GetComponent<MeshFilter>());
            if(mesh[i]!=null)
            oldMeshs.Add(mesh[i].sharedMesh);
           
            
        }

        for (int i = 0; i < meshRender.Count; i++)
        {
            if (meshRender[i] == null)
            {
                meshRender.RemoveAt(i);
                mesh.RemoveAt(i);
                
            }
            
            
        }
       
        //oldMeshsRecovery=new Mesh();
        //oldMeshsRecovery = mesh.sharedMesh;



        List<float> metalic=new List<float>();
        List<float> smoothness=new List<float>();
        for (int i = 0; i < meshRender.Count; i++)
        {
            metalic.Add(meshRender[i].sharedMaterial.GetFloat("_Metallic"));
            smoothness.Add(meshRender[i].sharedMaterial.GetFloat("_Glossiness"));
        }

        newMesh = new Mesh[mesh.Count];
        if (mesh.Count > 0)
        {
            for (int i = 0; i < mesh.Count; i++)
            {
               
                newMesh[i] =new Mesh();
                newMesh[i] = Instantiate<Mesh>(mesh[i].sharedMesh);
                //newMesh[i]= mesh[i].sharedMesh;
                mesh[i].sharedMesh = newMesh[i];
            }
        }




        for (int i = 0; i < newMesh.Length; i++)
        {
            Color color = new Color();
            Color colorEm=new Color();
            Color[] colors = new Color[newMesh[i].vertexCount];
            Color[] colorsEmmision = new Color[newMesh[i].vertexCount];
            //Color colEmissMultiMat = meshRender[i].sharedMaterial.GetColor("_EmissionColor");
            string [] texturePropertyName= new string[meshRender[i].sharedMaterial.GetTexturePropertyNames().Length];
            texturePropertyName = meshRender[i].sharedMaterial.GetTexturePropertyNames();
            Texture2D textureMetallic = (Texture2D) meshRender[i].sharedMaterial.GetTexture("_MetallicGlossMap");

            Vector2[] uvs = new Vector2[mesh[i].sharedMesh.vertices.Length];

            Color[] materialColors=new Color[newMesh[i].vertexCount];
            //Color emission = meshRender[i].sharedMaterial.GetColor("_EmissionColor");
            //Texture2D texture = (Texture2D) meshRender[i].sharedMaterial.mainTexture;
            //Texture2D textureEmission = (Texture2D) meshRender[i].sharedMaterial.GetTexture("_EmissionMap");
            //Texture2D textureNormal = (Texture2D)meshRender[i].sharedMaterial.GetTexture("_EmissionMap");
            //for (int a = 0; a < textureID.Length; a++)
            //{
            // Texture2D texture = (Texture2D)meshRender[i].sharedMaterial.GetTexture(textureID[a]);
            foreach (string s in texturePropertyName)
            {
               
                Texture2D texture = (Texture2D)meshRender[i].sharedMaterial.GetTexture(s);
                
                if (texture != null)
                {
                    if (s == "_MainTex" || s == "_EmissionMap"||s=="_MetallicGlossMap")
                    {
                      

                        for (int j = 0; j < newMesh[i].vertexCount; j++)
                        {
                            if (s == "_MainTex")
                            {
                                color = texture.GetPixelBilinear(newMesh[i].uv[j].x, newMesh[i].uv[j].y);
                                colors[j] = new Color(color.r, color.g, color.b, 0);
                                if (textureMetallic == null)
                                {
                                    uvs[j] = new Vector2(metalic[i], smoothness[i]);
                                }
                            }

                            if (s == "_EmissionMap")
                            {
                                colorEm = texture.GetPixelBilinear(newMesh[i].uv[j].x, newMesh[i].uv[j].y);
                                colorsEmmision[j] = new Color(colors[j].r, colors[j].g, colors[j].b,
                                    colorEm.maxColorComponent);
                            }

                            if (s == "_MetallicGlossMap")
                            {
                                color = texture.GetPixelBilinear(newMesh[i].uv[j].x, newMesh[i].uv[j].y);
                                uvs[j] = new Vector2(color.r, color.a);
                            }
                        }

                        if (s == "_MainTex")
                        {
                            newMesh[i].colors = colors;
                            
                        }

                        if (s == "_EmissionMap")
                        {
                            newMesh[i].colors = colorsEmmision;
                        }
                    }
                }

                
               

            }
            if (meshRender[i].sharedMaterial.mainTexture == null)
            {
                //for (int j = 0; j < meshRender[i].sharedMaterials.Length; j++)
                //{
                //    materialColors.AddRange(meshRender[i].sharedMaterials[j].GetColorArray("_Color"));
                int count = 0;
                

                for (int b = 0; b < newMesh[i].subMeshCount; b++)
                    {
                        Color colEmissMultiMat = meshRender[i].sharedMaterials[b].GetColor("_EmissionColor");
                    for (int j = newMesh[i].GetSubMesh(b).firstVertex;
                            j < newMesh[i].GetSubMesh(b).vertexCount+count;
                            j++)
                        {

                            color = meshRender[i].materials[b].color;
                        if (colEmissMultiMat != null)
                        {
                            materialColors[j] = (new Color(color.r, color.g, color.b,
                                colEmissMultiMat.maxColorComponent));

                        }
                        else
                        {
                            materialColors[j] = (new Color(color.r, color.g, color.b,
                                0));
                        }
                    }

                        count += newMesh[i].GetSubMesh(b).vertexCount;
                    }

                   

                newMesh[i].colors = materialColors;

                for (int j = 0; j < newMesh[i].vertexCount; j++)
                {
                    uvs[j] = new Vector2(metalic[i], smoothness[i]);

                }
              
            }

            mesh[i].sharedMesh.uv = uvs;

            

            //for (int j = 0; j < newMesh[i].vertices.Length; j++)
            //{
            //    if (texture != null)
            //    {
            //        color = texture.GetPixelBilinear(newMesh[i].uv[j].x, newMesh[i].uv[j].y);
            //        colors[j] = new Color(color.r, color.g, color.b, 0);
            //    }


            //}

            //newMesh[i].colors = colors;

            //for (int j = 0; j < newMesh[i].vertices.Length; j++)
            //{
            //    if (textureEmission != null)
            //    {
            //        color = textureEmission.GetPixelBilinear(newMesh[i].uv[j].x, newMesh[i].uv[j].y);
            //        colorsEmmision[j] = new Color(colors[j].r, colors[j].g, colors[j].b, color.maxColorComponent);
            //    }
            //}

            //if (textureEmission != null)
            //{
            //    newMesh[i].colors = colorsEmmision;
            //}
        }

        for (int i = 0; i < meshRender.Count; i++)
        {
            for (int j = 0; j < meshRender[i].sharedMaterials.Length; j++)
            {
                meshRender[i].sharedMaterial.shader = Shader.Find("VertexLightmap");
            }
        }
        //if (isTexture)
        //    {
        //        for (int i = 0; i < newMesh.Length; i++)
        //        {

        //            Color color = new Color();
        //            Color[] colors = new Color[newMesh[i].vertices.Length];
        //            Material mat = meshRender[i].sharedMaterial;
        //            List<Texture2D> texture = new List<Texture2D>();
        //            texture.Add(mat.GetTexture("_MainTex") as Texture2D);
        //            for (int j = 0; j < newMesh[i].vertices.Length; j++)
        //            {

        //                foreach (Texture2D texture1 in texture)
        //                {
        //                    if (texture1 != null)
        //                    {
        //                        for (int a = 0; a < texture1.height; a++)
        //                        {
        //                            for (int b = 0; b < texture1.width; b++)
        //                            {
        //                                color = texture1.GetPixel(a, b);

        //                                        //color = texture1.GetPixelBilinear(newMesh[i].uv[z].x,
        //                                        //    newMesh[i].uv[z].y);
        //                                        colors[j] = new Color(color.r, color.g, color.b, metalic[i]);


        //                            }
        //                        }
        //                    }
        //                }

        //            }

        //            for (int j = 0; j < newMesh[i].colors.Length; j++)
        //            {
        //                if(newMesh[i].colors[j]==colors[j])
        //                newMesh[i].colors = colors;
        //            }

        //        }

        //    }


        ////for (int i = 0; i < mesh.Count; i++)
        ////{
        ////    Vector2[] uvs = new Vector2[mesh[i].sharedMesh.vertices.Length];

        ////    for (int j = 0; j < uvs.Length; j++)
        ////    {
        ////        uvs[j] = new Vector2(metalic[i], smoothness[i]);
        ////    }

        ////    mesh[i].sharedMesh.uv = uvs;

        ////}


        //for (int i = 0; i < mesh.Length; i++)
        //{

        //    Vector2[] uvs = new Vector2[mesh[i].sharedMesh.vertices.Length];
        //    for (int j = 0; j < uvs.Length; j++)
        //    {
        //        uvs[j] = new Vector2(smoothness[i], mesh[i].sharedMesh.vertices[i].z);
        //    }

        //    mesh[i].sharedMesh.uv3 = uvs;

        //}
        ////for (int i = 0; i < meshRender.Count; i++)
        ////{
        ////    meshRender[i].sharedMaterial.shader = Shader.Find("VertexLightmap");
        ////}



        //string halfPath = "Assets/Scenes/";
        //string sceneName = SceneManager.GetActiveScene().name;
        //string lightmapName = "/Lightmap-0_comp_light.exr";
        //string fullPath = halfPath + sceneName + lightmapName;
        //Texture2D lightMap =
        //    (Texture2D) AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
        ////AssetDatabase.LoadAssetAtPath("Assets/Scenes/NewSphereScene/Lightmap-0_comp_light.exr");
        //Vector4 scaleOffset = meshRender.lightmapScaleOffset;
        //Vector2 scale = new Vector2(scaleOffset.x, scaleOffset.y);
        //Vector2 offset = new Vector2(scaleOffset.z, scaleOffset.w);
        //meshRender.sharedMaterial.SetTexture("_LM", lightMap);
        //meshRender.sharedMaterial.SetTextureOffset("_LM", offset);
        //meshRender.sharedMaterial.SetTextureScale("_LM", scale);


        //meshRender.sharedMaterial.SetFloat("_Smoothness", smoothness);



    }

    //private static void DeleteMeshes()
    //{
    //    for (int i = 0; i < oldMeshsRecovery.Length; i++)
    //    {
    //        Undo.RecordObject(oldMeshsRecovery[i], "Base Undo");
    //    }

    //    undoID = Undo.GetCurrentGroup();

    //    for (int i = 0; i < oldMeshs.Length; i++)
    //    {
    //        Undo.DestroyObjectImmediate(oldMeshsRecovery[i]);
    //        Undo.CollapseUndoOperations(undoID);
    //        //DestroyImmediate(oldMeshs[i],true);
    //    }
    //}

    private static void JustChangeToStandard()
    {
        ChangeNewMeshesOnOld();
        for (int i = 0; i < meshRender.Count; i++)
        {
            
            for (int j = 0; j < meshRender[i].sharedMaterials.Length; j++)
            {
                meshRender[i].sharedMaterial.shader = Shader.Find("Standard");
            }
        }
    }

    private static void JustChangeToVertexLightmap()
    {
       ChangeOldMeshesOnNew();
        for (int i = 0; i < meshRender.Count; i++)
        {
            for (int j = 0; j < meshRender[i].sharedMaterials.Length; j++)
            {
                meshRender[i].sharedMaterial.shader = Shader.Find("VertexLightmap");
            }
        }

    }

    //private static void RecoveryOldMeshes()
    //{
    //    Undo.RevertAllDownToGroup(undoID);
    //    for (int i = 0; i < oldMeshsRecovery.Length; i++)
    //    {
    //        mesh[i].sharedMesh = oldMeshsRecovery[i];
    //    }

    //}

    private static void ChangeNewMeshesOnOld()
    {
        for (int i = 0; i < mesh.Count; i++)
        {
            mesh[i].sharedMesh = oldMeshs[i];
        }

    }
    private static void ChangeOldMeshesOnNew()
    {
        for (int i = 0; i < mesh.Count; i++)
        {
            mesh[i].sharedMesh = newMesh[i];
        }
    }
    private static void SaveNewMesh()
    {
        string [] assetPath=new string[newMesh.Length];
        List<int> tempList=new List<int>();
        for (int i = 0; i <newMesh.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(oldMeshs[i]);
            if (path.Contains(".asset"))
            {
                path = path.Replace(".asset", "");
                
            }
            else if (path.Contains(".FBX"))
            {
                path = path.Replace(".FBX", "");
                
            }

            path = path + "ConvertedMesh";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (i == 0)
            {
                assetPath[i] = path + "/" + newMesh[i].name + ".asset";
                tempList.Add(0);
            }
            else 
            {
                if (!assetPath.Contains(assetPath[i]))
                {
                    assetPath[i] = path + "/" + newMesh[i].name + ".asset";
                    tempList.Add(0);
                }

                int temp = Random.Range(0, 100);
                assetPath[i] = path + "/" + newMesh[i].name + temp + ".asset";
                if (assetPath.Contains(assetPath[i]))
                {
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        while (assetPath[j].Contains(temp.ToString()))
                        {
                            temp = Random.Range(0, 100);
                        }
                    }

                    assetPath[i] = path + "/" + newMesh[i].name +temp+ ".asset";
                    tempList.Add(0);

                }
            }

            Mesh prefabMesh = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath[i],
                typeof(Mesh)); //3
            if (!prefabMesh)
            {
                AssetDatabase.CreateAsset(newMesh[i], assetPath[i]);
            }
        }
    }
}
#endif