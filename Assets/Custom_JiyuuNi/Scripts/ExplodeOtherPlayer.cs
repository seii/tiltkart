using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOtherPlayer : MonoBehaviour
{
    /*public Rigidbody otherPlayerRB;
    public float explosionForce = 20f;
    public float verticalForce = -20f;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 playerPos = otherPlayerRB.gameObject.transform.position;
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), -1f, Random.Range(0f, 1f));
        otherPlayerRB.AddExplosionForce(explosionForce, playerPos + offset, 0f, verticalForce, ForceMode.Impulse);
    }*/

    public float radius = 5.0F;
    public float power = 10.0F;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody collidedKart = other.attachedRigidbody;

        /*Vector3 offset = new Vector3(Random.Range(-1f, 1f), -1f, Random.Range(0f, 1f));
        collidedKart.AddExplosionForce(power, transform.position, 0f, 3f, ForceMode.Impulse);*/

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        
        foreach (Collider hit in colliders)
        {
            //Rigidbody rb = hit.GetComponent<Rigidbody>();
            Rigidbody rb = hit.attachedRigidbody;
            Vector3 kartPos = rb.gameObject.transform.position;

            //if (rb != null && !rb.Equals(collidedKart))
            if (rb != null)
                rb.AddExplosionForce(power, kartPos + explosionPos, 0f, 3.0F, ForceMode.Impulse);
        }
    }
}
