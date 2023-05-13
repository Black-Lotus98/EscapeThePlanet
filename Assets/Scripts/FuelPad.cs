using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPad : MonoBehaviour
{
    [SerializeField] float refillCooldown;
    [SerializeField] float RefillSpeed = 1;
    [SerializeField] bool isPlayerInside = false;


    GameObject PlayerScript;
    Player player;
    AudioSource AS;

    [SerializeField] AudioClip FuelStationSound;

    private void Start()
    {
        PlayerScript = GameObject.FindGameObjectWithTag("Player");
        player = PlayerScript.GetComponent<Player>();
        AS = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isPlayerInside)
        {
            if (player.enabled)
            {
                if (refillCooldown > 0f)
                {
                    if (player.GetFuelCounter() < 60)
                    {
                        player.FuelUsage(RefillSpeed);
                        refillCooldown -= RefillSpeed * Time.deltaTime;
                    }
                    else if (player.GetFuelCounter() > 60)
                    {
                        AS.Stop();
                        player.SetFuelCounter(60);
                    }
                }
                else
                {
                    AS.Stop();
                    refillCooldown = 0;
                    this.enabled = false;
                }
                if ((!player.enabled) && (player.GetFuelCounter() > 0))
                {
                    player.enabled = true;
                }

            }
            else
            {
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInside = true;
            AS.Stop();
            AS.loop = true;
            AS.clip = FuelStationSound;
            AS.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInside = false;
        }
        AS.Stop();
    }
}
