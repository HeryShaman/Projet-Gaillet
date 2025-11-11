using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyTransporter : EnemyBase
{
    [Header("Transporter Settings")]
    public float detectionPlayerRadius = 7f; // distance à laquelle il fuit le joueur
    public float infectDistance = 2f;        // distance pour infecter une cellule neutre
    public float fleeSpeedMultiplier = 1.5f; // vitesse de fuite
    public float searchInterval = 2f;        // temps entre recherches de cibles

    private NavMeshAgent agent;
    private Transform player;
    private EnemyReproducer targetReproducer; // cellule neutre ciblée
    private float nextSearchTime;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<CellController>().transform;
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= nextSearchTime)
        {
            UpdateTarget();
            nextSearchTime = Time.time + searchInterval;
        }

        HandleBehavior();
    }

    void UpdateTarget()
    {
        // Cherche toutes les cellules existantes
        EnemyReproducer[] reproducers = FindObjectsByType<EnemyReproducer>(FindObjectsSortMode.None);

        EnemyReproducer nearestNeutral = null;
        float minNeutralDist = Mathf.Infinity;

        EnemyReproducer nearestInfected = null;
        float minInfectedDist = Mathf.Infinity;

        foreach (var r in reproducers)
        {
            if (r == null) continue;
            float dist = Vector3.Distance(transform.position, r.transform.position);

            if (r.currentState == EnemyState.Neutral && dist < minNeutralDist)
            {
                nearestNeutral = r;
                minNeutralDist = dist;
            }
            else if (r.currentState == EnemyState.Infected && dist < minInfectedDist)
            {
                nearestInfected = r;
                minInfectedDist = dist;
            }
        }

        // Si le joueur est proche, on fuit vers une cellule infectée
        float playerDist = Vector3.Distance(transform.position, player.position);

        if (playerDist <= detectionPlayerRadius && nearestInfected != null)
        {
            targetReproducer = nearestInfected;
            agent.speed = MaxSpeed * fleeSpeedMultiplier;
        }
        else if (nearestNeutral != null)
        {
            targetReproducer = nearestNeutral;
            agent.speed = MaxSpeed;
        }
    }

    void HandleBehavior()
    {
        if (targetReproducer == null) return;

        agent.SetDestination(targetReproducer.transform.position);

        // Si on atteint une cellule neutre → on l’infecte
        float dist = Vector3.Distance(transform.position, targetReproducer.transform.position);
        if (targetReproducer.currentState == EnemyState.Neutral && dist <= infectDistance)
        {
            targetReproducer.SetInfected(true);
            Debug.Log($"{name} a infecté {targetReproducer.name} !");
        }
    }
}
