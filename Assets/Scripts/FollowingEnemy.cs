using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEnemy : MonoBehaviour
{
    [SerializeField] Transform CurrentTarget;
    Vector3 StartPostion;
    [SerializeField] float MovementSpeed= 0.01f;
     private void Start()
    {
        CurrentTarget = null;
        StartPostion = gameObject.GetComponent<Transform>().position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CurrentTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CurrentTarget = null;
        }
    }

    private void Update()
    {
        // try to go towards current target
        if (CurrentTarget != null && (CurrentTarget.position - transform.position).magnitude > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position,CurrentTarget.position,MovementSpeed*Time.deltaTime);
        }
        else if(CurrentTarget == null || (CurrentTarget.position == StartPostion))
        {
            transform.position = Vector3.MoveTowards(transform.position,StartPostion,MovementSpeed*Time.deltaTime);
        }
    }
}
