using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject TutorialPlane;

    private void Start()
    {
        TutorialPlane.SetActive(true);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("TutorialManager: Player not found in scene!", this);
            return;
        }
        player.GetComponent<InputHandler>().enabled = false;
    }
}

