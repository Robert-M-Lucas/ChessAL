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
            // Add method to event
            EditorApplication.playModeStateChanged += ModeChanged;
        }

        [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %q")]
        public static void PlayFromPrelaunchScene()
        {
            // Stop if playing
            if (EditorApplication.isPlaying == true)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            // Save scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // Get current scene
            string current_scene = EditorSceneManager.GetActiveScene().path;

            // Save current scene path
            File.WriteAllText("Assets/Editor/active_scene.txt", current_scene);

            // Open main menu
            EditorSceneManager.OpenScene(
                        "Assets/Scenes/Main Menu.unity");

            // Play
            EditorApplication.isPlaying = true;
        }

        static void ModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode) // Exited play mode
            {
                // Save scene
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                // Get saved scene path
                string current_scene = File.ReadAllText("Assets/Editor/active_scene.txt");

                // Open saved scene path
                if (current_scene != string.Empty && current_scene != EditorSceneManager.GetActiveScene().path) EditorSceneManager.OpenScene(current_scene);

                // Clear saved scene
                File.WriteAllText("Assets/Editor/active_scene.txt", string.Empty);
            }
        }
    }
}