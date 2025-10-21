using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    [Header("Entité")]
    public GameObject Entity;

    [Header("Mouvements")]
    public List<MovementBehaviour> MoveClass = new List<MovementBehaviour>();

    [Header("Debug Moves")]
    public bool DebugMode = false;
    public Vector3 WishVel = Vector3.zero;
    public int MoveIndex = 0;

    [Header("Debug Gizmos")]
    public float GizmoSphereScale = 0.25f;
    public float DebugVelocityDistance = 3f;
    public float DebugWishvelDistance = 1.5f;

    [Header("Debug Path")]
    public Color TracerColor = Color.yellow;
    public float TracerTimeLimit = 10f;
    public float TracerInterval = 0.2f;
    public float TracerSphereScale = 0.2f;

    private Queue<(Vector3 position, float timeCreated)> tracerPoints = new();
    private float tracerTimer = 0f;

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ToggleDebug();
        }

        if (Entity == null || MoveClass.Count == 0 || MoveClass[MoveIndex] == null)
            return;

        // Détection dynamique du type d'entité
        var charController = Entity.GetComponent<CharacterController>();
        var rb = Entity.GetComponent<Rigidbody>();

        // Appliquer le comportement actif
        MoveClass[MoveIndex].ApplyMovement(Entity, WishVel);

        // Si on utilise CharacterController, le déplacement peut être fait dans ApplyMovement
        // ou ici si nécessaire (ex: WishVel * delta)
        if (charController != null)
        {
            // Exemple : ne rien faire ici si ApplyMovement gère Move()
        }

        // Rigidbody : ApplyMovement s'occupe de rb.velocity

        // Debug visuel
        if (DebugMode == true)
        {
            DebugMove();
            HandleTracer();
        }
    }

    public void ToggleDebug()
    {
        DebugMode = !DebugMode;
    }

    // Mets en place un mouvement dans l'array du système
    public void SetMovement(int index)
    {
        if (MoveClass.Count == 0)
        {
            Debug.LogWarning("Aucun comportement de mouvement défini dans MoveClass.");
            return;
        }

        MoveIndex = (index + MoveClass.Count) % MoveClass.Count;
    }

    private void HandleTracer()
    {
        tracerTimer += Time.deltaTime;

        if (Entity != null && tracerTimer >= TracerInterval)
        {
            tracerPoints.Enqueue((Entity.transform.position, Time.time));
            tracerTimer = 0f;
            //Debug.Log("Tracer ajouté. Total points : " + tracerPoints.Count);
        }

        while (tracerPoints.Count > 0 && Time.time - tracerPoints.Peek().timeCreated > TracerTimeLimit)
        {
            tracerPoints.Dequeue();
            //Debug.Log("Tracer retiré. Reste points : " + tracerPoints.Count);
        }
    }

    // Change l'état du mouvement
    private void DebugMove()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetMovement(MoveIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetMovement(MoveIndex - 1);
        }
    }

    private void OnDrawGizmos()
    {
        if (!DebugMode) // Dessine si actif
            return;
        DrawDebugGizmos();
    }

    // DrawDebugGizmos sert à deboger direction, velocité et position
    private void DrawDebugGizmos()
    {
        if (!DebugMode)
            return;

        if (Entity == null)
            return;

        var cc = Entity.GetComponent<CharacterController>();
        var rb = Entity.GetComponent<Rigidbody>();

        Vector3 currentVelocity = Vector3.zero;

        if (cc != null)
            currentVelocity = cc.velocity;
        else if (rb != null)
            currentVelocity = rb.linearVelocity;

        // === POINT DE DIRECTION SOUHAITÉE (WishVel) ===
        Vector3 wishStart = Entity.transform.position;
        Vector3 wishEnd = wishStart + WishVel.normalized * DebugWishvelDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(wishStart, 0.1f);  // origine du mouvement
        Gizmos.DrawSphere(wishEnd, GizmoSphereScale);    // direction visée

        // === POINT DE VÉLOCITÉ ACTUELLE ===
        if (currentVelocity.magnitude > 0f)
        {
            Vector3 velStart = Entity.transform.position;
            Vector3 velEnd = velStart + currentVelocity.normalized * DebugVelocityDistance;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(velStart, 0.1f); // origine de la vitesse
            Gizmos.DrawSphere(velEnd, GizmoSphereScale);   // fin de la projection de la vitesse
        }
        // === TRACEUR DE POSITION ===
        foreach (var point in tracerPoints)
        {
            float lifePercent = 1f - Mathf.Clamp01((Time.time - point.timeCreated) / TracerTimeLimit);
            Color faded = new Color(TracerColor.r, TracerColor.g, TracerColor.b, lifePercent);
            Gizmos.color = faded;
            Gizmos.DrawSphere(point.position, TracerSphereScale * lifePercent);
        }
    }
}
