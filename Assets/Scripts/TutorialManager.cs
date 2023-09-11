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
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputHandler>().enabled = false;
    }
}

