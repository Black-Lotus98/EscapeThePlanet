using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : MonoBehaviour
{
    GameObject teleportBtn;

    public GameObject TPBtn { get { return teleportBtn; } }

    void Start()
    {
        GameObject found = GameObject.FindGameObjectWithTag("TeleportBtn");
        if (found == null)
        {
            Debug.LogError("TeleportButton: No GameObject with tag 'TeleportBtn' found!", this);
            return;
        }
        teleportBtn = found;
        teleportBtn.SetActive(false);
    }

}
