using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSoundTrigger : MonoBehaviour
{
    AudioSource AS;
    [SerializeField] GameObject StarCollectable;
    void Start()
    {
        AS = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (StarCollectable == null)
        {
            Destroy(gameObject);
        }
    }


}
