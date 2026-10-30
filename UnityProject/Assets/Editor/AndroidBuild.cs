using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yemen4x4.Core;

namespace Yemen4x4.Editor
{
    public static class AndroidBuild
    {
        [MenuItem("Yemen 4x4/Build Android APK")]
        public static void Build()
        {
            const string scenePath="Assets/Scenes/Yemen4x4_Main.unity";
            Directory.CreateDirectory("Assets/Scenes");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var bootstrap=new GameObject("Game Bootstrap");bootstrap.AddComponent<GameBootstrap>();
            var world=new GameObject("Yemen 4x4 Runtime World");world.AddComponent<YemenGameWorld>();
            EditorSceneManager.SaveScene(scene,scenePath);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(scenePath,true)};
            PlayerSettings.companyName="Yemen 4x4 Studio";
            PlayerSettings.productName="Yemen 4x4 Adventures";
            PlayerSettings.applicationIdentifier="com.yemen4x4.adventures";
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait=false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
            PlayerSettings.allowedAutorotateToLandscapeLeft=true;
            PlayerSettings.allowedAutorotateToLandscapeRight=true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevel35;
            PlayerSettings.stripEngineCode=true;
            Directory.CreateDirectory("Builds/Android");
            var options=new BuildPlayerOptions{scenes=new[]{scenePath},locationPathName="Builds/Android/Yemen4x4.apk",target=BuildTarget.Android,targetGroup=BuildTargetGroup.Android,options=BuildOptions.None};
            BuildReport report=BuildPipeline.BuildPlayer(options);
            if(report.summary.result!=BuildResult.Succeeded) throw new System.Exception("Android build failed: "+report.summary.result+" / "+report.summary.totalErrors+" errors");
            Debug.Log("Yemen 4x4 APK built: "+report.summary.outputPath+" ("+report.summary.totalSize+" bytes)");
        }
        private static AndroidSdkVersions AndroidMinimumSdk(){return AndroidSdkVersions.AndroidApiLevel26;}
    }
}