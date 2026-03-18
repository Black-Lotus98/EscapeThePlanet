using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSoundTrigger : MonoBehaviour
{
    [SerializeField] GameObject StarCollectable;

    void Update()
    {
        if (StarCollectable == null)
        {
            Destroy(gameObject);
        }
    }


}
