using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponManager : MonoBehaviour
{
    [Header("References")] public Transform playerCamera;

    public Transform weaponHolder;
    public WeaponInputHandler inputHandler;
    public List<Gun> weapons = new();
    public Gun currentGun;

    private readonly float switchCooldown = 0.12f;

    private Coroutine autoFireCoroutine;

    private int currentIndex = -1;
    private float lastSwitchTime = -10f;

    private void Start()
    {
        for (var i = 0; i < weapons.Count; i++)
            if (weapons[i] != null)
                weapons[i].gameObject.SetActive(false);

        if (weapons.Count > 0) EquipByIndex(0);

        if (currentGun != null && playerCamera != null) currentGun.SetCameraTransform(playerCamera);
        if (currentGun != null) currentGun.OnAmmoChanged += HandleAmmoChanged;
    }

    private void OnEnable()
    {
        if (inputHandler == null) return;

        inputHandler.onFirePerformed += HandleFirePerformed;
        inputHandler.onFireStarted += HandleFireStarted;
        inputHandler.onFireCanceled += HandleFireCanceled;
        inputHandler.onReloadPerformed += HandleReload;
        inputHandler.onSwitchPerformed += HandleSwitch;

        inputHandler.onScrollUp += HandleScrollUp;
        inputHandler.onScrollDown += HandleScrollDown;
    }

    private void OnDisable()
    {
        if (inputHandler == null) return;


        inputHandler.onFirePerformed -= HandleFirePerformed;
        inputHandler.onFireStarted -= HandleFireStarted;
        inputHandler.onFireCanceled -= HandleFireCanceled;
        inputHandler.onReloadPerformed -= HandleReload;
        inputHandler.onSwitchPerformed -= HandleSwitch;

        inputHandler.onScrollUp -= HandleScrollUp;
        inputHandler.onScrollDown -= HandleScrollDown;
    }

    public event Action<Gun> OnGunEquipped;
    public event Action<int, int> OnGunAmmoChanged;

    private void HandleFirePerformed()
    {
        // pour arme semi-auto :
        currentGun?.TryShoot();
    }


    private void HandleFireStarted()
    {
        // pour arme automatique :
        if (autoFireCoroutine == null) autoFireCoroutine = StartCoroutine(AutoFireCoroutine());
    }

    private void HandleFireCanceled()
    {
        // pour stoper tire des armes automatique :
        if (autoFireCoroutine != null)
        {
            StopCoroutine(autoFireCoroutine);
            autoFireCoroutine = null;
        }
    }

    private void HandleReload()
    {
        currentGun?.TryReload();
    }

    private void HandleSwitch()
    {
        EquipNext();
    }

    private void HandleScrollUp()
    {
        if (Time.time - lastSwitchTime < switchCooldown) return;
        EquipPrev();
        lastSwitchTime = Time.time;
    }

    private void HandleScrollDown()
    {
        if (Time.time - lastSwitchTime < switchCooldown) return;
        EquipNext();
        lastSwitchTime = Time.time;
    }

    private IEnumerator AutoFireCoroutine()
    {
        while (true)
        {
            currentGun?.TryShoot();
            yield return null;
        }
    }

    public void EquipNext()
    {
        if (weapons.Count == 0) return;
        var next = (currentIndex + 1) % weapons.Count;
        EquipByIndex(next);
    }

    public void EquipPrev()
    {
        if (weapons.Count == 0) return;
        var prev = (currentIndex - 1 + weapons.Count) % weapons.Count;
        EquipByIndex(prev);
    }

    public void EquipByIndex(int index)
    {
        if (index < 0 || index >= weapons.Count) return;
        if (currentIndex == index && currentGun != null) return;

        if (currentGun != null)
        {
            currentGun.OnAmmoChanged -= HandleAmmoChanged;
            currentGun.gameObject.SetActive(false);
        }

        // Nouveau Gun :
        currentIndex = index;
        currentGun = weapons[currentIndex];

        if (currentGun != null)
        {
            currentGun.gameObject.SetActive(true);

            currentGun.OnAmmoChanged += HandleAmmoChanged;

            OnGunEquipped?.Invoke(currentGun);
            OnGunAmmoChanged?.Invoke(currentGun.CurrentAmmo, currentGun.MaxAmmo);
        }
        else
        {
            OnGunEquipped?.Invoke(null);
            OnGunAmmoChanged?.Invoke(0, 0);
        }
    }

    public void EquipGun(Gun gun)
    {
        var idx = weapons.IndexOf(gun);
        if (idx >= 0) EquipByIndex(idx);
    }

    public void RegisterNewWeapon(Gun gun, bool equipImmediately = true)
    {
        if (gun == null) return;

        if (playerCamera != null) gun.SetCameraTransform(playerCamera);

        if (weaponHolder != null)
            gun.transform.SetParent(weaponHolder, false);
        else
            // fallback:
            gun.transform.SetParent(transform, false);

        gun.gameObject.SetActive(false);

        weapons.Add(gun);

        if (equipImmediately) EquipGun(gun);
    }

    private void HandleAmmoChanged(int current, int max)
    {
        OnGunAmmoChanged?.Invoke(current, max);
    }
}