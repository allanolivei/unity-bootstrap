using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ImageToChannel : EditorWindow
{
    [MenuItem("Window/ImageToChannel")]
    public static void CreateWindow()
    {
        EditorWindow.GetWindow<ImageToChannel>();
    }

    public Texture2D red;
    public Texture2D green;
    public Texture2D blue;
    public Texture2D alpha;
    public string fileName = "TextureMultiChannel";

    void OnGUI()
    {
        red = (Texture2D)EditorGUILayout.ObjectField("Red", red, typeof(Texture2D), true);
        green = (Texture2D)EditorGUILayout.ObjectField("Green", green, typeof(Texture2D), true);
        blue = (Texture2D)EditorGUILayout.ObjectField("Blue", blue, typeof(Texture2D), true);
        alpha = (Texture2D)EditorGUILayout.ObjectField("Alpha", alpha, typeof(Texture2D), true);
        fileName = GUILayout.TextField(fileName);

        EditorGUI.BeginDisabledGroup( red == null || green == null || blue == null || alpha == null || string.IsNullOrEmpty(fileName) );
        if( GUILayout.Button("Generate") )
            Generate(red, green, blue, alpha, fileName);
        EditorGUI.EndDisabledGroup();

    }

    public static void Generate( Texture2D red, Texture2D green, Texture2D blue, Texture2D alpha, string fileName )
    {
        int width = 0, height = 0;
        Texture2D[] channels = new Texture2D[] { red, green, blue, alpha };

        for (int i = 0; i < channels.Length; i++)
        {
            if (channels[i])
            {
                SetupTexture(channels[i]);
                if (channels[i].width > width) width = channels[i].width;
                if (channels[i].height > height) height = channels[i].height;
            }
        }

        int length = width * height;

        Color32[][] r = new Color32[][] {
                red?red.GetPixels32():new Color32[length],
                green?green.GetPixels32():new Color32[length],
                blue?blue.GetPixels32():new Color32[length],
                alpha?alpha.GetPixels32():new Color32[length],
            };

        Color32[] resultColor = new Color32[length];

        for (int i = 0; i < length; i++)
            resultColor[i] = new Color32(r[0][i].r, r[1][i].r, r[2][i].r, r[3][i].r);

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.SetPixels32(resultColor);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);

        // File generated
        string path = GetSelectedPathOrFallback() + "/" + fileName + ".png";
        File.WriteAllBytes(path, bytes);
        
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        Texture2D imgResult = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        Selection.activeObject = imgResult;
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    //configuracao basica para gerar o atlas
    public static Texture2D SetupTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter texImp = (TextureImporter)AssetImporter.GetAtPath(path);
        texImp.isReadable = true;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        return texture;
    }

    public static string GetSelectedPathOrFallback()
    {
        string selectedPath = GetCommonDirectory(Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets));
        string path = string.IsNullOrEmpty(selectedPath) ? "Assets" : selectedPath;

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
            path = Path.GetDirectoryName(path);

        return path;
    }

    public static string GetCommonDirectory( UnityEngine.Object[] obs )
    {
        string result = string.Empty;
        foreach (UnityEngine.Object obj in obs)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(result))
                result = path;
            else
            {
                while (path.Length > 0 && !path.Contains(result))
                {
                    result = Directory.GetParent(result).ToString();
                }
            }
        }

        return result;
    }



}
