using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CellController : MonoBehaviour
{
    [Header("Movement")]
    public float MaxSpeed = 5f;
    public float Accel = 5f;
    public float Friction = 5f;
    public float Gravity = 9.81f;

    [Header("Sprint")]
    public float SprintSpeed = 10f;
    public float SprintAccel = 5f;
    public bool IsCharging;

    [Header("Vitality")]
    public float CurrentVitality;
    public float StandingRecharge = 5f;
    public float MovingRecharge = 2.5f;
    public float DecreaseMultiplier = 5f;
    public float MaxVitality = 100f;

    [Header("Graphics")]
    public Transform PlayerModel;
    private float MinScale = 1.0f;
    private float MaxScale = 2.0f;
    public float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private InputReader inputreader;

    private CharacterController controller;
    private Vector2 MoveInput;
    private Vector3 velocity;
    private GravityField currentField;
    private float CurrentSpeed;
    private bool isTakingDamage;
    private float damageCooldown = 3f;
    private float lastDamageTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputreader.MoveEvent += OnDirection;
        inputreader.DashStartedEvent += OnBoostStart;
        inputreader.DashReleasedEvent += OnBoostRelease;
    }

    void OnDestroy()
    {
        inputreader.MoveEvent -= OnDirection;
        inputreader.DashStartedEvent -= OnBoostStart;
        inputreader.DashReleasedEvent -= OnBoostRelease;
    }

    private void Update()
    {
        bool canRegen = !isTakingDamage || Time.time - lastDamageTime > damageCooldown;

        if (IsCharging && CurrentVitality > 0)
            BoostMovement();
        else
        {
            if (canRegen) RegenerateVitality();
            HandleMovement();
        }

        UpdateScale();
    }

    void UpdateScale()
    {
        float normalized = Mathf.Clamp01(CurrentVitality / MaxVitality);
        float targetScale = Mathf.Lerp(MinScale, MaxScale, normalized);
        PlayerModel.localScale = Vector3.one * targetScale;
    }

    void HandleMovement()
    {
        CurrentSpeed = MaxSpeed;

        if (controller.isGrounded)
        {
            if (MoveInput.magnitude > 0.1f)
            {
                velocity.x = Mathf.Lerp(velocity.x, MoveInput.x * CurrentSpeed, Accel * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, MoveInput.y * CurrentSpeed, Accel * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0, Friction * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, 0, Friction * Time.deltaTime);
            }
        }

        Vector3 lookDir = new Vector3(velocity.x, 0f, velocity.z);
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            PlayerModel.rotation = Quaternion.Lerp(PlayerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        velocity.y -= Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void BoostMovement()
    {
        if (controller.isGrounded)
        {
            CurrentSpeed = SprintSpeed;
            velocity.x = Mathf.Lerp(velocity.x, MoveInput.x * CurrentSpeed, SprintAccel * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, MoveInput.y * CurrentSpeed, SprintAccel * Time.deltaTime);
            CurrentVitality -= DecreaseMultiplier * Time.deltaTime;
        }

        Vector3 lookDir = new Vector3(velocity.x, 0f, velocity.z);
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            PlayerModel.rotation = Quaternion.Lerp(PlayerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void RegenerateVitality()
    {
        if (MoveInput.magnitude >= 0.1f)
            CurrentVitality += MovingRecharge * Time.deltaTime;
        else
            CurrentVitality += StandingRecharge * Time.deltaTime;

        CurrentVitality = Mathf.Clamp(CurrentVitality, 0, MaxVitality);
    }

    public void TakeDamage(float amount)
    {
        CurrentVitality -= amount;
        if (CurrentVitality < 0) CurrentVitality = 0;
        lastDamageTime = Time.time;
        isTakingDamage = true;
    }

    private void OnDirection(Vector2 dir) => MoveInput = dir;
    private void OnBoostStart() => IsCharging = true;
    private void OnBoostRelease() => IsCharging = false;

    // ✅ Nouvelle version : inflige des dégâts aux ennemis au contact
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            float damage = 10f;
            enemy.TakeDamage(damage);
            Debug.Log($"Joueur inflige {damage} dégâts à {enemy.name} !");
        }
    }
}
