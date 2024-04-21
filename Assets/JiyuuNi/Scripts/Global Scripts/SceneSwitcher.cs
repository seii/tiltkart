using System;
using System.Collections;
using System.Collections.Generic;
using TiltFive;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Switch between multiple Unity "Scenes"
 */
public class SceneSwitcher : Singleton<SceneSwitcher>
{
    public static event Action<string, bool> sceneLoading;
    public static event Action<string> sceneUnloading;
    public static event Action<float> loadProgress;

    [SerializeField]
    [Tooltip("Name of scene to use for loading screen")]
    private string loadingSceneName;

    [SerializeField]
    [Tooltip("(Optional) Load scene with this name first after starting program")]
    private string firstSceneName;

    private string rootSceneName;
    private string activeSceneName;
    // List of all scenes which have been loaded, _except_ the scene
    // this script is started in (the "root scene")
    private List<string> loadedScenes = new List<string>();
    //private bool isStarted = false;
    private AsyncOperation loadingOperation = null;

    private Dictionary<string, AsyncOperation> pausedOperations = new Dictionary<string, AsyncOperation>(1);

    private string thisClass = nameof(SceneSwitcher);

    /**
     * <credit>
     * Jasper Flick - Catlike Coding - https://catlikecoding.com
     * </credit>
     */
    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        // ** WARNING **
        // This event is, at the time of writing, an unofficial addon to the
        // TiltFive Unity SDK. It will work within this game because the SDK
        // has been modified appropriately, but would not exist in other project!
        Player.newPlayerAdded += NewPlayerAdded;

        // Assumption: This script is started in the root scene
        rootSceneName = SceneManager.GetActiveScene().name;
        // Also track which scene is currently active
        activeSceneName = SceneManager.GetActiveScene().name;
        print($"{thisClass}: Attached to scene name {rootSceneName}");

        // If multiple scenes are already loaded in the Editor, make sure
        // that they are tracked as such
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            string tempSceneName = SceneManager.GetSceneAt(i).name;

