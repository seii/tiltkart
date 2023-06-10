using KartGame.KartSystems;
using UnityEngine;

public class ResetToStartingLine : MonoBehaviour
{
    public GameObject startingLine;

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
        }
        
    }
}
