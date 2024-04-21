using System.Collections.Generic;
using UnityEngine;

/**
 * Enable or disable GameObjects based on whether TiltFive glasses are connected or not
 */
public class TiltFiveObjectVisibility : MonoBehaviour
{
    [SerializeField]
    private T5Connection connection;

    [SerializeField]
    private List<GameObject> ObjectsToEnable;
    [SerializeField]
    private List<GameObject> ObjectsToDisable;

    private bool isStarted = false;

    private string thisClass = nameof(TiltFiveObjectVisibility);

    // Start is called before the first frame update
    private void Start()
    {
        //print($"{thisClass}: Glasses state on start: {glassesActive}");
        UpdateGlassesState();
        //print($"{thisClass}: Glasses state after start: {glassesActive}");
    }

    private void Update()
    {
        UpdateGlassesState();
    }

    private void OnDisable()
    {
        if(connection != null)
        {
            connection.UpdateKnownState(false);
        }
        else
        {
            print($"{thisClass}: Glasses already disconnected, skipping update of known state");
        }
    }

    private void UpdateGlassesState()
    {
        bool glassesAvailable = TiltFive.Display.GetGlassesAvailability();
        //print($"{thisClass}: Glasses active: {glassesAvailable}");
        //print($"{thisClass}: Last known state: {glassesActive}");

        // Glasses were previously active...
        if(connection.GetKnownState())
        {
            // ...and now are not
            if(!glassesAvailable)
            {
                print($"{thisClass}: TiltFiveObjectVisibility: Glasses disconnected!");
                ProcessGlassesState(glassesAvailable);
            }
            // ...and still are...
            else
            {
                // ...but we're in a new scene somehow, so the scene must have changed
                if(!isStarted)
                {
                    print($"{thisClass}: TiltFiveObjectVisibility: Possible scene switch, processing active/inactive objects again");
                    ProcessGlassesState(glassesAvailable);
                }
            }
        }
        // Glasses were previously inactive...
        else
        {
            // ... and now are active
            if (glassesAvailable)
            {
                print($"{thisClass}: TiltFiveObjectVisibility: Glasses connected!");
                ProcessGlassesState(glassesAvailable);
            }
        }
    }

    private void ProcessGlassesState(bool newState)
    {
        // Mark each scene as "started" the first time the glasses are updated in it
        if(!isStarted)
        {
            isStarted = true;
        }

        connection.UpdateKnownState(newState);

        foreach (GameObject obj in ObjectsToEnable)
        {
            print($"{thisClass}: TiltFiveObjectVisibility: Setting {obj.name} to {newState}");
            obj.SetActive(newState);
        }

        foreach (GameObject obj in ObjectsToDisable)
        {
            print($"{thisClass}: TiltFiveObjectVisibility: Setting {obj.name} to {newState}");
            obj.SetActive(!newState);
        }
    }
}
