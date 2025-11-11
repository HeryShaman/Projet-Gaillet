using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SphereGravityField : GravityField
{
    [Tooltip("Rayon de la zone de gravité (défini aussi automatiquement dans le collider).")]
    public float radius = 50f;

    private SphereCollider sphereCollider;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = radius;
    }

    public override Vector3 GetGravityDirection(Vector3 position)
    {
        return (transform.position - position).normalized;
    }

#if UNITY_EDITOR
    // Gizmo visible dans la scène pour debug
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
