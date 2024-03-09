using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : MonoBehaviour
{
    GameObject teleportBtn;

    public GameObject TPBtn { get { return teleportBtn; } }

    void Start()
    {
        teleportBtn = GameObject.FindGameObjectWithTag("TeleportBtn").gameObject;
        teleportBtn.SetActive(false);
    }

}
