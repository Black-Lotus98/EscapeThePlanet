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
        if (other.CompareTag("Player") && teleportPad.enabled)
        {
            teleportBtn.TPBtn.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && teleportPad.enabled)
        {
            teleportBtn.TPBtn.SetActive(false);
        }
    }

    void GetComponents()
    {
        GameObject tpBtnGO = GameObject.FindGameObjectWithTag("TPBtn");
        if (tpBtnGO == null)
        {
            Debug.LogError("DisplayTeleportBtn: No GameObject with tag 'TPBtn' found!", this);
            return;
        }
        teleportBtn = tpBtnGO.GetComponent<TeleportButton>();
        teleportPad = GetComponent<CustomTeleporter>();
        teleportBtn.TPBtn.SetActive(false);
    }
}
