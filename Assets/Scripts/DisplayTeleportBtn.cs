using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            // Route the shared on-screen button to THIS pad while the player stands on it.
            Button btn = teleportBtn.TPBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(teleportPad.TeleportNow); // guard against duplicate listeners
                btn.onClick.AddListener(teleportPad.TeleportNow);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && teleportPad.enabled)
        {
            Button btn = teleportBtn.TPBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(teleportPad.TeleportNow);
            }
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
