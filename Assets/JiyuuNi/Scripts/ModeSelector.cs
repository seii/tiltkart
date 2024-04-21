using UnityEngine;

/**
 * Track which type of objectives (the "mode") this specific race will use.
 * Also enable NPCs if necessary for that mode.
 */
public class ModeSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectiveManagers;

    private string modeName;

    private string thisClass = nameof(ModeSelector);

    // OnEnable is called before Start
    void OnEnable()
    {
        modeName = PlayerPrefs.GetString("Mode");

        bool modeSet = SetObjectiveManagerActiveIfExists();

        if(modeSet)
        {
            if (modeName.Equals("TimeTrial") || modeName.Equals("Laps"))
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
            print($"{thisClass}: Mode name is: " + modeName);
            foreach (GameObject target in objectiveManagers)
            {
                if (target.name.EndsWith(modeName))
                {
                    print($"{thisClass}: Setting objective manager active: " + target.name);
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

        if (checkpoints != null && checkpoints.Length > 0)
        {
            foreach (GameObject point in checkpoints)
            {
                //print($"{thisClass}: Verifying checkpoint type: " + point.name);
                if (point.name.Equals(modeName))
                {
                    //print($"{thisClass}: Activating checkpoint type: " + point.name);
                    point.SetActive(true);
                }
                else
                {
                    //print($"{thisClass}: Deactivating checkpoint type: " + point.name);
                    point.SetActive(false);
                }
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
