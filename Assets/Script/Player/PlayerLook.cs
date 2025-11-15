using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("References")] public Transform pivot;

    [Header("Camera Settings")] public InputActionReference lookAction;

    public float mouseSensitivity = 1.5f;
    public float minXAngle = -80f;
    public float maxXAngle = 80f;
    public bool invertY;

    private float rotationX;
    private float rotationY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        var look = Vector2.zero;
        if (lookAction != null) look = lookAction.action.ReadValue<Vector2>();

        // Delta mouse :
        var mouseX = look.x * mouseSensitivity;
        var mouseY = look.y * mouseSensitivity * (invertY ? 1f : -1f);

        rotationY += mouseX;
        rotationX += mouseY;
        rotationX = Mathf.Clamp(rotationX, minXAngle, maxXAngle);

        // Rotate :
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        if (pivot != null) pivot.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    private void OnEnable()
    {
        lookAction?.action.Enable();
    }

    private void OnDisable()
    {
        lookAction?.action.Disable();
    }

    public void SetEnabled(bool enabled)
    {
        if (lookAction != null && lookAction.action != null)
        {
            if (enabled)
                lookAction.action.Enable();
            else
                lookAction.action.Disable();
        }

        if (enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Delock de la cam :
    public void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}