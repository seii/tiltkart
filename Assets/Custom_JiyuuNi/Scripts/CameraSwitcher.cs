using System;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Tooltip("Which GameObject is the main non-TiltFive camera?")]
    public GameObject MainCamera;
    [Tooltip("Which GameObject is the TiltFive Camera?")]
    public GameObject TiltFiveCamera;
    public static event Action<GameObject> OnCameraChange;

    // Start is called before the first frame update
    void Start()
    {
        SwitchCamera();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchCamera();
    }

    /*
     * Switch cameras between TiltFive and standard based on whether TiltFive headset is available
     */
    private void SwitchCamera()
    {
        if (TiltFive.Display.GetGlassesAvailability())
        {
            Debug.Log("Glasses found!");
            // Only make the switch if the TiltFive camera isn't already active
            if(!TiltFiveCamera.activeInHierarchy)
            {
                Debug.Log("TiltFive prefab not active, activating...");
                MainCamera.GetComponent<AudioListener>().enabled = false;
                MainCamera.SetActive(false);
                TiltFiveCamera.GetComponentInChildren<AudioListener>().enabled = true;
                TiltFiveCamera.SetActive(true);
                OnCameraChange?.Invoke(TiltFiveCamera);
            }
        }
        else
        {
            // Only make the switch if the main camera isn't already active
            if (!MainCamera.activeInHierarchy)
            {
                Debug.Log("Glasses not found, using main camera");

                TiltFiveCamera.GetComponentInChildren<AudioListener>().enabled = false;
                TiltFiveCamera.SetActive(false);
                MainCamera.SetActive(true);
                MainCamera.GetComponent<AudioListener>().enabled = false;
                OnCameraChange?.Invoke(MainCamera);
            }
        }
    }
}
