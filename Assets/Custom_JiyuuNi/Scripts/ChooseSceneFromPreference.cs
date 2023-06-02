using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseSceneFromPreference : MonoBehaviour
{
    public static event Action OnScenePreLoad;

    public void LoadSceneFromPreference()
    {
        OnScenePreLoad?.Invoke();

        string sceneName = PlayerPrefs.GetString("Track");

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadSceneAsync(sceneName + "Scene");
        }
    }
}