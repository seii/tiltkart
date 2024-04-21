using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Ensure that Canvas objects used as containers for Buttons are correctly
 * active and visible when required
 */
public class CanvasNavigation : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void LateUpdate()
    {
        if (EventSystem.current.currentSelectedGameObject == null ||
            !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
        {
            //Debug.Log("Current selected object has been deactivated, adjusting selection");
            Button[] buttons = this.GetComponentsInChildren<Button>();
                
            if(buttons != null && buttons.Length > 0)
            {
                EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
            }
        }
    }
}
