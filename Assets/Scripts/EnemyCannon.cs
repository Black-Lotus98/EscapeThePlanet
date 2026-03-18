using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour
{ 
    [SerializeField] GameObject BulletPrefab;
    
    [SerializeField] Transform MuzzleTransform;
    AudioSource AS;
    [SerializeField] AudioClip ShootingSound;
    
    [SerializeField] float BulletDelay;
    float CurrentCooldown;

    [SerializeField] float BulletSpeed;

    private void Start()
    {
        AS = gameObject.GetComponent<AudioSource>();
        CurrentCooldown = BulletDelay;
    }

    private void Update()
    {
        CurrentCooldown -= Time.deltaTime;
        if (CurrentCooldown <= 0f)
        {
            shootingProcess();
            CurrentCooldown = BulletDelay;
        }
    }
    void shootingProcess()
    {
        var Bullet = Instantiate(BulletPrefab, MuzzleTransform.position, MuzzleTransform.rotation);

        if (ShootingSound != null)
            AS.PlayOneShot(ShootingSound);

        Rigidbody rb = Bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = MuzzleTransform.forward * BulletSpeed;
    }

}
