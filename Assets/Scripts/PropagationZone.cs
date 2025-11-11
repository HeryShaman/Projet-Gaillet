using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InfectionZone : MonoBehaviour
{
    private SphereCollider col;
    private float targetRadius;
    private float growSpeed;
    private bool erasing = false;

    public void Initialize(float maxRadius, float growSpeed)
    {
        this.targetRadius = maxRadius;
        this.growSpeed = growSpeed;
        col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.1f;
    }

    void Update()
    {
        if (!erasing && col.radius < targetRadius)
            col.radius += growSpeed * Time.deltaTime;
        else if (erasing)
        {
            col.radius -= growSpeed * 2f * Time.deltaTime;
            if (col.radius <= 0.1f)
                Destroy(gameObject);
        }
    }

    public void EraseZone()
    {
        erasing = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CellController player = other.GetComponent<CellController>();
            //if (player != null)
            //{
            //    // Ralentit le joueur
            //    player.ModifySpeed(1f); // vitesse limitée à 1
            //}
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CellController player = other.GetComponent<CellController>();
        //if (player != null && player.IsSprinting)
        //{
        //    EraseZone(); // spin gomme la zone
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CellController player = other.GetComponent<CellController>();
            //if (player != null)
            //    player.ModifySpeed(player.MaxSpeed); // vitesse normale
        }
    }
}
