using Cinemachine;
using KartGame.KartSystems;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Tracker for which Kart object is currently selected, and miscellaneous logic
 * to be executed when the Kart object is changed to a different Kart object
 */
public class KartSelector : MonoBehaviour
{
    public static event Action<GameObject> onKartChange;

    [SerializeField]
    private T5Connection connection;

    [SerializeField]
    private GameFlowManager[] gameManagers;

    [SerializeField]
    private ArcadeKart[] PlayerKarts;

    [SerializeField]
    private List<GameObject> cameras;

    private string kartName;
    private ArcadeKart currentKart;
    private TiltFiveProperties properties;

    private string thisClass = nameof(KartSelector);

    private void Start()
    {
        CameraSwitcher.OnCameraChange += CheckCurrentCamera;

        // Capture the most recent instance of TiltFiveProperties
        properties = FindObjectsOfType<TiltFiveProperties>()[^1];

        kartName = PlayerPrefs.GetString("Kart");
        connection.UpdateCurrentKartName(kartName);

        GameObject board = PreferenceManager.Instance.GetT5Board();

        if (board != null)
        {
            cameras.Add(board);
        }

        SwitchKart(kartName);
    }

    // Update is called once per frame
    private void Update()
    {
        // Unused. Currently, there's no gameplay mechanic that would require switching
        //    karts mid-race.
    }

    private void OnDisable()
    {
        CameraSwitcher.OnCameraChange -= CheckCurrentCamera;
    }

    private void SwitchKart(string kartName)
    {
        // Set default to first kart if no preference is found
        if (string.IsNullOrEmpty(kartName))
        {
            currentKart = PlayerKarts[0];
        }
        else
        {
            // Check each Kart, and if one matches the preference enable that kart
            foreach (ArcadeKart kart in PlayerKarts)
            {
                print($"{thisClass}: Checking {kart.name} looking for {kartName}");

                if (kart.gameObject.name.StartsWith(kartName))
                {
                    if(currentKart != null && !currentKart.Equals(kart.gameObject))
                    {
                        currentKart.gameObject.SetActive(false);
                    }

                    currentKart = kart;
                    break;
                }
            }
        }

        print($"{thisClass}: Current kart is " + currentKart.gameObject.name);

        currentKart.gameObject.SetActive(true);

        foreach (GameFlowManager manager in gameManagers)
        {
            if(manager.isActiveAndEnabled)
            {
                manager.playerKart = currentKart;
                break;
            }
        }

        ResetCameras(currentKart);

        print($"{thisClass}: Invoking change to {currentKart.name}");
        onKartChange?.Invoke(currentKart.gameObject);

        SimpleFollow[] followList = FindObjectsOfType<SimpleFollow>();

        for(int i = 0; i < followList.Length; i++)
        {
            followList[i].UpdateFollowObject(currentKart.gameObject);
        }

        if (properties != null)
        {
            print($"{thisClass}: TiltFive board follow object {currentKart.name} detected");
            properties.UpdateFollowObject(currentKart.gameObject);
        }
    }

    private void ResetCameras(ArcadeKart kart)
    {
        Transform kartTransform = kart.gameObject.transform;
        Transform capsuleTransform = kart.gameObject.transform.Find("KartBouncingCapsule");

        //print($"{thisClass}:  Current tracking object is " + trackingObject.gameObject.name);

        foreach(GameObject camera in cameras)
        {
            if(camera.activeInHierarchy)
            {
                CinemachineVirtualCamera cinemachineCamera =
                    camera.GetComponent<CinemachineVirtualCamera>();

                if (cinemachineCamera != null)
                {
                    print($"{thisClass}: Cinemachine detected");
                    cinemachineCamera.m_LookAt = kartTransform;
                    cinemachineCamera.m_Follow = capsuleTransform;
                }
            }
        }
    }

    private void CheckCurrentCamera(GameObject newCamera)
    {
        print($"{thisClass}: New camera name is " + newCamera.name);

        ResetCameras(currentKart);
    }
}
