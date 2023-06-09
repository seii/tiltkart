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
        Debug.Log("Track name in preferences: " + sceneName);

        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Loading scene");
            SceneManager.LoadSceneAsync(sceneName + "Scene");
        }else
        {
            Debug.Log("Not loading scene");
        }
    }
}