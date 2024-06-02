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
    //public static event Action<string, bool> sceneLoading;
    //public static event Action<string> sceneUnloading;
    public static event Action<float> loadProgress;

    [SerializeField]
    [Tooltip("Name of scene to use for loading screen")]
    private string loadingSceneName;

    [SerializeField]
    [Tooltip("(Optional) Load scene with this name first after starting program")]
    private string firstSceneName;

    private string rootSceneName;
    private string activeSceneName;
    private AsyncOperation loadingOperation = null;

    private Dictionary<string, AsyncOperation> pausedOperations = new Dictionary<string, AsyncOperation>(1);

    private string thisClass = nameof(SceneSwitcher);

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        // ** WARNING **
        // This event is, at the time of writing, an unofficial addon to the
        // TiltFive Unity SDK. It will work within this game because the SDK
        // has been modified appropriately, but would not exist in other project!
        Player.newPlayerAdded += NewPlayerAdded;
    }

    /**
     * <credit>
     * Jasper Flick - Catlike Coding - https://catlikecoding.com
     * </credit>
     */
    private void Start()
    {
        print($"{thisClass}: Starting, attached to scene name: {this.gameObject.name}");

        // Root scene = first loaded scene
        rootSceneName = SceneManager.GetSceneAt(0).name;

        // Also track which scene is currently active
        activeSceneName = SceneManager.GetActiveScene().name;

        // If multiple scenes are open in the Editor on launch, set the most recent as "active"
        if(Application.isEditor)
        {
            if(SceneManager.sceneCount > 1)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
            }
        }

        if (!string.IsNullOrEmpty(firstSceneName))
        {
            StartCoroutine(LoadScene(firstSceneName));
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

    public Scene GetRootScene()
    {
        return SceneManager.GetSceneByName(rootSceneName);
    }

    public void ReplaceCurrentScene(string newScene, bool useLoadingScreen)
    {
        print($"{thisClass}: Attempting to replace {activeSceneName} with {newScene}");

        if(useLoadingScreen)
        {
            print($"{thisClass}: Loading screen requested, activating it now");
            StartCoroutine(StartLoadingScreen(newScene));
        }else
        {
            print($"{thisClass}: No loading screen requested, proceeding with replacement");
            StartCoroutine(UnloadScene(activeSceneName));
            StartCoroutine(LoadScene(newScene));
        }
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
        SceneManager.SetActiveScene(GetRootScene());

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

        print($"{thisClass}: Player {index} fully activated, returning to scene {oldActiveScene.name}");
        SceneManager.SetActiveScene(oldActiveScene);
    }

    /**
     * <credit>
     * Jasper Flick - Catlike Coding - https://catlikecoding.com
     * </credit>
     */
    public IEnumerator LoadScene(string sceneName)
    {
        print($"{thisClass}: Beginning scene loading for {sceneName}");

        // Check if the scene is valid to load
        if (IsSceneInBuildSettings(sceneName))
        {
            // Don't ever attempt to load the root scene
            if(!sceneName.Equals(rootSceneName))
            {
                // No safeguard against re-loading is implemented here because the only
                // way in `SceneManager` to "reload" a scene is to call "load" again

                // Check first if the scene was pre-loaded and is just awaiting activation
                if (pausedOperations.Count > 0)
                {
                    string operationNames = "";

                    foreach (string oneName in pausedOperations.Keys)
                    {
                        operationNames = string.Concat(operationNames, ", ", oneName);
                    }

                    if (pausedOperations.ContainsKey(sceneName))
                    {
                        print($"{thisClass}: Found {sceneName} as a preloaded scene, activating it");

                        pausedOperations[sceneName].allowSceneActivation = true;
                        /*pausedOperations.Remove(sceneName);
                        yield return null;*/
                        yield return pausedOperations[sceneName];
                        yield return null;
                        pausedOperations.Remove(sceneName);
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                        activeSceneName = sceneName;
                    }
                    else
                    {
                        string[] loadingArray = new List<string>(pausedOperations.Keys).ToArray();
                        string firstLoading = loadingArray[0];
                        string loadingStrings = string.Join(", ", loadingArray);
                        Debug.LogWarning($"{thisClass}: The following scenes are queued for loading and " +
                            $"block {sceneName} from activating: '{operationNames}'");
                    }
                }else
                {
                    yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                    // Wait one more frame to allow loading to finish...
                    yield return null;

                    // ...then set the newly loaded scene as the active scene
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                    activeSceneName = sceneName;
                }
            }
        }else
        {
            print($"{thisClass}: Scene {sceneName} does NOT exist in build settings, skipping load");
        }
    }

    public IEnumerator UnloadScene(string sceneName)
    {
        // Never attempt to unload the root scene
        if(!sceneName.Equals(rootSceneName))
        {
            print($"{thisClass}: Beginning scene unloading for {sceneName}");

            bool alreadyLoaded = false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name.Equals(sceneName))
                {
                    print($"{thisClass}: Scene {sceneName} confirmed as still loaded, proceeding with unload");
                    enabled = false;

                    yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneName));

                    enabled = true;
                    alreadyLoaded = true;
                    break;
                }
            }

            if (!alreadyLoaded)
            {
                print($"{thisClass}: Scene {sceneName} not loaded, skipping unload");
            }
        }
    }

    public IEnumerator PreloadScene(string sceneName)
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

    private IEnumerator StartLoadingScreen(string newScene)
    {
        print($"{thisClass}: Activating loading screen");

        // Load the loading screen first, then the actual scene after
        yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

        yield return null;
        yield return UnloadScene(activeSceneName);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingSceneName));

        activeSceneName = loadingSceneName;

        print($"{thisClass}: Loading screen active, watching progress for scene {newScene}");
        yield return PreloadScene(newScene);

        // Wait one extra frame for preloading to complete
        yield return null;

        // Activate new scene now that it's loaded
        yield return LoadScene(newScene);

        // Remember to unload loading scene
        StartCoroutine(UnloadScene(loadingSceneName));
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print($"{thisClass}: Successfully loaded {scene.name}");

        // Design choice: For every scene _except_ the root scene or a scene specified
        // as "load this first", set that scene to "active" as soon as it's loaded
        if (!scene.name.Equals(rootSceneName))
        {
            print($"{thisClass}: Setting {scene.name} as active scene");
            SceneManager.SetActiveScene(scene);
        }

        // Design choice: Only save preferences in permanent fashion
        // on scene load. This helps to prevent scenes which terminate
        // early from saving potentially problematic preferences.
        PreferenceManager.Instance.SaveAllPrefs();
    }

    private void SceneUnloaded(Scene scene)
    {
        print($"{thisClass}: Successfully unloaded {scene.name}");
        //loadedScenes.Remove(scene.name);
    }

    private void ActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        /*if (oldScene != null && !string.IsNullOrEmpty(oldScene.name))
        {
            print($"{thisClass}: Detected change of active scene from {oldScene.name} to {newScene.name}");
            activeSceneName = newScene.name;

            if (oldScene.name.Equals(loadingSceneName))
            {
                //BeginUnload(loadingSceneName);
                StartCoroutine(UnloadScene(loadingSceneName));
            }
        }*/
    }

    private void NewPlayerAdded(PlayerIndex index)
    {
        print($"{thisClass}: New TiltFive player number {index} added");
        Scene currentScene = SceneManager.GetActiveScene();
        StartCoroutine(WaitForPlayerConnection(currentScene, index));
    }
}