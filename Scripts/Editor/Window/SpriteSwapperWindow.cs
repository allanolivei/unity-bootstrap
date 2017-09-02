using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace Bootstrap
{


    public class SpriteSwapperWindow : EditorWindow
    {
        [MenuItem("Bootstrap/Window/Sprite Swapper")]
        public static void WindowMenu()
        {
            EditorWindow win = EditorWindow.GetWindow<SpriteSwapperWindow>("Sprite Swap");
            win.Show();
        }

        public struct Swap
        {
            public Sprite spriteOld;
            public Sprite spriteNew;
        }

        public struct SpriteGroup
        {
            public Texture2D oldAtlas;
            public Texture2D newAtlas;
            public List<Swap> swaps;
        }

        public List<SpriteGroup> groups = new List<SpriteGroup>();
        public SpriteGroup swaps;

        public SerializedObject so;
        public Vector2 scrollbar;

        public void OnEnable()
        {
            if (swaps.swaps == null) swaps.swaps = new List<Swap>();
        }


        public void OnGUI()
        {
            scrollbar = EditorGUILayout.BeginScrollView(scrollbar);

            GUILayout.Box("Atlas completo para troca", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            int removeIndex = -1;

            for (int i = 0 ; i < groups.Count ; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("x")) removeIndex = i;
                GUILayout.EndHorizontal();

                SpriteGroup grp = groups[i];

                Texture2D oldAtlas = grp.oldAtlas;
                Texture2D resultOldAtlas = (Texture2D)EditorGUILayout.ObjectField("Atlas Antigo", oldAtlas, typeof(Texture2D), false);

                Texture2D newAtlas = grp.newAtlas;
                Texture2D resultNewAtlas = (Texture2D)EditorGUILayout.ObjectField("Atlas Novo", newAtlas, typeof(Texture2D), false);

                if (oldAtlas != resultOldAtlas || newAtlas != resultNewAtlas)
                {
                    grp.oldAtlas = resultOldAtlas;
                    grp.newAtlas = resultNewAtlas;
                    grp.swaps = GetSwapsByAtlas(resultOldAtlas, resultNewAtlas);
                }

                DrawSwapGroup(grp);

                groups[i] = grp;

                GUILayout.EndVertical();

                GUILayout.Space(20);
            }

            if (removeIndex != -1) groups.RemoveAt(removeIndex);

            if (GUILayout.Button("Adicionar Atlas"))
                groups.Add(new SpriteGroup());


            GUILayout.Space(40);

            GUILayout.Box("Sprites Aleatorios para troca", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            if (swaps.swaps.Count > 0)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                DrawSwapGroup(swaps);
                GUILayout.EndVertical();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Adicionar Sprite"))
                swaps.swaps.Add(new Swap());

            EditorGUILayout.EndScrollView();
        }

        public List<Swap> GetSwapsByAtlas( Texture2D oldAtlas, Texture2D newAtlas )
        {
            if (oldAtlas == null) return null;

            List<Swap> result = new List<Swap>();

            if (oldAtlas == null) return result;

            string spriteSheet = AssetDatabase.GetAssetPath(oldAtlas);
            Sprite[] oldSprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();

            Sprite[] newSprites = null;
            if (newAtlas)
            {
                spriteSheet = AssetDatabase.GetAssetPath(newAtlas);
                newSprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
            }

            for (int j = 0 ; j < oldSprites.Length ; j++)
            {
                Sprite spriteNew = newAtlas != null ? FindSpriteByName(newSprites, oldSprites[j].name) : null;
                result.Add(new Swap() { spriteOld = oldSprites[j], spriteNew = spriteNew });
            }

            return result;
        }

        public Sprite FindSpriteByName( Sprite[] sprites, string name )
        {
            for (int i = 0 ; i < sprites.Length ; i++)
                if (sprites[i].name == name) return sprites[i];
            return null;
        }

        public void DrawSwapGroup( SpriteGroup grp )
        {
            if (grp.swaps == null) grp.swaps = new List<Swap>();

            if (grp.swaps.Count == 0) return;

            // GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sprite Antigo", EditorStyles.boldLabel, GUILayout.Width((this.position.width - 40) * 0.5f));
            GUILayout.Label("Sprite Novo", EditorStyles.boldLabel, GUILayout.Width((this.position.width - 40) * 0.5f));
            GUILayout.EndHorizontal();

            int removeIndex = -1;

            for (int i = 0 ; i < grp.swaps.Count ; i++)
            {
                EditorGUILayout.BeginHorizontal();

                Swap swap = grp.swaps[i];

                swap.spriteOld = (Sprite)EditorGUILayout.ObjectField(swap.spriteOld, typeof(Sprite), true);
                swap.spriteNew = (Sprite)EditorGUILayout.ObjectField(swap.spriteNew, typeof(Sprite), true);
                if (GUILayout.Button("-", GUILayout.Width(20))) removeIndex = i;

                grp.swaps[i] = swap;

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Trocar", GUILayout.Width(50))) ApplySwap(grp);
            GUILayout.EndHorizontal();
            //GUILayout.EndVertical();

            if (removeIndex != -1) grp.swaps.RemoveAt(removeIndex);

        }

        public void ApplySwap( SpriteGroup grp )
        {
            //Sprite Renderer
            SpriteRenderer[] sr = FindObjectsOfType<SpriteRenderer>();
            Undo.RecordObjects(sr, "Swap Sprites");
            foreach (SpriteRenderer r in sr)
            {
                for (int i = 0 ; i < grp.swaps.Count ; i++)
                    if (r.sprite != null && grp.swaps[i].spriteNew != null &&
                        grp.swaps[i].spriteOld != null && r.sprite.name == grp.swaps[i].spriteOld.name)
                        r.sprite = grp.swaps[i].spriteNew;
            }

        }

    }

}