using UnityEngine;

public class Character_Manager : MonoBehaviour
{
    public Transform Entity;
    public MovementSystem Move;

    // Bloc qui ralentit le joueur
    public GameObject SlowBlockPrefab;
    private GameObject slowBlockInstance;
    private bool isSlowed = false;
    private float slowFactor = 0.5f; // 50% speed

    // Bloc porté (le bloc de scale)
    public GameObject CarryingBlock;
    private bool isCarrying = false;

    void Start()
    {
        slowBlockInstance = Instantiate(SlowBlockPrefab, transform);
        slowBlockInstance.SetActive(false);
    }

    void ProcessInput()
    {
        Move.WishVel = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            Move.WishVel.z += 1;
        if (Input.GetKey(KeyCode.S))
            Move.WishVel.z -= 1;
        if (Input.GetKey(KeyCode.A))
            Move.WishVel.x -= 1;
        if (Input.GetKey(KeyCode.D))
            Move.WishVel.x += 1;

        Move.WishVel = Move.WishVel.normalized;
    }

    void Update()
    {
        ProcessInput();
    }

    // Appelé quand collision avec TreeResource
    public void OnEnterTreeResource()
    {
        if (!isSlowed)
        {
            isSlowed = true;
            slowBlockInstance.SetActive(true);
            slowBlockInstance.transform.localPosition = new Vector3(0, 2, 0);
        }
    }

    // Appelé quand on quitte le tree resource
    public void OnExitTreeResource()
    {
        if (isSlowed)
        {
            isSlowed = false;
            slowBlockInstance.SetActive(false);
        }
    }

    // Gestion bloc porté
    public void PickupBlock(GameObject block)
    {
        CarryingBlock = block;
        isCarrying = true;
        CarryingBlock.transform.SetParent(transform);
        CarryingBlock.transform.localPosition = new Vector3(0, 2, 0);
    }

    public void DropBlock()
    {
        if (CarryingBlock)
        {
            CarryingBlock.transform.SetParent(null);
            CarryingBlock = null;
            isCarrying = false;
        }
    }

    public bool IsCarrying() => isCarrying;
}


