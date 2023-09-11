using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [SerializeField] int Damage;
    [SerializeField] GameObject BulletHitEffect;
    void OnCollisionEnter(Collision other)
    {
        var effect = Instantiate(BulletHitEffect,transform.position, Quaternion.identity);
        Destroy(effect);
        Destroy(gameObject);
    }  
}
