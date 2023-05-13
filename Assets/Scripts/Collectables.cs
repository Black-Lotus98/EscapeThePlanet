using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField] ParticleSystem ExplosionEffect;
    [SerializeField] int amount;

    int keyCounter = 0;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player != null)
        {

            if (gameObject.tag == "FuelBarrel" && player.GetFuelCounter() < player.GetMaxFlightTime())
            {
                player.FuelBarrel(amount);
                Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                Destroy(gameObject);
            }

            if (gameObject.tag == "ShieldCollectable" && player.GetCurrentShieldTime() < player.GetShieldMaxTime())
            {
                player.IncreaseShieldTime(amount);
                Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                Destroy(gameObject);
            }

            if (gameObject.tag == "Key")
            {
                keyCounter++;
                Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                player.SetKeyStatus(true);
                Destroy(gameObject);
            }


            if (gameObject.tag == "StarCollectable")
            {
                int CollectedStar = player.GetCollectedStarsCounter();

                if (player.GetStarStatus(CollectedStar))
                {
                    player.UpdateStarsGUI();
                    Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                    player.SetCollectedStarsCounter(amount);
                    Destroy(gameObject);
                }
            }

        }

    }
}