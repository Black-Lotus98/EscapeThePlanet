using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour, IRespawnResettable
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
        CurrentCooldown = DifficultyRuntime.BulletDelay(BulletDelay);
    }

    private void Update()
    {
        CurrentCooldown -= Time.deltaTime;
        if (CurrentCooldown <= 0f)
        {
            shootingProcess();
            CurrentCooldown = DifficultyRuntime.BulletDelay(BulletDelay);
        }
    }
    void shootingProcess()
    {
        var Bullet = Instantiate(BulletPrefab, MuzzleTransform.position, MuzzleTransform.rotation);

        if (ShootingSound != null)
            AS.PlayOneShot(ShootingSound);

        Rigidbody rb = Bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = MuzzleTransform.forward * DifficultyRuntime.BulletSpeed(BulletSpeed);
    }

    // Respawn: reset the firing cooldown (live bullets are cleared by the CheckpointManager).
    public void ResetToSpawn()
    {
        CurrentCooldown = DifficultyRuntime.BulletDelay(BulletDelay);
    }

}
