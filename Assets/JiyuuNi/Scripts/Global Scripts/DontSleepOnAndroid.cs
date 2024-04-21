using UnityEngine;

/**
 * Prevents screen from going to sleep on Android devices
 */
public class DontSleepOnAndroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
