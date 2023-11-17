using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishState : MonoBehaviour, CollisionState
{
    public void Handle(CollisionHandler context)
    {
        context.StartSuccessSequence();
    }
}
