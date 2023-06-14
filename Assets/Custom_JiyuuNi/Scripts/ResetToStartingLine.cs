using KartGame.KartSystems;
using System;
using UnityEngine;

public class ResetToStartingLine : MonoBehaviour
{
    public GameObject startingLine;

    public static Action OnOutOfBounds;

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
            OnOutOfBounds?.Invoke();
        }
        
    }
}
