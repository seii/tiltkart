using UnityEngine;

/**
 * Allow Unity to load scenes with a single click, using either
 * the scene's full name or simply the name of the preference
 * which stores its name in shortened form
 */
public class BindButtonToScene : MonoBehaviour
{
    public void LoadNamedScene(string sceneName)
    {
        SceneSwitcher.Instance.BeginLoad(sceneName, false, true);
    }

    public void LoadSceneByPrefName(string prefName)
    {
        string sceneName = PreferenceManager.Instance.GetPref(prefName) + "Scene";
        SceneSwitcher.Instance.BeginLoad(sceneName, true, true);
    }
}
