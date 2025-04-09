using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        agent.SetDestination(destination);
        agent.speed = speed;
        agent.isStopped = false;
        StartCoroutine(DestinationCheck());
    }

    public void Cancel()
    {
      //  animator.SetTrigger("Stop");
        agent.isStopped = true;
    }

    public bool HasPath()
    {
        return agent.hasPath;
    }

    public void StopTheCoroutine()
    {
        StopAllCoroutines();
    }

    private IEnumerator DestinationCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        //   animator.SetTrigger("Stop");
                        yield break;
                    }
                }
            }
            
        }
    }
}