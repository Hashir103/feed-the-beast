using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController CharacterController;
    private InputAction _moveAction, _lookAction, _jumpAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");

        _jumpAction.performed += OnJumpPerformed;

        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterController == null) return;

        // don't apply input if the player controller has disabled input
        if (!CharacterController.AcceptInput) return;

        Vector2 movementVector = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        CharacterController.Move(movementVector);

        Vector2 lookVector = _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;
        CharacterController.Rotate(lookVector);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        CharacterController.Jump();
    }
}
