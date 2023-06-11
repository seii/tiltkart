using System;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalSelector : MonoBehaviour
{
    [Tooltip("Which GameObjects are the 'content' to scroll through?")]
    public GameObject[] content;
    [Tooltip("Which button will shift the contents to the previous item?")]
    public Button leftButton;
    [Tooltip("Which button will shift the contents to the next item?")]
    public Button rightButton;
    public static event Action<string, string> OnSelectionChange;

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
        content[currentContentIndex].SetActive(true);
        OnSelectionChange?.Invoke(this.gameObject.name, content[currentContentIndex].name);
    }

    private void updateFromPreference()
    {
        Debug.Log("Entering updateFromPreference");

        string canvasName = this.gameObject.name;
        string shortCanvas = this.gameObject.name.Substring(0, canvasName.LastIndexOf("Form"));

        Debug.Log("Canvas name is " + canvasName);
        string prefResult = PlayerPrefs.GetString(shortCanvas);
        Debug.Log("Preference result is: " + prefResult);

        if (!string.IsNullOrEmpty(prefResult))
        {
            Debug.Log("Found " + canvasName);
            string firstTest = prefResult + "Button";
            string secondTest = prefResult + "_Player";
            string thirdTest = prefResult + "Track";

            for (int i = 0; i < content.Length; i++)
            {
                Debug.Log("Checking " + content[i].name);
                if(firstTest.Equals(content[i].name) || secondTest.Equals(content[i].name) ||
                    thirdTest.Equals(content[i].name))
                {
                    Debug.Log("Found " + content[i].name);
                    changeVisibility(i);
                    break;
                }
            }
        }

        Debug.Log("Exiting updateFromPreference");
    }
}
