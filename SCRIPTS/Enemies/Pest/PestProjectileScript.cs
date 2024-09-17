using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PestProjectileScript : MonoBehaviour
{
    Collider[] colliders;
    public GameObject AttackHitFX_Prefab;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 7f);
    }

    void Update()
    {
        rb.AddForce(gameObject.transform.forward * 50f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy")
        {
            Explode(other);
        }
    }

    void Explode(Collider hitCollider)
    {
        Instantiate(AttackHitFX_Prefab, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
        
        colliders = Physics.OverlapSphere(gameObject.transform.position, 5f);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Player")
            {
                collider.transform.GetComponent<IDamageHandler>().TakeDamage(25f, collider);
            }
        }
    }
}
