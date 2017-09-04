using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;


namespace Bootstrap
{


    

    public class MayaToUnity : EditorWindow
    {

        //Drawer drawer;
        //DoorRotate doorRotate;
        //DoorRotateSync doorRotateSync;
        //ItemLaunch draggingLaunch;
        //Trash trash;
        //Basket basket;
    
        [System.Serializable]
        public class MayaObject
        {
            public string name;
            public Vector3 position;
            public Vector3 rotation;
            public MayaObject[] children;
        }

        [System.Serializable]
        public struct PrefixToBehaviour
        {
            public string prefix;
            public string className;
        }

        [System.Serializable]
        public class PrefixGroup
        {
            public List<PrefixToBehaviour> prefix = new List<PrefixToBehaviour>();

            public bool HasPrefix( string prefixName )
            {
                foreach (PrefixToBehaviour p in prefix)
                    if (p.prefix == prefixName)
                        return true;
                return false;
            }
        }


        [MenuItem("Bootstrap/Window/MayaToUnity")]
        private static void ShowMyWindow()
        {
            EditorWindow.CreateInstance<MayaToUnity>().Show();
        }


        public static System.Action<List<GameObject>> OnCreatedEvent = delegate { };

        private TextAsset textasset;
        private MayaObject mayaObject;
        private string directory;
        private List<MayaObject> path = new List<MayaObject>();
        private PrefixGroup prefixGrp;
        private List<GameObject> objCreated = new List<GameObject>();

        private string configPath {
            get { return Application.dataPath + "/GameData/mayaToUnity.json"; } }

        private void OnEnable()
        {
            //Debug.Log( EditorPrefs.GetString("MAYA_TO_UNITY_PREFIX", "{\"prefix\":[]}") );
            //prefixGrp = JsonUtility.FromJson<PrefixGroup>( EditorPrefs.GetString("MAYA_TO_UNITY_PREFIX", "{\"prefix\":[]}"));
            string dir = System.IO.Path.GetDirectoryName(configPath);
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            if (!System.IO.File.Exists(configPath)) System.IO.File.WriteAllText(configPath, "{}");

            prefixGrp = JsonUtility.FromJson<PrefixGroup>(System.IO.File.ReadAllText(configPath));

            if ( !prefixGrp.HasPrefix("_BOX") )
                prefixGrp.prefix.Add(new PrefixToBehaviour() { prefix = "_BOX", className = "UnityEngine.BoxCollider" });

            if (!prefixGrp.HasPrefix("_SPHERE"))
                prefixGrp.prefix.Add(new PrefixToBehaviour() { prefix = "_SPHERE", className = "UnityEngine.SphereCollider" });
        }

        private void OnDisable()
        {
            System.IO.File.WriteAllText(configPath, JsonUtility.ToJson(prefixGrp));
            //EditorPrefs.SetString("MAYA_TO_UNITY_PREFIX", JsonUtility.ToJson(prefixGrp));
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            textasset = (TextAsset)EditorGUILayout.ObjectField(textasset, typeof(TextAsset), false);

            if (textasset != null && GUILayout.Button("Apply") )
            {
                mayaObject = JsonUtility.FromJson<MayaObject>(textasset.text);
                string filepath = AssetDatabase.GetAssetPath(textasset);
                directory = System.IO.Directory.GetParent(filepath).FullName;
            
                path.Clear();
                ApplyObject(mayaObject, path);
                OnCreatedEvent(objCreated);
                objCreated.Clear();
            }

            GUIPrefix();
        }

        protected void GUIPrefix()
        {
            int removeIndex = -1;
        
            for( int i = 0 ; i < prefixGrp.prefix.Count ; i++ )
            {
                GUILayout.BeginHorizontal();
                PrefixToBehaviour pf = prefixGrp.prefix[i];
                pf.prefix = GUILayout.TextField(prefixGrp.prefix[i].prefix);
                pf.className = GUILayout.TextField(prefixGrp.prefix[i].className);
                if (GUILayout.Button("x")) removeIndex = i;
                prefixGrp.prefix[i] = pf;
                GUILayout.EndHorizontal();
            }

            if (removeIndex != -1)
                prefixGrp.prefix.RemoveAt(removeIndex);

            if( GUILayout.Button("Add") )
            {
                prefixGrp.prefix.Add(new PrefixToBehaviour() { prefix=string.Empty, className=string.Empty });
            }
        }

