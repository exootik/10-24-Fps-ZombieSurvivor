using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class WeaponInputHandler : MonoBehaviour
{
    [Header("Input Actions")] public InputActionReference fireAction;

    public InputActionReference reloadAction;
    public InputActionReference switchNextAction;
    public InputActionReference scrollAction;

    public float scrollThreshold = 0.5f;

    private void OnEnable()
    {
        if (fireAction != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed += FirePerformed;
            fireAction.action.started += FireStarted;
            fireAction.action.canceled += FireCanceled;
        }

        if (reloadAction != null)
        {
            reloadAction.action.Enable();
            reloadAction.action.performed += ReloadPerformed;
        }

        if (switchNextAction != null)
        {
            switchNextAction.action.Enable();
            switchNextAction.action.performed += SwitchPerformed;
        }

        if (scrollAction != null)
        {
            scrollAction.action.Enable();
            scrollAction.action.performed += ScrollPerformed;
        }
    }


    private void OnDisable()
    {
        if (fireAction != null)
        {
            fireAction.action.performed -= FirePerformed;
            fireAction.action.started -= FireStarted;
            fireAction.action.canceled -= FireCanceled;
            fireAction.action.Disable();
        }

        if (reloadAction != null)
        {
            reloadAction.action.performed -= ReloadPerformed;
            reloadAction.action.Disable();
        }

        if (switchNextAction != null)
        {
            switchNextAction.action.performed -= SwitchPerformed;
            switchNextAction.action.Disable();
        }

        if (scrollAction != null)
        {
            scrollAction.action.performed -= ScrollPerformed;
            scrollAction.action.Disable();
        }
    }

    // Events consumers (WeaponManager, UI, etc.) can subscribe to
    public event Action onFirePerformed;
    public event Action onFireStarted;
    public event Action onFireCanceled;
    public event Action onReloadPerformed;
    public event Action onSwitchPerformed;
    public event Action onScrollUp;
    public event Action onScrollDown;


    // Callbacks that forward events
    private void FirePerformed(InputAction.CallbackContext ctx)
    {
        onFirePerformed?.Invoke();
    }

    private void FireStarted(InputAction.CallbackContext ctx)
    {
        onFireStarted?.Invoke();
    }

    private void FireCanceled(InputAction.CallbackContext ctx)
    {
        onFireCanceled?.Invoke();
    }

    private void ReloadPerformed(InputAction.CallbackContext ctx)
    {
        onReloadPerformed?.Invoke();
    }

    private void SwitchPerformed(InputAction.CallbackContext ctx)
    {
        onSwitchPerformed?.Invoke();
    }

    private void ScrollPerformed(InputAction.CallbackContext ctx)
    {
        var v = Vector2.zero;

        try
        {
            v = ctx.ReadValue<Vector2>();
        }
        catch
        {
            var f = ctx.ReadValue<float>();
            v = new Vector2(0f, f);
        }

        var y = v.y;

        if (y > scrollThreshold)
            onScrollUp?.Invoke();
        else if (y < -scrollThreshold)
            onScrollDown?.Invoke();
    }
}