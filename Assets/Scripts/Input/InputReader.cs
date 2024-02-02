using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static PlayerInput;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public UnityAction<Vector2, InputActionPhase> Move = delegate {};
    public UnityAction<Vector2> Look = delegate {};

    PlayerInput input;
    public Vector3 Direction => input.Player.Move.ReadValue<Vector2>();

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>(), context.phase);
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>());
    }
    public void OnFire(InputAction.CallbackContext context)
    {

    }

    private void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerInput();
            input.Player.SetCallbacks(this);
        }
        input.Enable();
    }

    private void OnDisable()
    {
        if (input != null) input.Disable();
    }
}
