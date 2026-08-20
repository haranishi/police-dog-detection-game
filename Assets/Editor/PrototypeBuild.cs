using System;
using System.IO;
using PoliceDog.Delivery;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PoliceDog.Editor
{
    public static class PrototypeBuild
    {
        private const string ScenePath = "Assets/Scenes/VerticalSlice.unity";

        [MenuItem("Police Dog/Create Vertical Slice Scene")]
        public static void CreateVerticalSliceScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Police Dog Vertical Slice", typeof(PrototypeBootstrap));
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            PlayerSettings.companyName = "Local Prototype";
            PlayerSettings.productName = "Police Dog Detection - Vertical Slice";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            AssetDatabase.SaveAssets();
            Debug.Log("Created vertical slice scene: " + ScenePath);
        }

        public static void BuildMac()
        {
            CreateVerticalSliceScene();
            var output = Path.GetFullPath("Builds/PoliceDogDetection.app");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.Development
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new InvalidOperationException("macOS build failed: " + report.summary.result);
            }
            Debug.Log("Created local build: " + output);
        }
    }
}
