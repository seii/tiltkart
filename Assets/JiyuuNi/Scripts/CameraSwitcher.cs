using System;
using UnityEngine;

/**
 * Switch cameras between TiltFive and standard based on whether TiltFive headset is available
 */
public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Which GameObject is the main non-TiltFive camera?")]
    private GameObject MainCamera;

    public static event Action<GameObject> OnCameraChange;

    private Camera TiltFiveCamera;

    private string thisClass = nameof(CameraSwitcher);

    // Start is called before the first frame update
    protected void Start()
    {
        TiltFiveCamera = PreferenceManager.Instance.GetT5Camera();
        SwitchCamera();
    }

    // Update is called once per frame
    protected void Update()
    {
        SwitchCamera();
    }

    private void SwitchCamera()
    {
        if (TiltFive.Display.GetGlassesAvailability())
        {
            //Debug.Log("Glasses found!");
            // Only make the switch if the TiltFive camera isn't already active
            if(!TiltFiveCamera.gameObject.activeInHierarchy)
            {
                Debug.Log($"{thisClass}: TiltFive prefab not active, activating...");
                TiltFiveCamera.gameObject.SetActive(true);
                OnCameraChange?.Invoke(TiltFiveCamera.gameObject);
            }

            MainCamera.SetActive(false);
        }
        else
        {
            // Only make the switch if the main camera isn't already active
            if (!MainCamera.activeInHierarchy)
            {
                Debug.Log($"{thisClass}: Glasses not found, using main camera");

                TiltFiveCamera.gameObject.SetActive(false);
                MainCamera.SetActive(true);
                OnCameraChange?.Invoke(MainCamera);
            }
        }
    }
}
