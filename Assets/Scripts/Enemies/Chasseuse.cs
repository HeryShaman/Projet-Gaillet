using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyHunter : EnemyBase
{
    [Header("Hunter Settings")]
    public float rangeAroundReproducer = 5f;
    public float rangeAroundTransporter = 2f;
    public float attackRange = 2f;
    public float drainRate = 10f; // vitalité drainée par seconde au joueur

    private NavMeshAgent agent;
    private Transform player;
    private EnemyReproducer nearestReproducer;
    private EnemyTransporter nearestTransporter;
    private float wanderTimer = 2f;
    private float timer;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<CellController>().transform;
        timer = wanderTimer;
    }

    protected override void Update()
    {
        base.Update();

        float playerDist = Vector3.Distance(transform.position, player.position);

        if (playerDist <= attackRange)
        {
            AttackPlayer();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            UpdateTargets();
            WanderAroundTargets();
            timer = 0f;
        }
    }

    void UpdateTargets()
    {
        nearestReproducer = FindNearest<EnemyReproducer>();
        nearestTransporter = FindNearest<EnemyTransporter>();
    }

    void WanderAroundTargets()
    {
        Vector3 destination = transform.position;

        if (nearestReproducer != null)
            destination = RandomNavSphere(nearestReproducer.transform.position, rangeAroundReproducer);
        else if (nearestTransporter != null)
            destination = RandomNavSphere(nearestTransporter.transform.position, rangeAroundTransporter);

        agent.SetDestination(destination);
    }

    void AttackPlayer()
    {
        agent.SetDestination(player.position);

        // Drain la vitalité du joueur et perd de la vie
        var controller = player.GetComponent<CellController>();
        if (controller != null)
        {
            controller.CurrentVitality -= drainRate * Time.deltaTime;
            TakeDamage(Time.deltaTime * 1f); // le chasseur s'épuise doucement
            Debug.Log($"{name} draine {drainRate * Time.deltaTime:F1} vitalité au joueur !");
        }
    }

    T FindNearest<T>() where T : EnemyBase
    {
        T[] enemies = FindObjectsByType<T>(FindObjectsSortMode.None);
        T nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = e;
            }
        }

        return nearest;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist + origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }
}
