using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Foundation.Tools
{
    public class Assistant : EditorWindow
    {
        [MenuItem("Tools/Assistant")]
        public static void ShowWindow() => GetWindow<Assistant>("Assistant");

        private List<string> _sceneNames;
        private List<string> _scenePaths;
        private int _selectedSceneIndex;

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            ShowSceneSelector();
            EditorGUILayout.Space(10);
            ShowPlaymodeControls();
        }

        private void OnEnable()
        {
            FillSceneLists();
        }

        private void FillSceneLists()
        {
            _sceneNames = new();
            _scenePaths = new();
            foreach (var sceneGuid in AssetDatabase.FindAssets("t:Scene"))
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                var sceneAsset = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));

                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scenePath);
                if (packageInfo != null) continue;

                _sceneNames.Add(sceneAsset.name);
                _scenePaths.Add(scenePath);
            }
        }

        private void ShowSceneSelector()
        {
            FillSceneLists();

            EditorGUILayout.BeginHorizontal();

            _selectedSceneIndex = EditorGUILayout.Popup(_selectedSceneIndex, _sceneNames.ToArray());

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_PlayButton On"), GUILayout.Width(30)))
            {
                if (!Application.isPlaying && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(_scenePaths[_selectedSceneIndex]);
                }
                else
                {
                    SceneManager.LoadScene(_sceneNames[_selectedSceneIndex]);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowPlaymodeControls()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            Time.timeScale = EditorGUILayout.Slider(Time.timeScale, .5f, 16);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(".5")) Time.timeScale = .5f;
            if (GUILayout.Button("1")) Time.timeScale = 1;
            if (GUILayout.Button("2")) Time.timeScale = 2;
            if (GUILayout.Button("4")) Time.timeScale = 4;
            if (GUILayout.Button("8")) Time.timeScale = 8;
            if (GUILayout.Button("16")) Time.timeScale = 16;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Restart Scene"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (GUILayout.Button("Restart Game"))
            {
                SceneManager.LoadScene(0);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}