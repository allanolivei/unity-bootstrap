using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Bootstrap
{


    public class BuildingToolWindow : EditorWindow
    {
        private const string KEY_SCENE_ENABLED = "buildSceneEnabled";
        private const string KEY_PLATFORM_ENABLED = "buildPlatformEnabled";
        private const string KEY_AUTO_RUN = "buildAutoRun";

        private static Dictionary<BuildTarget, string> BUILDTARGET_TO_PATH = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.Android, "Android" },
            { BuildTarget.StandaloneOSXUniversal, "Mac" },
            { BuildTarget.StandaloneLinuxUniversal, "Linux" },
            { BuildTarget.StandaloneWindows, "Win" }
        };

        private static Dictionary<BuildTarget, string> BUILDTARGET_TO_EXT = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.Android, ".apk" },
            { BuildTarget.StandaloneOSXUniversal, ".app" },
            { BuildTarget.StandaloneLinuxUniversal, ".x86" },
            { BuildTarget.StandaloneWindows, ".exe" }
        };

        [MenuItem("Bootstrap/Window/Building Tool")]
        private static void ShowBuildingTool()
        {
            EditorWindow.GetWindow<BuildingToolWindow>("Building Tool").Show();
        }

        private class SceneData
        {
            public string path;
            public bool toggle;
        }

        private class BuildTargetData
        {
            public BuildTarget target;
            public bool toggle;
        }

        private List<BuildTargetData> platforms = new List<BuildTargetData>();
        private List<SceneData> scenes = new List<SceneData>();
        private bool autoRun = false;

        private void OnEnable()
        {
            EditorPrefs.DeleteKey(KEY_PLATFORM_ENABLED);
            EditorPrefs.DeleteKey(KEY_SCENE_ENABLED);

            this.autoRun = EditorPrefs.GetBool(KEY_AUTO_RUN, true);
            this.LoadScenes();
            this.LoadBuildTarget();
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(KEY_AUTO_RUN, this.autoRun);
            this.SaveScenes();
            this.SaveBuildTarget();
        }

        private void OnGUI()
        {
            GUILayout.Box("Descriptions", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            //GUILayout.Label("Descriptions", EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);
            PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);
            EditorUserBuildSettings.allowDebugging =
            EditorUserBuildSettings.development = EditorGUILayout.Toggle("Is Debug", EditorUserBuildSettings.allowDebugging);


            //BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            //buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
            //buildPlayerOptions.locationPathName = "Build/Test/Mac/Nefron.app";
            //buildPlayerOptions.target = BuildTarget.StandaloneOSXUniversal;
            //buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode;
            

            GUILayout.Space(30);
            this.GUIScenes();

            GUILayout.Space(30);
            this.GUIBuildTarget();

            if( this.HasAndroidBuildTarget() )
            {
                EditorGUILayout.LabelField("Android Setup");
                PlayerSettings.virtualRealitySupported = EditorGUILayout.Toggle("VR", PlayerSettings.virtualRealitySupported);
                PlayerSettings.Android.androidIsGame = true;
                PlayerSettings.Android.androidTVCompatibility = false;
                PlayerSettings.Android.bundleVersionCode = 1;
                PlayerSettings.Android.minSdkVersion = PlayerSettings.virtualRealitySupported ? AndroidSdkVersions.AndroidApiLevel23 : AndroidSdkVersions.AndroidApiLevel16;
                
                //PlayerSettings.Android.keystorePass = "123456a";
                //PlayerSettings.Android.keyaliasPass = "123456a";
            }

            GUILayout.Space(30);
            autoRun = GUILayout.Toggle(autoRun, "Auto Run Last Element");
            if( GUILayout.Button("Build") )
            { 
                string result = this.Build();
                EditorUtility.DisplayDialog("Build Result", result, "ok");
            }

            //string result = BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        private void GUIScenes()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("Scenes", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("By Settings", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                string[] inSettings = GetScenesInSettings();
                for (int i = 0 ; i < scenes.Count ; i++)
                    scenes[i].toggle = System.Array.IndexOf(inSettings, scenes[i].path) != -1;
            }
            if (GUILayout.Button("Current", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                for (int i = 0 ; i < scenes.Count ; i++)
                    scenes[i].toggle = scenes[i].path == SceneManager.GetActiveScene().path;
            }
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                this.SaveScenes(); 
                this.LoadScenes();
            }
            GUILayout.EndHorizontal();
            
            for (int i = 0 ; i < scenes.Count ; i++)
            {
                GUILayout.BeginHorizontal();

                scenes[i].toggle = EditorGUILayout.Toggle(scenes[i].toggle, GUILayout.Width(18));
                GUILayout.Box(scenes[i].path, EditorStyles.textField, GUILayout.ExpandWidth(true));

                GUILayout.EndHorizontal();
            }
        }

        private void GUIBuildTarget()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("Build Target Queue", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("current", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                int index = IndexOfPlatform(EditorUserBuildSettings.activeBuildTarget);
                if (index == -1) platforms.Add(new BuildTargetData() { target = EditorUserBuildSettings.activeBuildTarget });
                for (int i = 0 ; i < platforms.Count ; i++)
                    platforms[i].toggle = platforms[i].target == EditorUserBuildSettings.activeBuildTarget;
            }
            GUILayout.EndHorizontal();
            
            int removeIndex = -1;
            int upIndex = -1;
            for (int i = 0 ; i < platforms.Count ; i++)
            {
                GUILayout.BeginHorizontal();

                platforms[i].toggle = EditorGUILayout.Toggle(platforms[i].toggle, GUILayout.Width(20));
                platforms[i].target = (BuildTarget)EditorGUILayout.EnumPopup(platforms[i].target);
                
                if (GUILayout.Button("^", EditorStyles.miniButton, GUILayout.Width(18))) upIndex = i;
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(18))) removeIndex = i;

                GUILayout.EndHorizontal();
            }

            if (removeIndex != -1) platforms.RemoveAt(removeIndex);
            if( upIndex != -1 )
            {
                int previousIndex = upIndex - 1 < 0 ? platforms.Count+(upIndex-1) : (upIndex - 1) % platforms.Count;
                BuildTargetData data = platforms[upIndex];
                platforms[upIndex] = platforms[previousIndex];
                platforms[previousIndex] = data;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(18)))
                platforms.Add( new BuildTargetData() { target = BuildTarget.NoTarget } );
            GUILayout.EndHorizontal();
        }

        private string Build()
        {
            string result = string.Empty;

            List<string> scenesResult = new List<string>();
            for (int i = 0 ; i < scenes.Count ; i++)
                if (scenes[i].toggle)
                    scenesResult.Add(scenes[i].path);

            int lastValid = -1;
            for (lastValid = platforms.Count - 1 ; lastValid >= 0 ; lastValid--)
                if ( platforms[lastValid].target != BuildTarget.NoTarget && platforms[lastValid].toggle ) break;


            for ( int i = 0 ; i < platforms.Count ; i++ )
            {
                if (platforms[i].target == BuildTarget.NoTarget ||
                    !platforms[i].toggle) continue;

                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = scenesResult.ToArray();
                buildPlayerOptions.locationPathName = GetFolderToBuild(platforms[i].target);//"Build/"++"/Mac/Nefron.app";
                buildPlayerOptions.target = platforms[i].target;
                if (EditorUserBuildSettings.development)
                    buildPlayerOptions.options = BuildOptions.AllowDebugging | BuildOptions.Development;
                else
                    buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode;
                
                if( i == lastValid && autoRun)
                { 
                    buildPlayerOptions.options = buildPlayerOptions.options | BuildOptions.AutoRunPlayer;
                }

                string platformResult = BuildPipeline.BuildPlayer(buildPlayerOptions);

                if (string.IsNullOrEmpty(platformResult))
                    result += platforms[i].target.ToString()+" SUCESSO!\n";
                else
                    result += platforms[i].target.ToString()+" "+platformResult+ " \n";
            }

            return result;
        }

        private static string GetFolderToBuild( BuildTarget target )
        {
            string p = target.ToString();
            BUILDTARGET_TO_PATH.TryGetValue(target, out p);
            string ext = "";
            BUILDTARGET_TO_EXT.TryGetValue(target, out ext);
            return "Build/" + p + "/"+PlayerSettings.productName+
                (target.Equals(BuildTarget.Android) && PlayerSettings.virtualRealitySupported ? "VR" : "") + ext;
        }

        private bool HasAndroidBuildTarget()
        {
            for (int i = 0 ; i < platforms.Count ; i++)
                if (platforms[i].target == BuildTarget.Android)
                    return true;
            return false;
        }

        private void SaveScenes()
        {
            string result = string.Empty;
            for (int i = 0 ; i < scenes.Count ; i++)
                if (scenes[i].toggle)
                    result += scenes[i].path + "*";
            EditorPrefs.SetString(KEY_SCENE_ENABLED, result);
        }

        private int IndexOfPlatform( BuildTarget target )
        {
            for (int i = 0 ; i < platforms.Count ; i++)
                if (platforms[i].target == target)
                    return i;
            return -1;
        }

        private void LoadScenes()
        {
            string[] allScenes = FindAllScenes();
            string[] enabledScenes =
                EditorPrefs.HasKey(KEY_SCENE_ENABLED) ?
                    EditorPrefs.GetString(KEY_SCENE_ENABLED, string.Empty).Split('*') :
                    new string[] { SceneManager.GetActiveScene().path };
            
            scenes.Clear();
            for (int i = 0 ; i < allScenes.Length ; i++)
            {
                scenes.Add(new SceneData()
                {
                    path = allScenes[i],
                    toggle = (System.Array.IndexOf(enabledScenes, allScenes[i]) != -1)
                });
            }
        }

        private void LoadBuildTarget()
        {
            string[] parts = EditorPrefs.GetString(KEY_PLATFORM_ENABLED, string.Empty).Split('*');
            platforms.Clear();
            for( int i = 0 ; i < parts.Length ; i++ )
            {
                string[] r = parts[i].Split('_');
                if (r.Length != 2) continue;

                platforms.Add(new BuildTargetData()
                {
                    target = (BuildTarget)System.Convert.ToInt32(r[0]),
                    toggle = r[1] == "1"
                });
            }

            if( platforms.Count == 0 )
                platforms.Add(new BuildTargetData() { target = EditorUserBuildSettings.activeBuildTarget, toggle=true });
        }

        private void SaveBuildTarget()
        {
            string result = string.Empty;
            for( int i = 0 ; i < platforms.Count ; i++ )
            {
                result += 
                    ((int)platforms[i].target).ToString() + "_" + 
                    (platforms[i].toggle ? "1" : "0") +
                    "*";
            }
            EditorPrefs.SetString(KEY_PLATFORM_ENABLED, result);
        }

        private static string[] FindAllScenes()
        {
            string[] id = AssetDatabase.FindAssets("t:Scene", null);
            string[] scenes = new string[id.Length];

            for (int i = 0 ; i < scenes.Length ; i++)
                scenes[i] = AssetDatabase.GUIDToAssetPath(id[i]);

            return scenes;
        }

        private static string[] GetScenesInSettings()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (var i = 0 ; i < scenes.Length ; i++)
                scenes[i] = EditorBuildSettings.scenes[i].path;
            return scenes;
        }

		[MenuItem("Bootstrap/Build/Test")]
		public static void BuildTest()
		{
		    EditorUserBuildSettings.allowDebugging = true;
		    EditorUserBuildSettings.development = true;

		    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		    buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
			buildPlayerOptions.locationPathName = GetFolderToBuild(EditorUserBuildSettings.activeBuildTarget);
			buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
			buildPlayerOptions.options = BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.AutoRunPlayer;
		    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

		    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Complete Success" : result, "ok");
		}

        //[MenuItem("Bootstrap/Build/Test/Mac")]
        //public static void BuildTestMac()
        //{
        //    BuildConfig();

        //    EditorUserBuildSettings.allowDebugging = false;
        //    EditorUserBuildSettings.development = false;

        //    PlayerSettings.companyName = "Labtime";
        //    PlayerSettings.productName = "Nefron";
        //    PlayerSettings.virtualRealitySupported = false;

        //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //    buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
        //    buildPlayerOptions.locationPathName = "Build/Test/Mac/Nefron.app";
        //    buildPlayerOptions.target = BuildTarget.StandaloneOSXUniversal;
        //    buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode;
        //    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        //    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Mac Complete Success" : result, "ok");
        //}

        ////[MenuItem("Bootstrap/Build/Test/VR")]
        //public static void BuildTestVR()
        //{
        //    BuildConfig();

        //    EditorUserBuildSettings.allowDebugging = true;
        //    EditorUserBuildSettings.development = true;

        //    PlayerSettings.companyName = "Labtime";
        //    PlayerSettings.productName = "NefronVR";
        //    PlayerSettings.virtualRealitySupported = true;
        //    PlayerSettings.Android.androidIsGame = true;
        //    PlayerSettings.Android.androidTVCompatibility = false;
        //    PlayerSettings.Android.bundleVersionCode = 1;
        //    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        //    PlayerSettings.Android.keystorePass = "123456a";
        //    PlayerSettings.Android.keyaliasPass = "123456a";

        //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //    buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
        //    buildPlayerOptions.locationPathName = "Build/Test/VR/NefronVR.apk";
        //    buildPlayerOptions.target = BuildTarget.Android;
        //    buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        //    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        //    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build VR Test Complete Success" : result, "ok");
        //}

        ////[MenuItem("Bootstrap/Build/Test/Android")]
        //public static void BuildTestAndroid()
        //{
        //    BuildConfig();

        //    EditorUserBuildSettings.allowDebugging = true;
        //    EditorUserBuildSettings.development = true;

        //    PlayerSettings.companyName = "Labtime";
        //    PlayerSettings.productName = "Nefron";
        //    PlayerSettings.virtualRealitySupported = false;
        //    PlayerSettings.Android.androidIsGame = true;
        //    PlayerSettings.Android.androidTVCompatibility = false;
        //    PlayerSettings.Android.bundleVersionCode = 1;
        //    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        //    PlayerSettings.Android.keystorePass = "123456a";
        //    PlayerSettings.Android.keyaliasPass = "123456a";

        //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //    buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
        //    buildPlayerOptions.locationPathName = "Build/Test/Android/Nefron.apk";
        //    buildPlayerOptions.target = BuildTarget.Android;
        //    buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        //    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        //    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Android Test Complete Success" : result, "ok");
        //}

        ////[MenuItem("Bootstrap/Build/VR")]
        //public static void BuildVR()
        //{
        //    BuildConfig();

        //    EditorUserBuildSettings.allowDebugging = false;
        //    EditorUserBuildSettings.development = false;

        //    PlayerSettings.companyName = "Labtime";
        //    PlayerSettings.productName = "NefronVR";
        //    PlayerSettings.virtualRealitySupported = true;
        //    PlayerSettings.Android.androidIsGame = true;
        //    PlayerSettings.Android.androidTVCompatibility = false;
        //    PlayerSettings.Android.bundleVersionCode = 1;
        //    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        //    PlayerSettings.Android.keystorePass = "123456a";
        //    PlayerSettings.Android.keyaliasPass = "123456a";

        //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //    buildPlayerOptions.scenes = GetScenesInSettings();//new[] { "Assets/Scenes/Scene1.unity" };
        //    buildPlayerOptions.locationPathName = "Build/VR/NefronVR.apk";
        //    buildPlayerOptions.target = BuildTarget.Android;
        //    buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode | BuildOptions.AutoRunPlayer;
        //    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        //    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build VR Complete Success" : result, "ok");
        //}

        ////[MenuItem("Bootstrap/Build/Android")]
        //public static void BuildAndroid()
        //{
        //    BuildConfig();

        //    EditorUserBuildSettings.allowDebugging = false;
        //    EditorUserBuildSettings.development = false;

        //    PlayerSettings.companyName = "Labtime";
        //    PlayerSettings.productName = "Nefron";
        //    PlayerSettings.virtualRealitySupported = false;
        //    PlayerSettings.Android.androidIsGame = true;
        //    PlayerSettings.Android.androidTVCompatibility = false;
        //    PlayerSettings.Android.bundleVersionCode = 1;
        //    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        //    PlayerSettings.Android.keystorePass = "123456a";
        //    PlayerSettings.Android.keyaliasPass = "123456a";

        //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //    buildPlayerOptions.scenes = GetScenesInSettings();//new[] { "Assets/Scenes/Scene1.unity" };
        //    buildPlayerOptions.locationPathName = "Build/Android/Nefron.apk";
        //    buildPlayerOptions.target = BuildTarget.Android;
        //    buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode | BuildOptions.AutoRunPlayer;
        //    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        //    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Android Complete Success" : result, "ok");
        //}

        ////[MenuItem("Bootstrap/Build/Mac")]
        //public static void BuildMac()
        //{
        //    BuildConfig();

        //    EditorUserBuildSettings.allowDebugging = false;
        //    EditorUserBuildSettings.development = false;

        //    PlayerSettings.companyName = "Labtime";
        //    PlayerSettings.productName = "Nefron";
        //    PlayerSettings.virtualRealitySupported = false;

        //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //    buildPlayerOptions.scenes = GetScenesInSettings();//new[] { "Assets/Scenes/Scene1.unity" };
        //    buildPlayerOptions.locationPathName = "Build/Mac/Nefron.app";
        //    buildPlayerOptions.target = BuildTarget.StandaloneOSXUniversal;
        //    buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode;
        //    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        //    EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Mac Complete Success" : result, "ok");
        //}

        //private static void BuildConfig()
        //{
        //}

    }

}