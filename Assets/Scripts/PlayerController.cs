using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    [Tooltip("When false the PlayerController will ignore input (useful for menus)")]
    public bool AcceptInput = true;
    public float MovementSpeed = 10f;
    public float RotationSpeed = 5f;
    public float JumpForce = 10f;
    public float Gravity = -30f;
    private float _rotationY;
    private float _rotationX;
    private float _verticalVelocity;

    public Transform CameraTransform;
    public Vector3 CameraOffset = new Vector3(0f, 1.6f, 0f);
    public float LookSensitivity = 1f;
    public float MinPitch = -80f;
    public float MaxPitch = 80f;
    public bool LockCursor = true;

    public Transform BodyRoot;
    public Renderer[] BodyRenderers;
    public Renderer[] ExcludeRenderers;
    public bool AutoCollectBodyRenderers = true;

    private Renderer[] _cachedBodyRenderers;

    void Start()
    {
        // get camera and character
        _characterController = GetComponent<CharacterController>();
        if (CameraTransform == null && Camera.main != null)
        {
            CameraTransform = Camera.main.transform;
        }

        // initialize rotation
        _rotationY = transform.localEulerAngles.y;
        if (CameraTransform != null)
        {
            if (CameraTransform.IsChildOf(transform))
                _rotationX = CameraTransform.localEulerAngles.x;
            else
                _rotationX = CameraTransform.eulerAngles.x;

            if (!CameraTransform.IsChildOf(transform))
                CameraTransform.SetParent(transform, false);

            CameraTransform.localPosition = CameraOffset;
        }
    }

    public void Move(Vector2 movementVector)
    {
        Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
        move *= MovementSpeed;

        if (_characterController.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        _verticalVelocity += Gravity * Time.deltaTime;
        move.y = _verticalVelocity;

        _characterController.Move(move * Time.deltaTime);
    }

    public void Rotate(Vector2 rotationVector)
    {
        _rotationY += rotationVector.x * RotationSpeed * LookSensitivity;
        _rotationX -= rotationVector.y * RotationSpeed * LookSensitivity;
        _rotationX = Mathf.Clamp(_rotationX, MinPitch, MaxPitch);

        transform.localRotation = Quaternion.Euler(0f, _rotationY, 0f);

        if (CameraTransform != null)
        {
            if (CameraTransform.IsChildOf(transform))
            {
                CameraTransform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
            }
            else
            {
                CameraTransform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
            }
        }
    }

    public void Jump()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity = JumpForce;
        }
    }

    void Update()
    {
        if (!AcceptInput) return;
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        if (LockCursor && mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        float h = 0f, v = 0f;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h += 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v -= 1f;
        }
        Vector2 ni = new Vector2(h, v);
        if (ni.sqrMagnitude > 1f) ni.Normalize();
        Move(ni);

        Vector2 mouseDelta = mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
        Rotate(mouseDelta * 0.1f);

        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }

        if (CameraTransform != null && CameraTransform.IsChildOf(transform))
        {
            CameraTransform.localPosition = CameraOffset;
        }
    }
}
