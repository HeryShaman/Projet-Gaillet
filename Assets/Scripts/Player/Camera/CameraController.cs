using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera References")]
    public Transform Target;          // Ce que la caméra regarde (souvent le joueur)

    [Header("Camera Position")]
    public float CamLength;
    public float CamHeight;

    [Header("Field of View")]
    public float OriginalFov = 60f;
    public float targetFov;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (Target == null) { Debug.LogWarning("Assignation Manquante"); return; }
    }

    void LateUpdate()
    {
        CameraPosition();

        // Fonction test
        if (Input.GetKey(KeyCode.Space)) CameraZoom(OriginalFov + 5f, 5f);
        else CameraZoom(OriginalFov, 1f);
    }

    void CameraMovement()
    {

    }

    void CameraPosition()
    {
        // Position Actuel
        Vector3 CamPos = new Vector3(0, CamHeight, -CamLength);
        transform.position = Target.position + Quaternion.Euler(0, 0, 0) * CamPos;
        transform.LookAt(Target);
    }

    void CameraZoom(float newTargetFov, float zoomSpeed)
    {
        targetFov = newTargetFov;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * zoomSpeed);
    }
}