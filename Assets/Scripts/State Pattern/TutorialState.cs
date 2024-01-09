using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialState : MonoBehaviour, CollisionState
{
    public void Handle(CollisionHandler context)
    {
        context.FinishTutorial();
    }
}
