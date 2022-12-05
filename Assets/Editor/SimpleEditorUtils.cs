using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace EditorAddons
{
    [InitializeOnLoad]
    public static class SimpleEditorUtils
    {
        static SimpleEditorUtils()
        {
            EditorApplication.playModeStateChanged += ModeChanged;
        }

        [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %q")]
        public static void PlayFromPrelaunchScene()
        {
            if (EditorApplication.isPlaying == true)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            string current_scene = EditorSceneManager.GetActiveScene().path;

            File.WriteAllText("Assets/Editor/active_scene.txt", current_scene);

            EditorSceneManager.OpenScene(
                        "Assets/Scenes/Main Menu.unity");
            EditorApplication.isPlaying = true;
        }

        static void ModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                string current_scene = File.ReadAllText("Assets/Editor/active_scene.txt");
                if (current_scene != string.Empty) EditorSceneManager.OpenScene(current_scene);
                current_scene = string.Empty;
            }
        }
    }
}