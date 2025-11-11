using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InfectionNode : MonoBehaviour
{
    public GameObject zonePrefab;
    public float zoneMaxRadius = 5f;
    public float zoneGrowSpeed = 1f;

    [HideInInspector] public InfectionManager manager;
    private InfectionZone zone;

    void Start()
    {
        // Crée la zone de ralentissement
        GameObject z = Instantiate(zonePrefab, transform.position, Quaternion.identity);
        zone = z.GetComponent<InfectionZone>();
        zone.Initialize(zoneMaxRadius, zoneGrowSpeed);
    }

    // Détruit le point (ex : dash dessus)
    public void DestroyNode()
    {
        if (zone != null)
            zone.EraseZone(); // gomme la zone progressivement
        manager.RemoveNode(this);
        Destroy(gameObject);
    }
}
