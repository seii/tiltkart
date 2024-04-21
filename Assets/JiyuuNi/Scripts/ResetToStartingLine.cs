using KartGame.KartSystems;
using System;
using UnityEngine;

/**
 * Detect collision with a trigger, then reset the colliding `ArcadeKart`
 * back to the defined starting line if that trigger is detected.
 */
public class ResetToStartingLine : MonoBehaviour
{
    [SerializeField]
    private GameObject startingLine;

    public static event Action<string> OnOutOfBounds;

    private void OnTriggerEnter(Collider other)
    {
        // Detect all karts, not just the player
        ArcadeKart kart = other.GetComponentInParent<ArcadeKart>();

        if(kart != null)
        {
            kart.transform.position = startingLine.transform.position;
            kart.transform.rotation = Quaternion.identity;
            kart.Rigidbody.velocity = Vector3.zero;
            kart.Rigidbody.angularVelocity = Vector3.zero;
            OnOutOfBounds?.Invoke(kart.name);
        }
        
    }
}
