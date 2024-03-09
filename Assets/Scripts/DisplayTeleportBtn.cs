using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTeleportBtn : MonoBehaviour
{

    public CustomTeleporter teleportPad;
    [SerializeField] TeleportButton teleportBtn;



    void Start()
    {
        Invoke("GetComponents", 0.1f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && teleportPad.enabled)
        {
            teleportBtn.TPBtn.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && teleportPad.enabled)
        {
            teleportBtn.TPBtn.SetActive(false);
        }
    }

    void GetComponents()
    {
        teleportBtn = GameObject.FindGameObjectWithTag("TPBtn").GetComponent<TeleportButton>();
        teleportPad = GetComponent<CustomTeleporter>();
        teleportBtn.TPBtn.SetActive(false);
    }
}
