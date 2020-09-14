using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR



public class ConvertMesh : EditorWindow
{
    private static bool isPBR;
    private static bool isTexture;

    [ExecuteInEditMode] [MenuItem("Window/ConvertMesh")]

    
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConvertMesh));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("From PBR"))
        {
            isPBR = true;
            isTexture = false;
        }
        if (GUILayout.Button("From Texture"))
        {
            isPBR = false;
            isTexture = true;
        }
        if (GUILayout.Button("CONVERT shader standard to vertexlightmap"))
        {
            ChangeShader();
        }
        if (GUILayout.Button("Change to Standard"))
        {
            JustChangeToStandard();
        }
        if (GUILayout.Button("CHange to VertexLightmap"))
        {
            JustChangeToVertexLightmap();
        }
        if (GUILayout.Button("-----Warning on Down------"))
        {

        }
        if (GUILayout.Button("!!!DELETE OLD MESHES!!!"))
        {
            DeleteMeshes();
        }
        if (GUILayout.Button("Recovery old Meshes"))
        {
            RecoveryOldMeshes();
        }
        if (GUILayout.Button("Change New Meshes on Old"))
        {
            ChangeNewMeshesOnOld();
        }

    }

    //private static MeshRenderer meshRender;

    //private Mesh newMesh;
    //private Mesh mesh;

    private static MeshRenderer[] meshRender;
    private static Mesh[] oldMeshs;
    private static Mesh[] oldMeshsRecovery;
    private static MeshFilter[] mesh;
    private static int undoID;

    private static void ChangeShader()
    {
        meshRender = new MeshRenderer[FindObjectsOfType<MeshRenderer>().Length];
        meshRender = FindObjectsOfType<MeshRenderer>();
        oldMeshs = new Mesh[meshRender.Length];
        oldMeshsRecovery = new Mesh[meshRender.Length];
        mesh = new MeshFilter[FindObjectsOfType<MeshFilter>().Length];
        for (int i = 0; i < meshRender.Length; i++)
        {
            mesh[i] = meshRender[i].transform.gameObject.GetComponent<MeshFilter>();

            oldMeshs[i] = mesh[i].sharedMesh;


        }

        Array.Copy(oldMeshs, oldMeshsRecovery, oldMeshs.Length);

        float[] metalic = new float[meshRender.Length];
        float[] smoothness = new float[meshRender.Length];

        for (int i = 0; i < meshRender.Length; i++)
        {
            metalic[i] = meshRender[i].sharedMaterial.GetFloat("_Metallic");
            smoothness[i] = meshRender[i].sharedMaterial.GetFloat("_Glossiness");
        }

        Mesh[] newMesh = new Mesh[mesh.Length];

        for (int i = 0; i < mesh.Length; i++)
        {
            newMesh[i] = new Mesh();
            newMesh[i] = Instantiate<Mesh>(mesh[i].sharedMesh);
            //newMesh[i]= mesh[i].sharedMesh;
            meshRender[i].GetComponent<MeshFilter>().sharedMesh = newMesh[i];


        }


        if (isPBR)
        {
            for (int i = 0; i < newMesh.Length; i++)
            {

                Color color = new Color();
                Color[] colors = new Color[newMesh[i].vertices.Length];
                List<Material> mats = new List<Material>();
                foreach (Material sharedMaterial in meshRender[i].sharedMaterials)
                {
                    mats.Add(sharedMaterial);
                }

                Color[] oldColors = newMesh[i].colors;
                for (int j = 0; j < newMesh[i].vertices.Length; j++)
                {
                    
                    foreach (Material material in mats)
                    {
                       
                            //color = material.GetColor("_EmissionColor");
                            
                            colors[j] = new Color(color.r, color.g, color.b, metalic[i]);
                        
                            
                    }


                }

                newMesh[i].colors = colors;
            }
        }

        if (isTexture)
            {
                for (int i = 0; i < newMesh.Length; i++)
                {

                    Color color = new Color();
                    Color[] colors = new Color[newMesh[i].vertices.Length];
                    Material mat = meshRender[i].sharedMaterial;
                    List<Texture2D> texture = new List<Texture2D>();
                    texture.Add(mat.GetTexture("_MainTex") as Texture2D);
                    for (int j = 0; j < newMesh[i].vertices.Length; j++)
                    {
                    
                        foreach (Texture2D texture1 in texture)
                        {
                            if (texture1 != null)
                            {
                                for (int a = 0; a < texture1.height; a++)
                                {
                                    for (int b = 0; b < texture1.width; b++)
                                    {
                                        //color = texture1.GetPixel(a, b);
                                        color = texture1.GetPixelBilinear(newMesh[i].uv[j].x, newMesh[i].uv[j].y);
                                        colors[j] = new Color(color.r, color.g, color.b, metalic[i]);
                                    }
                                }
                            }
                        }

                    }

                    newMesh[i].colors = colors;

                }

            }

            //for (int i = 0; i < mesh.Length; i++)
            //{

            //    Vector2[] uvs = new Vector2[mesh[i].sharedMesh.vertices.Length];
            //    for (int j = 0; j < uvs.Length; j++)
            //    {
            //        uvs[j] = new Vector2(smoothness[i], mesh[i].sharedMesh.vertices[i].z);
            //    }

            //    mesh[i].sharedMesh.uv3 = uvs;

            //}

            for (int i = 0; i < meshRender.Length; i++)
            {
                meshRender[i].sharedMaterial.shader = Shader.Find("VertexLightmap");
            }

            for (int i = 0; i < meshRender.Length; i++)
            {
                string halfPath = "Assets/Scenes/";
                string sceneName = SceneManager.GetActiveScene().name;
                string lightmapName = "/Lightmap-0_comp_light.exr";
                string fullPath = halfPath + sceneName + lightmapName;
                Texture2D lightMap =
                    (Texture2D) AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
                //AssetDatabase.LoadAssetAtPath("Assets/Scenes/NewSphereScene/Lightmap-0_comp_light.exr");
                Vector4 scaleOffset = meshRender[i].lightmapScaleOffset;
                Vector2 scale = new Vector2(scaleOffset.x, scaleOffset.y);
                Vector2 offset = new Vector2(scaleOffset.z, scaleOffset.w);
                meshRender[i].sharedMaterial.SetTexture("_LM", lightMap);
                meshRender[i].sharedMaterial.SetTextureOffset("_LM", offset);
                meshRender[i].sharedMaterial.SetTextureScale("_LM", scale);
            }

            for (int i = 0; i < meshRender.Length; i++)
            {
                meshRender[i].sharedMaterial.SetFloat("_Smoothness", smoothness[i]);
            }


    }

    private static void DeleteMeshes()
    {
        for (int i = 0; i < oldMeshsRecovery.Length; i++)
        {
            Undo.RecordObject(oldMeshsRecovery[i], "Base Undo");
        }

        undoID = Undo.GetCurrentGroup();

        for (int i = 0; i < oldMeshs.Length; i++)
        {
            Undo.DestroyObjectImmediate(oldMeshsRecovery[i]);
            Undo.CollapseUndoOperations(undoID);
            //DestroyImmediate(oldMeshs[i],true);
        }
    }

    private static void JustChangeToStandard()
    {
        meshRender = new MeshRenderer[FindObjectsOfType<MeshRenderer>().Length];
        meshRender = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < meshRender.Length; i++)
        {
            meshRender[i].sharedMaterial.shader = Shader.Find("Standard");
        }
    }

    private static void JustChangeToVertexLightmap()
    {
        meshRender = new MeshRenderer[FindObjectsOfType<MeshRenderer>().Length];
        meshRender = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < meshRender.Length; i++)
        {
            meshRender[i].sharedMaterial.shader = Shader.Find("VertexLightmap");
        }

    }

    private static void RecoveryOldMeshes()
    {
        Undo.RevertAllDownToGroup(undoID);
        for (int i = 0; i < oldMeshsRecovery.Length; i++)
        {
            mesh[i].sharedMesh = oldMeshsRecovery[i];
        }

    }

    private static void ChangeNewMeshesOnOld()
    {
        for (int i = 0; i < oldMeshs.Length; i++)
        {
            mesh[i].sharedMesh = oldMeshs[i];
        }
    }
}
#endif