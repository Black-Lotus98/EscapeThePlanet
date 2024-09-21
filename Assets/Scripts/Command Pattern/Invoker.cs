using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoker
{
    public void ExecuteCommand(Command command, Rigidbody rigidbody, AudioSource audioSource)
    {
        if (command == null)
        {
            Debug.LogWarning("Attempted to execute null command.");
            return;
        }
        
        if (rigidbody == null)
        {
            Debug.LogWarning("Rigidbody is null in command execution.");
            return;
        }
        
        try
        {
            command.Execute(rigidbody, audioSource);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing command: {e.Message}");
        }
    }
}