            if (tempSceneName != rootSceneName)
            {
                print($"Scene {tempSceneName} already loaded in editor, adding it to loaded scenes");
                loadedScenes.Add(tempSceneName);
            }
        }        

        if (!string.IsNullOrEmpty(firstSceneName))
        {
            BeginLoad(firstSceneName, false, false);
        }

        Scene mostRecentScene = GetRootScene();

        if (SceneManager.sceneCount > 0)
        {
            mostRecentScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }

        if (!mostRecentScene.name.StartsWith(rootSceneName))
        {
            SceneManager.SetActiveScene(mostRecentScene);
        }
    }

    protected void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnloaded;
        SceneManager.activeSceneChanged -= ActiveSceneChanged;

        // ** WARNING **
        // This event is, at the time of writing, an unofficial addon to the
        // TiltFive Unity SDK. It will work within this game because the SDK
        // has been modified appropriately, but would not exist in other project!
        Player.newPlayerAdded -= NewPlayerAdded;
    }

    public void PreLoad(string sceneName)
    {
        print($"{thisClass}: Preloading {sceneName}");
        StartCoroutine(PreloadScene(sceneName));
    }

    public void BeginLoad(string sceneName, bool loadingScreen, bool replaceActiveScene)
    {
        print($"{thisClass}: Beginning scene loading for {sceneName}");

        // Check if the scene is valid to load
        if (IsSceneInBuildSettings(sceneName))
        {
            print($"{thisClass}: Scene {sceneName} exists in build settings");

            if (replaceActiveScene)
            {
                print($"{thisClass}: Received request to replace {activeSceneName} with {sceneName}");
                //StartCoroutine(UnloadScene(activeSceneName));
                BeginUnload(activeSceneName);
            }

            if (loadedScenes.Contains(sceneName))
            {
                print($"{thisClass}: Scene {sceneName} already loaded, skipping load");
            }
            else
            {
                print($"{thisClass}: Scene {sceneName} not already loaded, loading it now");
                StartCoroutine(LoadScene(sceneName, loadingScreen));
            }
        }
        else
        {
            print($"{thisClass}: Scene {sceneName} does NOT exist in build settings, skipping load");
        }
    }

    public void BeginUnload(string sceneName)
    {
        print($"{thisClass}: Beginning scene unloading for {sceneName}");

        // Check if the scene is valid to unload
        foreach(string tempScene in loadedScenes)
        {
            if(tempScene.Equals(sceneName))
            {
                print($"{thisClass}: Scene {sceneName} confirmed as still loaded");
                StartCoroutine(UnloadScene(sceneName));
                break;
            }else
            {
                print($"{thisClass}: Scene {sceneName} not loaded, skipping unload");
            }
        }
    }

    public Scene GetRootScene()
    {
        return SceneManager.GetSceneByName(rootSceneName);
    }

    /**
     * <credit>
     * Derek Foster - Tri-Powered Games - https://tri-powered-games.com/knowledge-center/unity-how-and-when-to-use-additive-scenes
     * </credit>
     */
    private bool IsSceneInBuildSettings(string name)
    {
        bool result = false;

        // Don't let invalid strings through
        if (string.IsNullOrEmpty(name)) {
            print($"{thisClass}: No scene name provided, aborting");
        }

        // Fetch all scenes in build settings, and identify if the any of the
        // scenes match the given scene name, if so, return true
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            string sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0) {
                result = true;
                break;
            }
        }

        return result;
    }

    private IEnumerator WaitForPlayerConnection(Scene oldActiveScene, PlayerIndex index)
    {
        // Glasses typically take a couple seconds to fully spin up, wait for this to happen
        yield return new Player.WaitUntilPlayerConnected(index);

        // The "pose root" for the cameras is a placeholder GameObject created well before
        // a new Glasses connection occurs. In multi-scene environments, this means the
        // object was likely instantiated somewhere outside the root scene no matter how
        // fast we switch to the root scene when a connection is detected. To solve this
        // problem, simply query the new connection for the object and move that object
        // to the root scene.
        GameObject glassesCameras = Glasses.GetPoseRoot(index);
        print($"Found player Glasses object {glassesCameras}");
        SceneManager.MoveGameObjectToScene(glassesCameras, GetRootScene());

        print($"{thisClass}: Player {index} fully activated, returning {oldActiveScene.name}");
        SceneManager.SetActiveScene(oldActiveScene);
    }

    /**
     * <credit>
     * Jasper Flick - Catlike Coding - https://catlikecoding.com
     * </credit>
     */
    private IEnumerator LoadScene(string sceneName, bool loadingScreen)
    {
        yield return null;

        print($"{thisClass}: Loading {sceneName} additively");
        string loadingOutput = loadingScreen ? "requested, activating" : "not requested";
        print($"{thisClass}: Loading screen {loadingOutput}");

        sceneLoading?.Invoke(sceneName, loadingScreen);
        enabled = false;

        if(pausedOperations.Count > 0)
        {
            string test = "";
            foreach (string onetest in pausedOperations.Keys)
            {
                test = string.Concat(test, ", ", onetest);
            }

            if (pausedOperations.ContainsKey(sceneName))
            {
                print($"{thisClass}: Found {sceneName} as a preloaded scene, activating it");

                if (loadingScreen)
                {
                    Debug.LogWarning($"{thisClass}: {sceneName} was already preloaded, skipping loading screen");
                }

                pausedOperations[sceneName].allowSceneActivation = true;
                pausedOperations.Remove(sceneName);
            }
            else
            {
                
                string[] loadingArray = new List<string>(pausedOperations.Keys).ToArray();
                string firstLoading = loadingArray[0];
                string loadingStrings = string.Join(", ", loadingArray);
                Debug.LogWarning($"{thisClass}: The following scenes are queued for loading and " +
                    $"block activation: '{test}'");
            }

            yield return null;
        }else
        {
            if (loadingScreen)
            {
                // Load the loading screen first, then the actual scene after
                SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
                loadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadingOperation.allowSceneActivation = false;

                while (!loadingOperation.isDone)
                {
                    print($"{thisClass}: Loading progress for {sceneName}: {loadingOperation.progress * 100}%");
                    loadProgress.Invoke(loadingOperation.progress);

                    if (loadingOperation.progress >= 0.9f)
                    {
                        loadingOperation.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        loadingOperation = null;
        enabled = true;
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        print($"{thisClass}: Unloading {sceneName}");
        sceneUnloading?.Invoke(sceneName);
        enabled = false;
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneName));
        yield return null;

        enabled = true;
    }

    private IEnumerator PreloadScene(string sceneName)
    {
        print($"{thisClass}: Preloading {sceneName}");

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        print($"{thisClass}: Adding {sceneName} to list of preloading scenes");
        pausedOperations.Add(sceneName, operation);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            print($"{thisClass}: Loading progress for {sceneName}: {operation.progress * 100}%");
            loadProgress.Invoke(operation.progress);

            yield return null;
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print($"{thisClass}: Successfully loaded {scene.name}");

        // Design choice: For every scene _except_ the root scene or a scene specified
        // as "load this first", set that scene to "active" as soon as it's loaded
        if(!scene.name.Equals(rootSceneName))
        {
            print($"{thisClass}: Setting {scene.name} as active scene");
            SceneManager.SetActiveScene(scene);
        }

        // Track all loading/loaded scenes locally, whether they're active or not
        loadedScenes.Add(scene.name);

        // Design choice: Only save preferences in permanent fashion
        // on scene load. This helps to prevent scenes which terminate
        // early from saving potentially problematic preferences.
        PreferenceManager.Instance.SaveAllPrefs();
    }

    private void SceneUnloaded(Scene scene)
    {
        print($"{thisClass}: Successfully unloaded {scene.name}");
        loadedScenes.Remove(scene.name);
    }

    private void ActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        print($"{thisClass}: Detected change of active scene from {oldScene.name} to {newScene.name}");
        activeSceneName = newScene.name;

        if(oldScene.name.Equals(loadingSceneName))
        {
            BeginUnload(loadingSceneName);
        }
    }

    private void NewPlayerAdded(PlayerIndex index)
    {
        print($"{thisClass}: New TiltFive player number {index} added");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(GetRootScene());
        StartCoroutine(WaitForPlayerConnection(currentScene, index));
    }
}