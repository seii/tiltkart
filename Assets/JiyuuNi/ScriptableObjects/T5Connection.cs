using UnityEngine;

[CreateAssetMenu(menuName = "Tilt Five/Connection", order = 2, fileName = "New T5 Connection")]
public class T5Connection : ScriptableObject
{
    [SerializeField]
    private bool isActive = false;
    [SerializeField]
    private string currentKartName;

    public bool GetKnownState()
    {
        return isActive;
    }

    public void UpdateKnownState(bool newState)
    {
        isActive = newState;
    }

    public string GetCurrentKartName()
    {
        return currentKartName;
    }

    public void UpdateCurrentKartName(string newKartName)
    {
        if(newKartName != null)
        {
            currentKartName = newKartName;
        }else
        {
            Debug.LogError("T5Connection: Attempted to update kart, but new kart object is null");
        }
    }
}
