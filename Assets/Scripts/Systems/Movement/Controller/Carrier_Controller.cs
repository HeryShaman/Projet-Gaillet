using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "Carrier", menuName = "Movement/CarrierController")]
public class CarriedController : MovementBehaviour
{
    private Vector3 velocity;

    [Header("Movement Settings")]
    public float MaxSpeed;
    public float Accel;
    public float Friction;
    public float Gravity;

    [Header("Carrying Settings")]
    public float CarriedCharges;
    public float MaxCarryPenalty;


    public override void ApplyMovement(GameObject entity, Vector3 wishVel)
    {
        var Controller = entity.GetComponent<CharacterController>();
        if (Controller.isGrounded)
        {
            if (wishVel.magnitude > 0.0f)
                Accelerate(entity, wishVel);

            else
            {
                Deccelerate();
            }
        }
        else
        {
            ApplyGravity();
        }
        Controller.Move(velocity * Time.deltaTime);
    }

    void Accelerate(GameObject entity, Vector3 wishvel)
    {
        wishvel = (entity.transform.TransformDirection(new Vector3(wishvel.x, 0f, wishvel.z))).normalized;
        velocity = Vector3.Lerp(
        velocity,
        wishvel * Mathf.Max(0f, MaxSpeed - Mathf.Min(MaxCarryPenalty, CarriedCharges)),
        Accel * Time.deltaTime
        );
    }   

    void Deccelerate()
    {
        velocity = Vector3.Lerp(velocity, Vector3.zero, Friction * Time.deltaTime);
    }

    void ApplyGravity()
    {
        velocity.y -= Gravity * Time.deltaTime;
    }
}
