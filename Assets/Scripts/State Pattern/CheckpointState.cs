using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointState : MonoBehaviour, CollisionState
{
    public void Handle(CollisionHandler context)
    {
        context.ActivateCheckpoint();
    }
}
