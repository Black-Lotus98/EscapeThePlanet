using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishState : MonoBehaviour, CollisionState
{
    public void Handle(CollisionHandler context)
    {
        Debug.Log("You hit the finish pad, FinishState");

        context.StartSuccessSequence();
    }
}
