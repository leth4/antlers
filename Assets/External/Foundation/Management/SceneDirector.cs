using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Foundation
{
    public static class SceneDirector
    {
        public static int ActiveSceneIndex => SceneManager.GetActiveScene().buildIndex;

        public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }

        public static void LoadScene(int buildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(buildIndex, loadSceneMode);
        }

        public static void LoadScene(Scene scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadScene(scene.buildIndex, loadSceneMode);
        }

        public static void LoadNextScene()
        {
            if (ActiveSceneIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                LoadScene(0);
            }
            else
            {
                LoadScene(ActiveSceneIndex + 1);
            }
        }

        public static void RestartScene()
        {
            LoadScene(SceneManager.GetActiveScene());
        }
    }
}