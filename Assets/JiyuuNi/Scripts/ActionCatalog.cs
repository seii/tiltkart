using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Catalog all Actions for troubleshooting and/or logging
 */
public class ActionCatalog : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Enable logging for events and Actions?")]
    private bool LoggingEnabled;

    private string thisClass = "ActionCatalog";

    void Awake()
    {
        CameraSwitcher.OnCameraChange += OnCameraChange;
        HorizontalSelector.OnSelectionChange += OnSelectionChange;
        KartSelector.onKartChange += OnKartChange;
        ResetToStartingLine.OnOutOfBounds += OnOutOfBounds;
        SceneSwitcher.loadProgress += LoadProgress;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    void OnDestroy()
    {
        CameraSwitcher.OnCameraChange -= OnCameraChange;
        HorizontalSelector.OnSelectionChange -= OnSelectionChange;
        KartSelector.onKartChange -= OnKartChange;
        ResetToStartingLine.OnOutOfBounds += OnOutOfBounds;
        SceneSwitcher.loadProgress -= LoadProgress;
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnloaded;
    }

    void OnCameraChange(GameObject gameObject)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected OnCameraChange for {gameObject.name}");
        }
    }

    void OnSelectionChange(string canvasName, string objName)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected OnSelectionChange for {canvasName} to value {objName}");
        }
    }

    void OnKartChange(GameObject gameObj)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected OnKartChange for {gameObj.name}");
        }
    }

    void OnOutOfBounds(string collider)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected OnOutOfBounds for {collider}");
        }
    }

    void SceneLoading(string sceneName, bool loadingScreen)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected SceneLoading for {sceneName}, loading screen = {loadingScreen}");
        }
    }

    void SceneUnloading(string sceneName)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected SceneUnloading for {sceneName}");
        }
    }

    void LoadProgress(float progress)
    {
        if (LoggingEnabled)
        {
            print($"Loading progress: {progress}");
        }
    }

    void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected SceneLoaded for {scene.name}");
        }
    }

    void SceneUnloaded(Scene scene)
    {
        if (LoggingEnabled)
        {
            print($"{thisClass}: Detected SceneUnloaded for {scene.name}");
        }
    }
}
