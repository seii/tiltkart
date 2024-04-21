using UnityEngine;

/**
 * Set target FPS for this game on all platforms
 */
public class TargetFPS : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}
