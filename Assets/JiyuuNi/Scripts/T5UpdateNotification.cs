using UnityEngine;

/**
 * Create notification object when objectives are updated
 */
public class T5UpdateNotification : MonoBehaviour
{
    [SerializeField]
    private NotificationToast T5Notification;

    // Start is called before the first frame update
    protected void Start()
    {
        Objective.onUpdateObjective += OnUpdateObjective;
    }

    private void OnDisable()
    {
        Objective.onUpdateObjective -= OnUpdateObjective;
    }

    void OnUpdateObjective(UnityActionUpdateObjective objective)
    {
        if(!string.IsNullOrEmpty(objective.notificationText))
        {
            T5Notification.Initialize(objective.notificationText);
            T5Notification.gameObject.SetActive(true);
        }
    }
}
