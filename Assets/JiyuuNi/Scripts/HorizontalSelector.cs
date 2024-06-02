using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Populate a selector field whose value can be changed by selecting buttons
 * representing "left" and "right"
 */
public class HorizontalSelector : MonoBehaviour
{
    [SerializeField, Tooltip("Which GameObjects are the 'content' to scroll through?")]
    private GameObject[] content;
    [SerializeField, Tooltip("Which button will shift the contents to the previous item?")]
    private Button leftButton;
    [SerializeField, Tooltip("Which button will shift the contents to the next item?")]
    private Button rightButton;
    [SerializeField, Tooltip("Which TextMesh UI is showing the track's name?")]
    private TextMeshProUGUI textContainer;

    public static event Action<string, string> OnSelectionChange;

    private string thisClass = "HorizontalSelector";

    // Start with the first added item
    private int currentContentIndex = 0;

    void Start()
    {
        if (content != null && content.Length > 0)
        {
            updateFromPreference();

            // Event listener for "left" (previous) button
            leftButton.onClick.AddListener(() =>
            {
                previousContent();
            });

            // Event listener for "right" (next) button
            rightButton.onClick.AddListener(() =>
            {
                nextContent();
            });

            // If player doesn't choose any options, make sure the displayed
            //    option gets written to preferences
            PreferenceManager.Instance.SetPref(GetPrefNameFromForm(),
                MakePrefValue(content[0].name));
        }
    }

    private void nextContent()
    {
        if (currentContentIndex == content.Length - 1) {
            changeVisibility(0);
        }
        else
        {
            changeVisibility(currentContentIndex + 1);
        }
    }

    private void previousContent()
    {
        if (currentContentIndex == 0)
        {
            changeVisibility(content.Length - 1);
        }
        else
        {
            changeVisibility(currentContentIndex - 1);
        }
    }

    private void changeVisibility(int index)
    {
        content[currentContentIndex].SetActive(false);
        currentContentIndex = index;
        GameObject obj = content[currentContentIndex];
        obj.SetActive(true);
        string prefValue = MakePrefValue(obj.name);
        //print($"{thisClass}: Preference value reported as '{prefValue}'");
        PreferenceManager.Instance.SetPref(GetPrefNameFromForm(), prefValue);

        OnSelectionChange?.Invoke(this.gameObject.name, obj.name);
    }

    private string GetPrefNameFromForm()
    {
        string name = this.name.Substring(0, this.name.LastIndexOf("Form"));
        //print($"{thisClass}: Preference name reported as '{name}'");
        return name;
    }

    private string MakePrefValue(string name)
    {
        string tempName = name;

        //print($"{thisClass}: Altering value {obj}");
        
        string[] textEndings = { "Button", "_Player", "Track" };

        foreach (string ending in textEndings)
        {
            if (tempName.EndsWith(ending))
            {
                tempName = tempName.Substring(0, tempName.LastIndexOf(ending));
            }
        }

        if (textContainer != null)
        {
            textContainer.text = tempName;
        }

        return tempName;
    }

    private void updateFromPreference()
    {
        string canvasName = this.gameObject.name;
        string shortCanvas = this.gameObject.name.Substring(0, canvasName.LastIndexOf("Form"));

        //Debug.Log("Canvas name is " + canvasName);
        string prefResult = PreferenceManager.Instance.GetPref(shortCanvas);
        //Debug.Log("Preference result is: " + prefResult);

        if (!string.IsNullOrEmpty(prefResult))
        {
            //Debug.Log("Found canvas " + canvasName);
            string firstTest = prefResult + "Button";
            string secondTest = prefResult + "_Player";
            string thirdTest = prefResult + "Track";

            for (int i = 0; i < content.Length; i++)
            {
                //Debug.Log("Matching " + content[i].name);
                if(firstTest.Equals(content[i].name) || secondTest.Equals(content[i].name) ||
                    thirdTest.Equals(content[i].name))
                {
                    //Debug.Log("Found " + content[i].name);
                    changeVisibility(i);
                    break;
                }
            }
        }
    }
}
