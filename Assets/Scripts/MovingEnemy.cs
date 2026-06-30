using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour, IRespawnResettable
{
    Transform CurrentTarget;
    [SerializeField] List<Transform> Targets;
    [SerializeField]int CurrentTargetIndex;

    [SerializeField] float MovementSpeed= 0.01f;
    [SerializeField] float EnemyRadius = 2f;

    Vector3 spawnPosition;


    void Start()
    {
        spawnPosition = transform.position;
        if (Targets == null || Targets.Count == 0)
        {
            Debug.LogError("MovingEnemy has no targets assigned!", this);
            enabled = false;
            return;
        }
        CurrentTargetIndex = 0;
        CurrentTarget = Targets[CurrentTargetIndex];
    }

    // Respawn: return to the authored spawn position and restart the patrol route.
    public void ResetToSpawn()
    {
        transform.position = spawnPosition;
        if (Targets != null && Targets.Count > 0)
        {
            CurrentTargetIndex = 0;
            CurrentTarget = Targets[CurrentTargetIndex];
        }
    }


    void Update()
    {
        if(Vector3.Distance(transform.position, CurrentTarget.position) < EnemyRadius)
        {
            CurrentTargetIndex++;
            if(CurrentTargetIndex >= Targets.Count)
            {
                CurrentTargetIndex = 0;
            }
            
            CurrentTarget = Targets[CurrentTargetIndex];
        }
        transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.position, MovementSpeed * Time.deltaTime) ;

    }
}
