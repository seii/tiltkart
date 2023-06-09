using Cinemachine;
using KartGame.KartSystems;
using UnityEngine;

public class KartSelector : MonoBehaviour
{
    public GameObject tiltFiveGameBoard;
    public GameObject mainCamera;
    public GameObject cinemachineVirtualCamera;
    public GameFlowManager gameManager;
    public ArcadeKart[] PlayerKarts;

    private string kartName;
    private ArcadeKart currentKart;
    private GameObject trackingObject;

    private void OnEnable()
    {
        // Default to main camera
        trackingObject = mainCamera;

        CameraSwitcher.OnCameraChange += CheckCurrentCamera;

        kartName = PlayerPrefs.GetString("Kart");

        SwitchKart(kartName);
    }

    // Update is called once per frame
    void Update()
    {
        // Unused. Currently, there's no gameplay mechanic that would require switching
        //    karts mid-race.
    }

    private void OnDestroy()
    {
        CameraSwitcher.OnCameraChange -= CheckCurrentCamera;
    }

    private void SwitchKart(string kartName)
    {
        // Set default to first kart if no preference is found
        if (string.IsNullOrEmpty(kartName))
        {
            currentKart = PlayerKarts[0];
            currentKart.gameObject.SetActive(true);
            gameManager.playerKart = currentKart;
            ResetCameras(currentKart);
        }
        else
        {
            // Check each Kart, and if one matches the preference enable that kart
            foreach (ArcadeKart kart in PlayerKarts)
            {
                if (kart.gameObject.name.StartsWith(kartName))
                {
                    if(currentKart != null && !currentKart.Equals(kart.gameObject))
                    {
                        currentKart.gameObject.SetActive(false);
                    }

                    currentKart = kart;
                    currentKart.gameObject.SetActive(true);
                    gameManager.playerKart = currentKart;
                    ResetCameras(currentKart);
                    break;
                }
            }
        }
    }

    private void ResetCameras(ArcadeKart kart)
    {
        Transform kartTransform = kart.gameObject.transform;
        Transform capsuleTransform = kart.gameObject.transform.Find("KartBouncingCapsule").gameObject.transform;

        if (trackingObject.Equals(mainCamera))
        {
            cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = kartTransform;
            cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = capsuleTransform;
        }
        else if (trackingObject.Equals(tiltFiveGameBoard))
        {
            tiltFiveGameBoard.GetComponent<FollowKart>().followObject = kartTransform;
        }
    }

    private void CheckCurrentCamera(GameObject newCamera)
    {
        ResetCameras(currentKart);
    }
}
