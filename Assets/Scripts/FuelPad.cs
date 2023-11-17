using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPad : MonoBehaviour
{
    public bool isPlayerInside = false;


    public Player player;
    public AudioSource AS;

    [SerializeField] AudioClip FuelStationSound;


    public FuelPadState currentState;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        AS = gameObject.GetComponent<AudioSource>();

        currentState = new IdleState();
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateStats(this);
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
