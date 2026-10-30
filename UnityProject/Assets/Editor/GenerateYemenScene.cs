#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yemen4x4.Core;

namespace Yemen4x4.Editor
{
    public static class GenerateYemenScene
    {
        [MenuItem("Yemen 4x4/Generate Playable Prototype Scene")]
        public static void Generate()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Single);
            var root=new GameObject("Yemen 4x4 Game Bootstrap");
            root.AddComponent<GameBootstrap>();
            root.AddComponent<YemenGameWorld>();
            const string dir="Assets/Scenes";
            if(!AssetDatabase.IsValidFolder(dir))AssetDatabase.CreateFolder("Assets","Scenes");
            EditorSceneManager.SaveScene(scene,dir+"/SanaaDepot.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(dir+"/SanaaDepot.unity",true)};
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait=false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
            PlayerSettings.allowedAutorotateToLandscapeLeft=true;
            PlayerSettings.allowedAutorotateToLandscapeRight=true;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            AssetDatabase.SaveAssets();
            Debug.Log("Yemen 4x4 prototype scene generated. Open Assets/Scenes/SanaaDepot.unity.");
        }
    }
}
#endif
