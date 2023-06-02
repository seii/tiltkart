using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreferenceManager : MonoBehaviour
{
    [Tooltip("Which panels hold the values to be saved as preferences?")]
    public UIPanel[] canvasNames = null;
    //public static event Action<string, string> OnPreferenceLoaded;

    private Dictionary<string, string> prefDict = new Dictionary<string, string> {
        { "Mode", "LapRace" },
        { "Kart", "KartClassic" },
        { "Track", "Oval" }
    };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Entering Start for " + this.gameObject.name);
        //PlayerPrefs.DeleteAll();

        Dictionary<string, string> tempPrefDict = new Dictionary<string, string>(prefDict.Count);

        foreach ((string shortCanvas, string shortButton) in prefDict)
        {
            if (PlayerPrefs.HasKey(shortCanvas))
            {
                string tempValue = PlayerPrefs.GetString(shortCanvas);
                Debug.Log("PlayerPrefs already contained key '" + shortCanvas + "', setting prefDict to value '" + tempValue + "'");
                tempPrefDict[shortCanvas] = tempValue;
            }
            else
            {
                Debug.Log("No new values detected in PlayerPrefs, using value " + shortButton + " for key " + shortCanvas);
                tempPrefDict[shortCanvas] = shortButton;
            }
        }

        prefDict = tempPrefDict;
        tempPrefDict = null;

        HorizontalSelector.OnSelectionChange += updateDict;
        ChooseSceneFromPreference.OnScenePreLoad += trackUpdate;

        // Populate default preferences on first launch, but don't overwrite if they already exist
        if(canvasNames != null && canvasNames.Length > 0)
        {
            Debug.Log("Canvas names exist, loading preferences");
            foreach(UIPanel canvas in canvasNames)
            {
                string shortName = stripEnding(canvas.gameObject.name);
                Debug.Log("Attempting to load preference " + shortName);
                //loadPreferenceObjectsFromCanvas(canvas);
                loadPrefIfExists(shortName);
            }
        }else
        {
            Debug.Log("No canvases have been set, moving on");
        }
        Debug.Log("Exiting Start");
    }

    // Called before scene is destroyed
    void OnDestroy()
    {
        Debug.Log("Entering onDestroy");
        Debug.Log("prefDict size is: " + prefDict.Keys.Count);

        HorizontalSelector.OnSelectionChange -= updateDict;

        if(canvasNames != null && canvasNames.Length > 0)
        {
            Debug.Log("Attempting to set preferences");
            foreach ((string canvasName, string buttonName) in prefDict)
            {
                Debug.Log("onDestroy attempts to set preference '" + canvasName + "' to '" + buttonName + "'");
                setPref(canvasName, buttonName);
            }

            PlayerPrefs.Save();
        }
        

        //PlayerPrefs.Save();

        /*foreach (UIPanel canvas in canvasNames)
        {
            setPreferenceObjectsFromCanvas(canvas);
        }*/
        Debug.Log("Exiting onDestroy");
    }
    
    private void setPref(string key, string value)
    {
        Debug.Log("Entering setPref");
        Debug.Log("Checking if preference " + key + " exists...");
        if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            Debug.Log("Preference key or value was empty, skipping!");
        }
        else
        {
            Debug.Log("Setting PlayerPrefs key " + key + " to " + value);
            PlayerPrefs.SetString(key, value);
        }
        
        Debug.Log("Exiting setPref");
    }

    private void loadPrefIfExists(string key)
    {
        Debug.Log("Entering loadPrefIfExists");

        if (string.IsNullOrEmpty(key))
        {
            Debug.Log("Key or value to be loaded was empty, skipping!");
        }
        else if (PlayerPrefs.HasKey(key))
        {
            string tempValue = PlayerPrefs.GetString(key);
            Debug.Log("PlayerPrefs reports value '" + tempValue + "' for key '" + key + "'");
            prefDict[key] = tempValue;
            //OnPreferenceLoaded?.Invoke(key, tempValue);
        }else
        {
            Debug.Log("No preference by the name " + key + " exists, moving on");
        }
        Debug.Log("Exiting loadPrefIfExists");
    }

    private void updateDict(string key, string value)
    {
        Debug.Log("Entering updateDict");

        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            Debug.Log("updateDict key is " + key + " and value is " + value);
            string shortKey = stripEnding(key);
            string shortValue = stripEnding(value);
            Debug.Log("Shortened key is " + shortKey + ", shortened value is " + shortValue);

            if (prefDict.ContainsKey(shortKey))
            {
                Debug.Log("prefDict already has key " + shortKey + ", updating with " + shortValue);
                prefDict[shortKey] = shortValue;
            }
            else
            {
                Debug.Log("prefDict does not contain the key " + shortKey + ", adding it with value " + shortValue);
                prefDict.Add(shortKey, shortValue);
            }
        }
        else
        {
            Debug.Log("updateDict key or value was blank, skipping");
        }

        Debug.Log("Exiting updateDict");
    }

    private void trackUpdate()
    {
        if(!string.IsNullOrEmpty(prefDict["Track"]))
        {
            Debug.Log("Quick setting 'Track' preference to '" + prefDict["Track"] + "'");
            PlayerPrefs.SetString("Track", prefDict["Track"]);
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
