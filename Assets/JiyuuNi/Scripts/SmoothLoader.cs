using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Control progress of a slider bar used on a loading screen
 */
public class SmoothLoader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Slider to use as a progress bar")]
    private List<Slider> progressBar;

    private float currentValue;

    private string thisClass = nameof(SmoothLoader);

    // Start is called before the first frame update
    /**
     * <credit>
     * Patryk Galach - Reality Unit - https://www.patrykgalach.com/2021/02/15/smooth-scene-loading/
     * </credit>
     */
    void Start()
    {
        SceneSwitcher.loadProgress += UpdateSlider;

        foreach (Slider slider in progressBar)
        {
            if(slider.isActiveAndEnabled)
            {
                print($"{thisClass}: Found {slider.name}");
                slider.value = currentValue = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentValue > 0)
        {
            foreach (Slider slider in progressBar)
            {
                if (slider.isActiveAndEnabled)
                {
                    slider.value = currentValue;
                }
            }
        }

        if(currentValue >= 0.9f)
        {
            currentValue = 0;
        }
    }

    private void OnDisable()
    {
        SceneSwitcher.loadProgress -= UpdateSlider;
    }

    private void UpdateSlider(float progress)
    {
        currentValue = progress;
    }
}
