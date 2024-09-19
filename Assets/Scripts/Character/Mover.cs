using GameDevTV.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour, IAction, ISaveable
{
    [SerializeField]
    private float maxSpeed = 6f;
    private float maxNavPathLength = 40f;

    private NavMeshAgent navAgent;

    private Health health;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        navAgent.enabled = !health.IsDead();
        UpdateAnimator();
    }



    public bool CanMoveTo(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        if (!hasPath) return false;
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }

        if (GetPathLength(path) > maxNavPathLength) return false;

        return true;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float total = 0;
        if (path.corners.Length < 2) return total; //경로상 꺾이는 부분

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return total;
    }

    public void StartMoveAction(Vector3 destination, float speedFraction)
    {
        GetComponent<ActionScheduler>().StartAction(this);
        MoveTo(destination, speedFraction);
    }

    public void MoveTo(Vector3 destination, float speedFraction)
    {

        navAgent.destination = destination;
        navAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        navAgent.isStopped = false;
    }


    public void Cancel()
    {
        navAgent.isStopped = true;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = navAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    public object CaptureState()
    {
        return new SerializableVector3(transform.position);
    }

    public void RestoreState(object state)
    {
        SerializableVector3 position = (SerializableVector3)state;
        navAgent.enabled = false;
        transform.position = position.ToVector();
        navAgent.enabled = true;
        GetComponent<ActionScheduler>().CancelCurrentAction();
    }
}
