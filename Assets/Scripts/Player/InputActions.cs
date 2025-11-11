using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, GameInput.IControllerActions
{
    private GameInput _gameInput;

    // ====== Propriétés publiques ======
    public float MoveX { get; private set; }
    public float MoveZ { get; private set; }
    public Vector2 MoveDirection { get; private set; }

    // ====== Events ======
    public event Action<Vector2> MoveEvent;
    public event Action DashStartedEvent;
    public event Action DashReleasedEvent;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            _gameInput.Controller.SetCallbacks(this);
        }
        _gameInput.Controller.Enable();
    }

    private void OnDisable()
    {
        _gameInput.Controller.Disable();
    }

    // ====== Implémentations des actions ======
    public void OnDirection(InputAction.CallbackContext context)
    {
        Vector2 direction = Vector2.zero;

        if (context.performed)
            direction = context.ReadValue<Vector2>();
        else if (context.canceled)
            direction = Vector2.zero;

        // Mise à jour des propriétés
        MoveDirection = direction;
        MoveX = direction.x;
        MoveZ = direction.y;

        MoveEvent?.Invoke(direction);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
            DashStartedEvent?.Invoke();

        if (context.canceled)
            DashReleasedEvent?.Invoke();
    }

}