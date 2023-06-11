using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelector : MonoBehaviour
{
    public ObjectiveReachTargets[] objectiveManagers;

    private string modeName;

    // OnEnable is called before Start
    void OnEnable()
    {
        modeName = PlayerPrefs.GetString("Mode");

        bool modeSet = SetObjectiveManagerActiveIfExists();

        if(modeSet)
        {
            if(modeName.Equals("TimeTrial"))
            {
                setNPCsEnabled(true);
            }
            
            if (modeName.Equals("Crash"))
            {
                setNPCsEnabled(false);
            }

            activateModeCheckpoints();
        }
    }

    private bool SetObjectiveManagerActiveIfExists()
    {
        bool result = false;

        // Enable the appropriate Objective
        if (!string.IsNullOrEmpty(modeName))
        {
            Debug.Log("Mode name is: " + modeName);
            foreach (ObjectiveReachTargets target in objectiveManagers)
            {
                if (target.name.EndsWith(modeName))
                {
                    Debug.Log("Setting objective manager active: " + target.name);
                    target.gameObject.SetActive(true);
                    result = true;
                }
            }
        }

        return result;
    }

    private void activateModeCheckpoints()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("PlayerCheckpoints");

        foreach (GameObject point in checkpoints)
        {
            Debug.Log("Verifying checkpoint type: " + point.name);
            if (point.name.Equals(modeName))
            {
                Debug.Log("Activating checkpoint type: " + point.name);
                point.SetActive(true);
            }
            else
            {
                Debug.Log("Deactivating checkpoint type: " + point.name);
                point.SetActive(false);
            }
        }
    }

    private static void setNPCsEnabled(bool enabled)
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPCs");

        foreach (GameObject obj in npcs)
        {
            obj.SetActive(enabled);
        }
    }
}
