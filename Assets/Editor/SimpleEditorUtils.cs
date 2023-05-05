using System.IO;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EditorAddons
{
    /// <summary>
    /// Contains Unity Editor addons
    /// </summary>
    [InitializeOnLoad]
    public static class EditorUtils
    {
        static EditorUtils()
        {
            // Add method to event
            EditorApplication.playModeStateChanged += ModeChanged;
        }

        [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %q")]
        public static void PlayFromPreLaunchScene()
        {
            // Stop if playing
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            // Save scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // Get current scene
            string current_scene = SceneManager.GetActiveScene().path;

            // Save current scene path
            File.WriteAllText("Assets/Editor/active_scene.txt", current_scene);

            // Open main menu
            EditorSceneManager.OpenScene(
                        "Assets/Scenes/Main Menu.unity");

            // Play
            EditorApplication.isPlaying = true;
        }

        private static void ModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange != PlayModeStateChange.EnteredEditMode) return; // Exited play mode
            // Save scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // Get saved scene path
            var current_scene = File.ReadAllText("Assets/Editor/active_scene.txt");

            // Open saved scene path
            if (current_scene != string.Empty && current_scene != SceneManager.GetActiveScene().path) EditorSceneManager.OpenScene(current_scene);

            // Clear saved scene
            File.WriteAllText("Assets/Editor/active_scene.txt", string.Empty);
        }
    }
}