        protected void ApplyObject( MayaObject mayaObject, List<MayaObject> path, GameObject parentReference = null )
        {
            GameObject obj = null;

            if ( !string.IsNullOrEmpty(mayaObject.name) )
            {
                // Recuperar objeto existente
                if (parentReference != null)
                {
                    Transform tr = parentReference.transform.FindChild(mayaObject.name);
                    if (tr != null)
                        obj = tr.gameObject;
                }
                else
                    obj = GameObject.Find(mayaObject.name);



                // Caso o objeto nao for encontrado na cena, recriar objeto
                if (obj == null)
                {
                    if (mayaObject.name.Contains("_GO"))
                        obj = CreateByData(mayaObject);
                    else
                        obj = CreateInnerObject(path, mayaObject);

                    if (obj == null)
                        obj = new GameObject(mayaObject.name);

                    obj.isStatic = true;

                    Undo.RegisterCreatedObjectUndo(obj, "Created Obj By Maya");
                }

                // configurar posicao e rotacao do objeto
                Undo.RecordObject(obj.transform, "Transform Position");
                obj.transform.rotation = MayaRotationToUnity(mayaObject.rotation);
                obj.transform.position = mayaObject.position;
                Undo.SetTransformParent(obj.transform, parentReference != null ? parentReference.transform : null, "Organize Hierarchy");
                //obj.transform.parent = parentReference != null ? parentReference.transform : null;

                //adiciona mesh collider para o pai ou para si mesmo
                if ( mayaObject.name.Contains("_COL") )
                {
                    if( mayaObject.name.Contains("_GO") )
                    {
                        MeshCollider col = obj.GetComponent<MeshCollider>();
                        if (col == null) col = parentReference.AddComponent<MeshCollider>();
                        col.sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
                    }
                    else
                    {
                        MeshCollider col = parentReference.GetComponent<MeshCollider>();
                        if (col == null) col = parentReference.AddComponent<MeshCollider>();
                        col.sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
                        DestroyImmediate(obj);
                        return;
                    }
                }

                //add components by prefix
                foreach (PrefixToBehaviour pr in prefixGrp.prefix)
                {
                    if (!string.IsNullOrEmpty(pr.prefix) && mayaObject.name.Contains(pr.prefix))
                    {
                        System.Type tp = GetType(pr.className);
                        if (tp != null)
                        {
                            if (obj.GetComponent(tp) == null)
                                obj.AddComponent(tp);
                        }
                        else
                            Debug.Log("NAO ENCONTREI: " + pr.className);
                    }
                }
            
                //remove renderer
                if (mayaObject.name.Contains("_RR"))
                    obj.GetComponent<Renderer>().enabled = false;

                if (mayaObject.name.Contains("_PREF"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Prefabs/Maya");
                    string prefabPath = "Assets/Prefabs/Maya/" + obj.name + ".prefab";
                    GameObject currentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    GameObject prefab = (currentPrefab != null) ?
                        PrefabUtility.ReplacePrefab(obj, currentPrefab, ReplacePrefabOptions.Default) :
                        PrefabUtility.CreatePrefab(prefabPath, obj);
                    DestroyImmediate(obj);
                    obj = prefab;
                }

                //add to object created
                objCreated.Add(obj);
            }




            // continua recursividade para os filhos
        
            if (mayaObject.children == null) return;

            path.Add(mayaObject);
            foreach (MayaObject mo in mayaObject.children)
                ApplyObject(mo, path, obj);
            path.RemoveAt(path.Count - 1);
        }
    
        protected GameObject CreateInnerObject( List<MayaObject> path, MayaObject obj ) 
        {
            path.Add(obj);
            GameObject reference = GetObjectInnerFBX(path);
            if(reference != null )
            {
                reference = GameObject.Instantiate(reference, obj.position, Quaternion.Euler(obj.rotation));
                reference.name = obj.name;
            }
            path.RemoveAt(path.Count - 1);
            return reference;
        }

        protected GameObject GetObjectInnerFBX( List<MayaObject> path )
        {
            //encontro no caminho o ultimo elemento que e um fbx

            int fbxParentIndex = path.Count - 1;
            for (; fbxParentIndex >= 0 ; fbxParentIndex--)
            { 
                try
                {

                    if ( !string.IsNullOrEmpty(path[fbxParentIndex].name) && path[fbxParentIndex].name.Contains("_GO"))
                        break;
                }
                catch(System.Exception e)
                {
                    Debug.Log("Error");
                }
            }

            //se nao obtiver resultado, o caminho nao contem um fbx
            if (fbxParentIndex == -1)
                return null;

            //recupero a referencia do fbx
            Transform fbx = GetReference(path[fbxParentIndex].name).transform;
        
            //caminho dentro da hierarquia do fbx
            for (++fbxParentIndex ; fbxParentIndex < path.Count && fbx != null ; fbxParentIndex++)
                fbx = fbx.transform.FindChild(path[fbxParentIndex].name);

            //retorno o objeto encontrado
            return fbx != null ? fbx.gameObject : null;
        }

        protected GameObject CreateByData( MayaObject data )
        {
            GameObject reference = GetReference(data.name);
            GameObject result = GameObject.Instantiate(reference, data.position, Quaternion.Euler(data.rotation));
            result.name = data.name;
            return result;
        }

        protected GameObject GetReference( string objName )
        {
            string fbxName = objName.Substring(0, objName.IndexOf("_GO"));

            string file = System.IO.Path.Combine(directory, fbxName + ".fbx");
            file = file.Substring(file.LastIndexOf("Assets"));
            return (GameObject)AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
        }

        private Quaternion MayaRotationToUnity( Vector3 rotation )
        {
            Vector3 flippedRotation = new Vector3(rotation.x, -rotation.y, -rotation.z);
            // convert XYZ to ZYX
            var qx = Quaternion.AngleAxis(flippedRotation.x, Vector3.right);
            var qy = Quaternion.AngleAxis(flippedRotation.y, Vector3.up);
            var qz = Quaternion.AngleAxis(flippedRotation.z, Vector3.forward);
            var qq = qz * qy * qx; // this is the order
            return qq;
        }





        public static Type GetType( string TypeName )
        {

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            var type = Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (TypeName.Contains("."))
            {

                // Get the name of the assembly (Assumption is that we are using 
                // fully-qualified type names)
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                // Attempt to load the indicated Assembly
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;

                // Ask that assembly to return the proper Type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;

            }

            // If we still haven't found the proper type, we can enumerate all of the 
            // loaded assemblies and see if any of them define the type
            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {

                // Load the referenced assembly
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    // See if that assembly defines the named type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }

            // The type just couldn't be found...
            return null;

        }



    }



}