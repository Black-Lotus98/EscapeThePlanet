using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [SerializeField] int Damage;
    [SerializeField] GameObject BulletHitEffect;
    void OnCollisionEnter(Collision other)
    {
        Instantiate(BulletHitEffect,transform.position, Quaternion.identity);
        Destroy(gameObject);
    }  
}
