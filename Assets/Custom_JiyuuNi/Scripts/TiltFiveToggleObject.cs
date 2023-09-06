using UnityEngine;
using UnityEngine.Events;

public class TiltFiveToggleObject : MonoBehaviour
{
    public UnityEvent OnGlassesConnect;
    public UnityEvent OnGlassesDisconnect;

    // Start is called before the first frame update
    void Start()
    {
        TiltFiveObjectVisibility.OnGlassesStateChange += emitState;
    }

    private void OnDestroy()
    {
        TiltFiveObjectVisibility.OnGlassesStateChange -= emitState;
    }

    private void emitState(bool isActive)
    {
        if (isActive)
        {
            //Debug.Log("Firing OnGlassesConnect");
            OnGlassesConnect.Invoke();
        }
        else
        {
            //Debug.Log("Firing OnGlassesDisconnect");
            OnGlassesDisconnect.Invoke();
        }
    }
}
