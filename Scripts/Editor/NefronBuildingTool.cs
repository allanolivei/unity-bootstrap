using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class NefronBuildingTool
{

    [MenuItem("Nefron/Build/Test/Mac")]
    public static void BuildTestMac()
    {
        BuildConfig();

        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.development = false;

        PlayerSettings.companyName = "Labtime";
        PlayerSettings.productName = "Nefron";
        PlayerSettings.virtualRealitySupported = false;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
        buildPlayerOptions.locationPathName = "Build/Test/Mac/Nefron.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSXUniversal;
        buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode;
        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Mac Complete Success" : result, "ok");
    }

    [MenuItem("Nefron/Build/Test/VR")]
    public static void BuildTestVR()
    {
        BuildConfig();

        EditorUserBuildSettings.allowDebugging = true;
        EditorUserBuildSettings.development = true;

        PlayerSettings.companyName = "Labtime";
        PlayerSettings.productName = "NefronVR";
        PlayerSettings.virtualRealitySupported = true;
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.androidTVCompatibility = false;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.keystorePass = "123456a";
        PlayerSettings.Android.keyaliasPass = "123456a";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
        buildPlayerOptions.locationPathName = "Build/Test/VR/NefronVR.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build VR Test Complete Success" : result, "ok");
    }

    [MenuItem("Nefron/Build/Test/Android")]
    public static void BuildTestAndroid()
    {
        BuildConfig();

        EditorUserBuildSettings.allowDebugging = true;
        EditorUserBuildSettings.development = true;

        PlayerSettings.companyName = "Labtime";
        PlayerSettings.productName = "Nefron";
        PlayerSettings.virtualRealitySupported = false;
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.androidTVCompatibility = false;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        PlayerSettings.Android.keystorePass = "123456a";
        PlayerSettings.Android.keyaliasPass = "123456a";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { SceneManager.GetActiveScene().path };
        buildPlayerOptions.locationPathName = "Build/Test/Android/Nefron.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Android Test Complete Success" : result, "ok");
    }

    [MenuItem("Nefron/Build/VR")]
    public static void BuildVR()
    {
        BuildConfig();

        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.development = false;

        PlayerSettings.companyName = "Labtime";
        PlayerSettings.productName = "NefronVR";
        PlayerSettings.virtualRealitySupported = true;
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.androidTVCompatibility = false;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.keystorePass = "123456a";
        PlayerSettings.Android.keyaliasPass = "123456a";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenesInSettings();//new[] { "Assets/Scenes/Scene1.unity" };
        buildPlayerOptions.locationPathName = "Build/VR/NefronVR.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode | BuildOptions.AutoRunPlayer;
        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build VR Complete Success" : result, "ok");
    }

    [MenuItem("Nefron/Build/Android")]
    public static void BuildAndroid()
    {
        BuildConfig();

        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.development = false;

        PlayerSettings.companyName = "Labtime";
        PlayerSettings.productName = "Nefron";
        PlayerSettings.virtualRealitySupported = false;
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.androidTVCompatibility = false;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        PlayerSettings.Android.keystorePass = "123456a";
        PlayerSettings.Android.keyaliasPass = "123456a";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenesInSettings();//new[] { "Assets/Scenes/Scene1.unity" };
        buildPlayerOptions.locationPathName = "Build/Android/Nefron.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode | BuildOptions.AutoRunPlayer;
        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Android Complete Success" : result, "ok");
    }

    [MenuItem("Nefron/Build/Mac")]
    public static void BuildMac()
    {
        BuildConfig();

        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.development = false;

        PlayerSettings.companyName = "Labtime";
        PlayerSettings.productName = "Nefron";
        PlayerSettings.virtualRealitySupported = false;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenesInSettings();//new[] { "Assets/Scenes/Scene1.unity" };
        buildPlayerOptions.locationPathName = "Build/Mac/Nefron.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSXUniversal;
        buildPlayerOptions.options = BuildOptions.ForceOptimizeScriptCompilation | BuildOptions.Il2CPP | BuildOptions.StrictMode;
        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        EditorUtility.DisplayDialog("Build Result", string.IsNullOrEmpty(result) ? "Build Mac Complete Success" : result, "ok");
    }

    private static void BuildConfig()
    {
    }

    private static string[] GetScenesInSettings()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (var i = 0 ; i < scenes.Length ; i++)
            scenes[i] = EditorBuildSettings.scenes[i].path;
        return scenes;
    }
	
}
