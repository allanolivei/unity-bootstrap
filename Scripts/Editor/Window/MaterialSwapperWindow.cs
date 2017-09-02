using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bootstrap
{


    public class MaterialSwapperWindow : EditorWindow
    {

        private const string SAVE_KEY = "overrideMaterialCache";

        [MenuItem("Bootstrap/Window/Material Swapper")]
        private static void RequireWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<MaterialSwapperWindow>("Material Swap");
            window.Show();
        }

        [System.Serializable]
        public class SwapData
        {
            public bool toggle;
            public string current;
            public string next;
        }

        [System.Serializable]
        public class Swap
        {
            public bool toggle;
            public Material current;
            public Material next;

            public Swap()
            { }

            public Swap( SwapData data )
            {
                this.Deserialize(data);
            }

            public void Deserialize( SwapData data )
            {
                if (!string.IsNullOrEmpty(data.current))
                    this.current = AssetDatabase.LoadAssetAtPath<Material>(data.current);
                if (!string.IsNullOrEmpty(data.next))
                    this.next = AssetDatabase.LoadAssetAtPath<Material>(data.next);
                this.toggle = data.toggle;
            }

            public SwapData Serialize()
            {
                return new SwapData()
                {
                    toggle = this.toggle,
                    current = this.current == null ? string.Empty : AssetDatabase.GetAssetPath(this.current),
                    next = this.next == null ? string.Empty : AssetDatabase.GetAssetPath(this.next)
                };
            }
        }

        [System.Serializable]
        public class SwapGroupData
        {
            public SwapData[] swaps;
        }


        private List<Swap> swaps = new List<Swap>();
        private Vector2 scrollbar;

        private void OnEnable()
        {
            this.Load();
        }

        private void OnDisable()
        {
            this.Save();
        }

        private void OnGUI()
        {
            scrollbar = EditorGUILayout.BeginScrollView(scrollbar);

            //if (GUILayout.Button("Find All Materials in Scene"))
            //    this.FindAllMaterialsInScene();


            Object[] result = this.GUIDragAndDropArea("Drop Material", DragIsValid);
            if (result != null)
            {
                for (int i = 0 ; i < result.Length ; i++)
                {
                    Material mat = result[i] as Material;
                    if (mat == null)
                        continue;
                    if (IndexOfSwap(mat) == -1)
                        swaps.Add(new Swap() { current = mat, toggle = true });
                }
            }

            int deleteIndex = -1;


            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Empty, EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("scene", EditorStyles.toolbarButton, GUILayout.Width(50)))
                this.FindAllMaterialsInScene();
            GUILayout.EndHorizontal();

            for (int i = 0 ; i < swaps.Count ; i++)
            {
                EditorGUILayout.BeginHorizontal();

                swaps[i].toggle = EditorGUILayout.Toggle(swaps[i].toggle, GUILayout.Width(20));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(swaps[i].current, typeof(Material), true, GUILayout.Width(position.width * 0.5f));
                EditorGUI.EndDisabledGroup();
                swaps[i].next = (Material)EditorGUILayout.ObjectField(swaps[i].next, typeof(Material), true);
                if (swaps[i].next != null && GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20)))
                    swaps[i].next = null;
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                    deleteIndex = i;

                EditorGUILayout.EndHorizontal();
            }

            if (deleteIndex != -1)
            {
                swaps.RemoveAt(deleteIndex);
                this.Save();
            }

            if (IsValidToSwap() && GUILayout.Button("Swap"))
            {
                this.ApplySwap();
                this.Save();
            }

            EditorGUILayout.EndScrollView();

        }

        private bool IsValidToSwap()
        {
            for (int i = 0 ; i < swaps.Count ; i++)
                if (swaps[i].toggle && swaps[i].current && swaps[i].next)
                    return true;
            return false;
        }

        private bool DragIsValid( Object[] obs )
        {
            foreach (UnityEngine.Object ob in obs)
                if (ob is Material)
                    return true;
            return false;
        }

        private Object[] GUIDragAndDropArea( string label, System.Func<Object[], bool> IsValid )
        {
            GUIStyle st = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box;
            st.alignment = TextAnchor.MiddleCenter;
            GUILayout.Box(label, st, GUILayout.Height(100), GUILayout.ExpandWidth(true));

            if (Event.current.mousePosition.y > 0 && Event.current.mousePosition.y < 100)
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:

                        if (IsValid(DragAndDrop.objectReferences))
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        else
                            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                        Event.current.Use();
                        break;
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                        return DragAndDrop.objectReferences;
                }
            }

            return null;
        }

        private int IndexOfSwap( Material current )
        {
            for (int i = 0 ; i < swaps.Count ; i++)
                if (swaps[i].current == current)
                    return i;

            return -1;
        }

        private int IndexOfSwap( Material current, Material next )
        {
            for (int i = 0 ; i < swaps.Count ; i++)
                if (swaps[i].current == current && swaps[i].next == next)
                    return i;

            return -1;
        }

        private void Save()
        {
            SwapGroupData data = new SwapGroupData();
            data.swaps = new SwapData[swaps.Count];
            for (int i = 0 ; i < swaps.Count ; i++)
                data.swaps[i] = swaps[i].Serialize();
            EditorPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(data));
        }

        private void Load()
        {
            swaps.Clear();
            SwapGroupData data = JsonUtility.FromJson<SwapGroupData>(EditorPrefs.GetString(SAVE_KEY, "{}"));
            if (data.swaps == null)
                return;
            for (int i = 0 ; i < data.swaps.Length ; i++)
                if (data.swaps[i] != null)
                    swaps.Add(new Swap(data.swaps[i]));
        }

        /// <summary>
        /// find all materials in scene and disable other materials
        /// </summary>
        private void FindAllMaterialsInScene()
        {
            for (int i = 0 ; i < swaps.Count ; i++)
                swaps[i].toggle = false;

            Renderer[] rends = FindObjectsOfType<Renderer>();
            foreach (Renderer rend in rends)
            {
                Material[] mats = rend.sharedMaterials;
                foreach (Material mat in mats)
                {
                    int index = IndexOfSwap(mat);
                    if (index == -1)
                        swaps.Add(new Swap() { current = mat, toggle = true });
                    else
                        swaps[index].toggle = true;
                }
            }
        }

        private void ApplySwap()
        {
            Renderer[] rends = FindObjectsOfType<Renderer>();

            for (int i = 0 ; i < swaps.Count ; i++)
            {
                if (!swaps[i].toggle || swaps[i].next == null || swaps[i].next == swaps[i].current)
                    continue;

                foreach (Renderer rend in rends)
                {
                    Material[] mats = rend.sharedMaterials;
                    for (int m = 0 ; m < mats.Length ; m++)
                        if (swaps[i].current == mats[m])
                            mats[m] = swaps[i].next;
                    rend.sharedMaterials = mats;
                }
            }
        }

    }



}