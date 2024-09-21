using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShield : Command
{
    private readonly ShieldManager shieldManager;
    private readonly AudioClip shieldActivationSound;

    public ToggleShield(ShieldManager shieldManager, AudioClip shieldActivationSound)
    {
        this.shieldManager = shieldManager;
        this.shieldActivationSound = shieldActivationSound;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        if (shieldManager != null)
        {
            shieldManager.ToggleShield();
            shieldManager.NotifyObservers(UIState.ShieldChanged);
        }
        else
        {
            Debug.LogWarning("ShieldManager is null in ToggleShield command.");
        }
    }
}
