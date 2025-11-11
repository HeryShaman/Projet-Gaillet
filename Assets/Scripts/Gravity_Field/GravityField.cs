using UnityEngine;

public abstract class GravityField : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravityForce = 20f;
    public abstract Vector3 GetGravityDirection(Vector3 position);
}
