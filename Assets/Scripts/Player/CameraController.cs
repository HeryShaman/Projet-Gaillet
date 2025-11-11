using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera References")]
    public Transform target;          // Ce que la caméra regarde (souvent le joueur)
    public Transform camPlaceholder;  // Point où la caméra doit se placer (souvent un enfant du joueur)

    [Header("Field of View")]
    public float OriginalFOV = 60f;
    public float targetFOV;

    private Camera cam;

    void Start()
    {
        // S'assure que la caméra du GameObject est bien trouvée
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (camPlaceholder == null || target == null)
        {
            Debug.LogWarning("⚠️ CameraController : camPlaceholder ou target non assigné !");
            return;
        }
        transform.position = camPlaceholder.position;
        transform.LookAt(target);

        if (Input.GetKey(KeyCode.Space)) ZoomEffect(75f, 5f);
        else ZoomEffect(OriginalFOV, 1f);
    }

    void ZoomEffect(float newTargetFOV, float zoomSpeed)
    {
        targetFOV = newTargetFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

}
