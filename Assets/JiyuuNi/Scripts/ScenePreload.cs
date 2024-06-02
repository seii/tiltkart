using System.Collections.Generic;
using UnityEngine;

/**
 * Preload one or more specified scenes
 */
public class ScenePreload : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Names of levels which should be pre-loaded in advance")]
    private List<string> levelsToPreload;

    private string thisClass = nameof(ScenePreload);

    // Start is called before the first frame update
    private void Start()
    {
        foreach(string scene in levelsToPreload)
        {
            print($"{thisClass}: Asking for {scene} to be preloaded");
            StartCoroutine(SceneSwitcher.Instance.PreloadScene(scene));
        }
    }
}
