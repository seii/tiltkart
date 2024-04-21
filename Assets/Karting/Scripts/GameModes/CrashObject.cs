using UnityEngine;

/// <summary>
/// This class inherits from TargetObject and represents a CrashObject.
/// </summary>
public class CrashObject : TargetObject
{
    [Header("CrashObject")]
    [Tooltip("The VFX prefab spawned when the object is collected")]
    public ParticleSystem CollectVFX;

    [Tooltip("The position of the centerOfMass of this rigidbody")]
    public Vector3 centerOfMass;

    [Tooltip("Apply a force to the crash object to make it fly up onTrigger")]
    public float forceUpOnCollide;

    [Tooltip("If greater than 0, destroy object after this many seconds")]
    public float duration;

    Rigidbody m_rigid;

    private string thisClass = nameof(CrashObject);

    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_rigid.centerOfMass = centerOfMass;
        Register();
    }

    void OnCollect(Collider other)
    {
        active = false;
        if (CollectVFX)
            CollectVFX.Play();
               
        if (m_rigid) m_rigid.AddForce(forceUpOnCollide*Vector3.up, ForceMode.Impulse);

        Objective.OnUnregisterPickup(this);

        TimeManager.OnAdjustTime(TimeGained);

        if (duration > 0f) Destroy(this.gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        
        if ((layerMask.value & 1 << other.gameObject.layer) > 0 &&
            other.gameObject.CompareTag("Player"))
        {
            print($"{thisClass}: Collider {other.name} collected item {this.name}");
            OnCollect(other);
        }
    }
    
}
