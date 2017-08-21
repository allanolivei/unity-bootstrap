using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshCombine : ScriptableWizard
{
    public GameObject meshToCombine;
    protected GameObject gameObject;

    private class CombinedMesh : MonoBehaviour
    {
        public GameObject target;
    }

    [CustomEditor(typeof(CombinedMesh))]
    private class CombinedMeshEditor : Editor
    {

        private CombinedMesh combined;

        void OnEnable()
        {
            combined = target as CombinedMesh;
        }

        public override void OnInspectorGUI()
        {
            if (combined == null || combined.target == null) return;

            if (combined.target.activeInHierarchy && GUILayout.Button("Deactive Reference"))
            {
                combined.target.SetActive(false);
            }
            else if (!combined.target.activeInHierarchy && GUILayout.Button("Active Reference"))
            {
                combined.target.SetActive(true);
            }

            if( GUILayout.Button("Generate") )
            {
                //MeshCombine.Generate(combined.target, combined.gameObject);
            }
        }
    }

    [MenuItem("Mesh Combine/Combine Meshes")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Combine Mesh", typeof(MeshCombine));
    }

    static public void Generate( GameObject meshToCombine, GameObject gameObject )
    {
        if (meshToCombine != null)
        {
            // Find all mesh filter submeshes and separate them by their cooresponding materials
            ArrayList materials = new ArrayList();
            ArrayList combineInstanceArrays = new ArrayList();
            Mesh meshResult = new Mesh();
            string fileName = "CombinedMesh_" + meshToCombine.name;
            string filePath = "Assets/CombinedMesh";
            string prefabPath = filePath + "/" + fileName + ".prefab";
            if (!AssetDatabase.IsValidFolder(filePath)) AssetDatabase.CreateFolder("Assets", "CombinedMesh");

            MeshFilter[] meshFilters = meshToCombine.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

                // Handle bad input
                if (!meshRenderer)
                {
                    Debug.LogError("MeshFilter does not have a coresponding MeshRenderer.");
                    continue;
                }
                if (meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
                {
                    Debug.LogError("Mismatch between material count and submesh count. Is this the correct MeshRenderer?");
                    continue;
                }

                for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
                {
                    int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                    if (materialArrayIndex == -1)
                    {
                        materials.Add(meshRenderer.sharedMaterials[s]);
                        materialArrayIndex = materials.Count - 1;
                        combineInstanceArrays.Add(new ArrayList());
                    }

                    CombineInstance combineInstance = new CombineInstance();
                    combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                    combineInstance.subMeshIndex = s;
                    combineInstance.mesh = meshFilter.sharedMesh;
                    (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
                }
            }

            if (gameObject == null)
            {
                gameObject = new GameObject(fileName);
                gameObject.transform.SetParent( meshToCombine.transform.parent );
                meshToCombine.SetActive(false);
            }

            // For MeshFilter
            {
                // Get / Create mesh filter
                MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
                if (!meshFilterCombine)
                    meshFilterCombine = gameObject.AddComponent<MeshFilter>();

                // Combine by material index into per-material meshes
                // also, Create CombineInstance array for next step
                Mesh[] meshes = new Mesh[materials.Count];
                CombineInstance[] combineInstances = new CombineInstance[materials.Count];

                for (int m = 0; m < materials.Count; m++)
                {
                    CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
                    meshes[m] = new Mesh();
                    meshes[m].CombineMeshes(combineInstanceArray, true, true);

                    combineInstances[m] = new CombineInstance();
                    combineInstances[m].mesh = meshes[m];
                    combineInstances[m].subMeshIndex = 0;
                }


                // Combine into one
                meshResult.CombineMeshes(combineInstances, false, false);
                MeshUtility.Optimize(meshResult);
				Unwrapping.GenerateSecondaryUVSet(meshResult);

                

                meshFilterCombine.sharedMesh = meshResult;
                EditorUtility.SetDirty(meshFilterCombine);
                // Destroy other meshes
                /*foreach (Mesh mesh in meshes)
                {
                    mesh.Clear();
                    DestroyImmediate(mesh);
                }*/
            }

            // For MeshRenderer
            {
                // Get / Create mesh renderer
                MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
                if (!meshRendererCombine)
                    meshRendererCombine = gameObject.AddComponent<MeshRenderer>();

                // Assign materials
                Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
                meshRendererCombine.materials = materialsArray;
            }


            /*CombinedMesh combinedMesh = gameObject.GetComponent<CombinedMesh>();
            if (!combinedMesh)
                combinedMesh = gameObject.AddComponent<CombinedMesh>();
            combinedMesh.target = meshToCombine;*/



            //AssetDatabase.CreateAsset(meshResult, filePath + "/" + fileName);



            Object prefab = PrefabUtility.CreateEmptyPrefab( prefabPath );
            AssetDatabase.AddObjectToAsset(meshResult, prefab);
            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    public void OnWizardCreate()
    {
        Generate( meshToCombine, gameObject );
    }


    static private int Contains (ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++) 
        {
            if (((Material)searchList [i]).name == searchName) 
                return i;
        }
        return -1;
    }
}
 
