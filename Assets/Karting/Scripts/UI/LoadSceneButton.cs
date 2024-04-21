using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Tooltip("What is the name of the scene we want to load when clicking the button?")]
    public string SceneName;

    //public static event Action<string, string> requestSceneLoad;

    public void LoadTargetScene() 
    {
        //SceneManager.LoadSceneAsync(SceneName);
        //requestSceneLoad?.Invoke(SceneName, SceneManager.GetActiveScene().name);
    }
}
