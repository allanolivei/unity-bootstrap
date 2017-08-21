using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AtlasGenerator
{

    public static string atlasName = "atlas.png";

    //verifica se tem alguma imagem selecionada para deixar o menu habilitado
    [MenuItem("Assets/Generate Atlas[atlas.png] %#c", true)]
    static bool Validate()
    {
        Object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets | SelectionMode.DeepAssets);
        return objects.Length > 0;
    }

    //inseri no menu superior "Assets" a opcao de gerar um novo atlas das imagens selecionadas( Ctrl+Shift+C = %#c ) na raiz da pasta "Assets"
    [MenuItem("Assets/Generate Atlas[atlas.png] %#c", false, -200)]
    public static void Generate()
    {
        //recupera todos as texturas selecionadas, inclusive aquelas que estao dentro das pastas selecionadas
        Object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets | SelectionMode.DeepAssets);

        //se não obtiver nenhuma texture retorna uma mensagem
        if (objects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No images selected", "Ok");
            return;
        }

        //configura objetos
        Texture2D[] textures = SetupTextures(objects);

        //gera o atlas na raiz da pasta assets
        GenerateImageAtlas(textures, GetSelectedPathOrFallback() + "/atlas.png");
    }

    public static void Generate( string path )
    {

        //recupera todos as texturas selecionadas, inclusive aquelas que estao dentro das pastas selecionadas
        //Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        //Debug.Log("CARREGAR IMAGENS: " + path+": "+objects.Length);

        List<Object> objects = new List<Object>();

        string[] files = AssetDatabase.FindAssets("t:texture2D", new[] { path });

        Debug.Log("FIND FILES: "+path+": "+files.Length);

        foreach( string guid in files )
        {
            //string file = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log("GET FILE: " + AssetDatabase.GUIDToAssetPath(guid));
            Object ob = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Texture2D));
            if (ob != null) objects.Add(ob);
        }

        //se não obtiver nenhuma texture retorna uma mensagem
        if (objects.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No images selected", "Ok");
            return;
        }

        //configura objetos
        Texture2D[] textures = SetupTextures( objects.ToArray() );

        //gera o atlas na raiz da pasta assets
        GenerateImageAtlas(textures, GetSelectedPathOrFallback() + "/atlas.png");
    }

    public static Texture2D[] SetupTextures( Object[] objects )
    {
        //converte a lista de objetos e configura 
        List<Texture2D> textures = new List<Texture2D>();
        for (int i = 0; i < objects.Length; i++)
        {
            Texture2D tex = objects[i] as Texture2D;
            if (!tex.name.Contains("atlas"))
            {
                textures.Add(AtlasGenerator.SetupTexture(tex));
            }
        }

        return textures.ToArray();
    }

    //configuracao basica para gerar o atlas
    public static Texture2D SetupTexture(Texture2D texture)
    {
        //recupera os parametros de importação do objeto
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter texImp = (TextureImporter)AssetImporter.GetAtPath(path);
        //texImp.textureType = TextureImporterType.Advanced;
        //Para gerar o atlas é necessario que as imagens permitam a leitura e tenha o formato ARGB32
        texImp.isReadable = true;
        texImp.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        texImp.npotScale = TextureImporterNPOTScale.None;
        //atualiza as configuracoes
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        //returna a textura configurada
        return texture;
    }

    //gera e configura o atlas
    public static void GenerateImageAtlas(Texture2D[] textures, string path)
    {
        /** 
         * GENERATE ATLAS
         **/
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        Rect[] uvs = texture.PackTextures(textures, 4, 4096);
        bool isNew = !File.Exists(path);

        //se for uma nova imagem salvar para recuperar o textureimporter
        if (isNew) texture = SaveAtlas(path, texture);

        /**
         * GENERATE SPRITESHEET DATA
         **/
        TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
        float w = texture.width;
        float h = texture.height;
        int count = 0;
        SpriteMetaData[] spritesheet;

        //modificar todos os sprite meta da que ja existem
        if (imp.spritesheet != null)
        {
            spritesheet = imp.spritesheet;
            for (int i = 0; i < spritesheet.Length; i++)
            {
                string name = spritesheet[i].name;
                bool exist = false;
                for (int j = 0; j < textures.Length; j++)
                {
                    //verifica se ja contém a texture para modificar
                    if (textures[j].name+"_atl" == name)
                    {
                        //converte uv para pixels
                        Rect uv = uvs[j];
                        uv.x *= w; uv.y *= h;
                        uv.width *= w; uv.height *= h;


                        TextureImporter im = AssetImporter.GetAtPath( AssetDatabase.GetAssetPath(textures[j]) ) as TextureImporter;
                        if( im.spriteImportMode == SpriteImportMode.Single )
                        { 
                            spritesheet[i].border = im.spriteBorder;
                            spritesheet[i].pivot = im.spritePivot;
                        }
                        

                        spritesheet[i].rect = uv;

                        count++;
                        exist = true;
                        continue;
                    }
                }

                //a textura nao foi selecionada, mas mesmo assim mantem o espaco, caso posteriormente queira inseri-la novamente
                if (!exist) spritesheet[i].rect = new Rect(0, 0, 5, 5);
            }
        }
        else spritesheet = new SpriteMetaData[0]; //primeira vez

        //redimensiona para inserir novos sprites
        count = textures.Length - count;
        if (count > 0) System.Array.Resize<SpriteMetaData>(ref spritesheet, spritesheet.Length + count);

        //verificar se falta algum sprite ser inserido
        for (int i = 0; i < textures.Length; i++)
        {
            string name = textures[i].name+"_atl";
            bool exist = false;
            for (int j = 0; j < spritesheet.Length; j++)
            {
                if (spritesheet[j].name == name) { exist = true; continue; }
            }

            //se o sprite ainda nao estiver no spritesheet cria-lo
            if (!exist)
            {
                //converte uv para pixels
                Rect uv = uvs[i];
                uv.x *= w; uv.y *= h;
                uv.width *= w; uv.height *= h;

                TextureImporter im = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(textures[i])) as TextureImporter;

                SpriteMetaData sprite = new SpriteMetaData();
                sprite.name = name;


                sprite.border = im.spriteBorder;
                sprite.pivot = im.spritePivot;
                //Debug.Log(textures[i].name + ":::" + im.spriteBorder + "___" + im.spritePivot);

                sprite.rect = uv;
                spritesheet[spritesheet.Length - count] = sprite;
                count--;
            }
        }

        //configuracoes basicas para o atlas
        imp.spritesheet = spritesheet;
        imp.isReadable = true;
        imp.mipmapEnabled = true;
        imp.alphaIsTransparency = true;
        imp.wrapMode = TextureWrapMode.Clamp;
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Multiple;
        imp.npotScale = TextureImporterNPOTScale.None;

        /**
         * Apply texture configurations
         **/
        if (!isNew) texture = SaveAtlas(path, texture);
        else AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Selection.activeObject = texture;
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    static public Texture2D SaveAtlas(string path, Texture2D texture)
    {
        System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(path);

        return (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
    }

    static public string GetSelectedPathOrFallback()
    {
        //string path = "Assets";



        /*Debug.Log("teste1: " + Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets));
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }*/

        string selectedPath = GetCommonDirectory(Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets));
        string path = string.IsNullOrEmpty(selectedPath) ? "Assets" : selectedPath;

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
            path = Path.GetDirectoryName(path);

        return path;
    }

    static public string GetCommonDirectory( UnityEngine.Object[] obs )
    {
        string result = string.Empty;
        foreach (UnityEngine.Object obj in obs)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(result)) result = path;
            else
            {
                while( path.Length > 0 && !path.Contains(result) )
                {
                    result = Directory.GetParent(result).ToString();
                }
            }
        }

        return result;
    }



}