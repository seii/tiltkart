using System.Collections.Generic;
using UnityEngine;
using System;

public class TiltFiveObjectVisibility : MonoBehaviour
{
    public static Action<bool> OnGlassesStateChange;

    private bool glassesActive = true;
    private bool firedOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log($"Glasses state on start: {glassesActive}");
        UpdateGlassesState();
        //Debug.Log($"Glasses state after start: {glassesActive}");
    }

    private void Update()
    {
        UpdateGlassesState();
    }

    private void UpdateGlassesState()
    {
        bool glassesAvailable = TiltFive.Display.GetGlassesAvailability();
        //Debug.Log($"Glasses active: {glassesAvailable}");
        //Debug.Log($"Last known state: {glassesActive}");

        if (glassesActive && !glassesAvailable)
        {
            Debug.Log("Glasses disconnected!");
            glassesActive = false;
            OnGlassesStateChange?.Invoke(false);
        }

        if (!glassesActive && glassesAvailable)
        {
            Debug.Log("Glasses connected!");
            glassesActive = true;
            OnGlassesStateChange?.Invoke(true);
        }

        if(glassesActive && glassesAvailable && !firedOnce)
        {
            Debug.Log("Glasses already connected, setting up for new scene");
            firedOnce = true;
            OnGlassesStateChange?.Invoke(true);
        }
    }
}
