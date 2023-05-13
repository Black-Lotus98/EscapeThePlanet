using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour
{ 
    [SerializeField] GameObject BulletPrefab;
    
    [SerializeField] Transform MuzzeltTransform;
    AudioSource AS;
    [SerializeField] AudioClip ShootingSound;
    
    [SerializeField] float BulletDelay;
    float CurrentCooldown;

    [SerializeField] float BulletSpeed;

    private void Start()
    {
        AS=gameObject.GetComponent<AudioSource>();    
    }
    
    private void Update()
    {
        CurrentCooldown -= Time.deltaTime;
        if(CurrentCooldown<=0f)
        {       
            Invoke("shootingProcess",1f);
            CurrentCooldown=BulletDelay;
        }            
    }
    void shootingProcess()
    {
        var Bullet = Instantiate(BulletPrefab, MuzzeltTransform.position,MuzzeltTransform.rotation);
        
        AS.PlayOneShot(ShootingSound);
        Bullet.GetComponent<Rigidbody>().velocity=MuzzeltTransform.forward * BulletSpeed;
    }

}
