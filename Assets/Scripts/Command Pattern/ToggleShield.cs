using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShield : Command
{
    ShieldManager shieldManager;
    AudioClip shieldActivationSound;

    public ToggleShield(ShieldManager aShieldManager, AudioClip aShieldActivationSound)
    {
        this.shieldManager = aShieldManager;
        this.shieldActivationSound = aShieldActivationSound;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        shieldManager.ToggleShield();
        shieldManager.NotifyObservers(UIState.ShieldChanged);
    }

}
