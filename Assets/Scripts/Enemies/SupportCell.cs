using UnityEngine;

public enum EnemyState
{
    Neutral,
    Infected
}

public class EnemyReproducer : EnemyBase
{
    [Header("Reproduction Settings")]
    public GameObject transporterPrefab;
    public GameObject hunterPrefab;

    public float RegenRate = 1f;
    public int MaxSpawnCount = 10;
    public int MinSpawnPerCycle = 2;
    public int MaxSpawnPerCycle = 2;
    public float HealthCostPerSpawn = 25f;
    public float SpawnInterval = 3f;

    private int currentSpawned = 0;
    private float nextSpawnTime = 0f;

    [Header("Infection Settings")]
    public EnemyState currentState = EnemyState.Neutral;
    public Material neutralMaterial;
    public Material infectedMaterial;
    private Renderer rend;

    protected override void Start()
    {
        base.Start();
        rend = GetComponentInChildren<Renderer>();
        UpdateVisualState();
    }

    protected override void Update()
    {
        base.Update();
        Regenerate();

        // reproduction uniquement si infectée
        if (currentState == EnemyState.Infected &&
            Time.time >= nextSpawnTime &&
            CurrentHealth >= HealthCostPerSpawn &&
            currentSpawned < MaxSpawnCount)
        {
            TrySpawnEnemies();
            nextSpawnTime = Time.time + SpawnInterval;
        }
    }

    private void Regenerate()
    {
        if (CurrentHealth < MaxHealth)
            CurrentHealth += RegenRate * Time.deltaTime;
    }

    private void TrySpawnEnemies()
    {
        int enemiesToSpawn = Random.Range(MinSpawnPerCycle, MaxSpawnPerCycle + 1);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, MaxSpawnCount - currentSpawned);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (CurrentHealth < HealthCostPerSpawn)
                break;

            GameObject prefabToSpawn = Random.value > 0.5f ? transporterPrefab : hunterPrefab;
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 1f;
            spawnPos.y = transform.position.y;

            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            CurrentHealth -= HealthCostPerSpawn;
            currentSpawned++;
        }
    }

    // ---------------------------
    // 🔹 Gestion des états d’infection
    // ---------------------------

    public void SetInfected(bool infected)
    {
        currentState = infected ? EnemyState.Infected : EnemyState.Neutral;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (rend == null) return;

        if (currentState == EnemyState.Infected && infectedMaterial != null)
            rend.material = infectedMaterial;
        else if (neutralMaterial != null)
            rend.material = neutralMaterial;
    }

    // ---------------------------
    // 🔹 Gestion des dégâts et guérison
    // ---------------------------

    public override void TakeDamage(float amount)
    {
        // Si la cellule est neutre, elle n’est pas affectée
        if (currentState == EnemyState.Neutral)
            return;

        // Si infectée, elle peut être “soignée” par le joueur
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            HealCell();
            return;
        }

        UpdateTargetScale();
    }

    private void HealCell()
    {
        SetInfected(false);
        CurrentHealth = MaxHealth;
        currentSpawned = 0; // réinitialise le compteur de spawn
        Debug.Log($"{name} a été soignée par le joueur !");
    }

    // 🔸 On empêche la destruction totale ici
    protected override void Die()
    {
        // Ne pas détruire si infectée (soignée à la place)
        if (currentState == EnemyState.Infected)
        {
            HealCell();
        }
        else
        {
            base.Die(); // détruire seulement si neutre
        }
    }
}
