using UnityEngine;

/**
 * Running the `Exit` method causes the program to exit in the Editor as well as
 * on full builds for PC and Android
 */
public class ExitApplication : MonoBehaviour
{
    public void Exit()
    {
#if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_ANDROID
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
#endif

        Application.Quit();
    }
}
