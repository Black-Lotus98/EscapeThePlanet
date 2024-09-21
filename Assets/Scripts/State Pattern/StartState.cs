using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : MonoBehaviour, CollisionState
{
    public void Handle(CollisionHandler context)
    {
        Debug.Log("You hit the launch pad, StartState ");
    }
}
