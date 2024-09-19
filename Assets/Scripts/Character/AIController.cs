using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField]
    private float chaseDistance = 5f;

    [Header("Aggrevate")]
    private float timeSinceAggrevated = Mathf.Infinity;
    [SerializeField]
    private float aggroCooldownTime = 3f;

    [SerializeField]
    private float shoutDistance = 5f;

    private Vector3 guardPosition;

    private Health health;
    private Mover mover;
    private Fighter fighter;
    private GameObject player;

    private void Awake()
    {
        health = GetComponent<Health>();
        player = GameObject.FindWithTag("Player");
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
        guardPosition = transform.position;
    }

    private void Update()
    {
        if (health.IsDead()) return;

        if (IsAggrevated() && fighter.CanAttack(player))
        {
            AttackBehaviour();
        }
        else
        {
            SuspicionBehaviour();
        }

        UpdateTimers();
    }

    private void UpdateTimers()
    {
        timeSinceAggrevated += Time.deltaTime;
    }

    private void SuspicionBehaviour()
    {
        GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    private void AttackBehaviour()
    {
        fighter.Attack(player);

        AggrevateNearByEnemies();
    }
    private void AggrevateNearByEnemies()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

        foreach (RaycastHit hit in hits)
        {
            AIController ai = hit.collider.GetComponent<AIController>();

            if (ai == null) continue;

            ai.Aggrevate();
        }
    }

    public void Aggrevate()
    {
        timeSinceAggrevated = 0;
    }

    private bool IsAggrevated()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
    }
}
