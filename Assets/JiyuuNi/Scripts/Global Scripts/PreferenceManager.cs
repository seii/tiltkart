using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Manage preferences for the overall game. "Preferences" in this case means two
 * different things:
 * 1. Temporary values stored in memory by the game and wiped after each run
 * 2. Permanent values stored by Unity's `UnityEngine.PlayerPrefs` class
 */
public class PreferenceManager : Singleton<PreferenceManager>
{
    [SerializeField]
    private Camera TiltFiveCamera = null;
    [SerializeField]
    private GameObject TiltFiveBoard = null;
    
    private Dictionary<string, string> prefDict = new Dictionary<string, string>(3);

    private string thisClass = nameof(PreferenceManager);

    private enum Settings
    {
        Mode,
        Kart,
        Track
    }

    // Start is called before the first frame update
    protected void Start()
    {
        print($"{thisClass}: Entering Start phase for {this.gameObject.name}");

        // Uncomment the below line if PlayerPrefs are corrupted and need to
        // be reset. Put the comment back after the script runs, as no
        // preferences are retained as long as this line is uncommented!
        //PlayerPrefs.DeleteAll();

        // Populate local dictionary of preference names
        foreach (Settings setting in Enum.GetValues(typeof(Settings)))
        {
            string prefValue = "";

            prefValue = PlayerPrefs.GetString(setting.ToString());

            // If previous value exists for a preference, put that into the dictionary
            if (!string.IsNullOrEmpty(prefValue)) {
                print($"{thisClass}: PlayerPrefs already contained key \"{setting.ToString()}\", " +
                    $"setting value \"{prefValue}\" locally as well");
                prefDict.Add(setting.ToString(), prefValue);
            }else
            {
                print($"{thisClass}: PlayerPrefs does not contain a value for key \"{setting.ToString()}\", " +
                    "setting blank local value");
                prefDict.Add(setting.ToString(), "");
            }
        }
    }

    // Called before scene is destroyed
    new void OnDestroy()
    {
        base.OnDestroy();

        //print($"{thisClass}: prefDict size is {prefDict.Keys.Count}");

        print($"{thisClass}: Scene closing, attempting to persist preferences");

        foreach (Settings setting in Enum.GetValues(typeof(Settings)))
        {
            SetPref(setting.ToString(), prefDict[setting.ToString()]);
        }

        SaveAllPrefs();
    }
    
    public void SetPref(string key, string value)
    {
        print($"{thisClass}: Checking if preference \"{key}\" exists");
        if(string.IsNullOrEmpty(key))
        {
            print($"{thisClass}: No preference with name \"{key}\" found, skipping save");
        }else if(string.IsNullOrEmpty(value))
        {
            print($"{thisClass}: Empty value supplied for preference \"{key}\", skipping save");
        }
        else
        {
            print($"{thisClass}: Setting value \"{value}\" in preference \"{key}\"");

            if(!prefDict.ContainsKey(key))
            {
                prefDict.Add(key, value);
            }else
            {
                prefDict[key] = value;
            }
            
            PlayerPrefs.SetString(key, value);
        }
    }

    public string GetPref(string key)
    {
        string result = null;

        if (string.IsNullOrEmpty(key))
        {
            print($"{thisClass}: No preference with name \"{key}\" found, skipping load");
        }
        else if (prefDict.ContainsKey(key))
        {
            if(String.IsNullOrEmpty(prefDict[key]))
            {
                print($"{thisClass}: Preference \"{key}\" found with null or empty value, skipping load");
            }
            else
            {
                string tempValue = prefDict[key];
                print($"{thisClass}: Preference \"{key}\" found with value \"{tempValue}\"");

                result = tempValue;
            }
        }

        return result;
    }

    public Camera GetT5Camera()
    {
        return TiltFiveCamera;
    }

    public GameObject GetT5Board()
    {
        return TiltFiveBoard;
    }

    public void UpdateT5Board(Transform update)
    {
        TiltFiveBoard.transform.position = update.position;
        TiltFiveBoard.transform.rotation = update.rotation;
        TiltFiveBoard.transform.localScale = update.localScale;
    }

    public void SaveAllPrefs()
    {
        PlayerPrefs.Save();
    }

    private void UpdateDict(string key, string value)
    {
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            print($"{thisClass}: updateDict key is " + key + " and value is " + value);
            string shortKey = stripEnding(key);
            string shortValue = stripEnding(value);
            print($"{thisClass}: Shortened key is " + shortKey + ", shortened value is " + shortValue);

            SetPref(key, value);
        }
        else
        {
            print($"{thisClass}: updateDict key or value was blank, skipping");
        }
    }

    private string stripEnding(string value)
    {
        if(value.Contains("Form"))
        {
            return value.Substring(0, value.LastIndexOf("Form"));
        }

        if (value.Contains("Button"))
        {
            return value.Substring(0, value.LastIndexOf("Button"));
        }

        if (value.Contains("_Player"))
        {
            return value.Substring(0, value.LastIndexOf("_Player"));
        }

        if (value.Contains("Track"))
        {
            return value.Substring(0, value.LastIndexOf("Track"));
        }

        return "";
    }
}
