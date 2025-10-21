using UnityEngine;

public abstract class MovementBehaviour : ScriptableObject
{
    // ApplyMovement r�cup�re l'entit�, son vecteur et le delta temps
    public virtual void ApplyMovement(GameObject entity, Vector3 wishVel)
    {
        Debug.LogError("ApplyMovement() doit �tre surcharg� dans les classes d�riv�es");
    }
}