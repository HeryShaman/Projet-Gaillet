using UnityEngine;

public abstract class MovementBehaviour : ScriptableObject
{
    // ApplyMovement récupère l'entité, son vecteur et le delta temps
    public virtual void ApplyMovement(GameObject entity, Vector3 wishVel)
    {
        Debug.LogError("ApplyMovement() doit être surchargé dans les classes dérivées");
    }
}