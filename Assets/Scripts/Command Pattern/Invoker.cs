using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoker
{
    public void ExecuteCommand(Command command, Rigidbody rigidbody, AudioSource audioSource)
    {
        if (command != null)
        {
            command.Execute(rigidbody, audioSource);
        }
        else
        {
            Debug.Log($"Command is null");
        }
    }

}
