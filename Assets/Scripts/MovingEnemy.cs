using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    Transform CurrentTarget;
    [SerializeField] List<Transform> Targets;
    [SerializeField]int CurrentTargetIndext;
   
    [SerializeField] float MovementSpeed= 0.01f;
    [SerializeField] float EnemyRadius = 2f;


    void Start()
    {
        CurrentTargetIndext =0;
        CurrentTarget = Targets[CurrentTargetIndext];
    }


    void Update()
    {
        if(Vector3.Distance(transform.position, CurrentTarget.position) < EnemyRadius)
        {
            CurrentTargetIndext++;
            if(CurrentTargetIndext >= Targets.Count)
            {
                CurrentTargetIndext = 0;
            }
            
            CurrentTarget = Targets[CurrentTargetIndext];
        }
        transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.position, MovementSpeed * Time.deltaTime) ;

    }
}
