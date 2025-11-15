using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Gun : MonoBehaviour
{
    [Header("Data & Components")] public GunData gunData;

    public Transform gunMuzzle;
    public Animator animator;
    public GameObject bulletHolePrefab;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;

    [Header("Runtime refs")] public Transform cameraTransform;

    [Header("UI Feedback")]
    [Tooltip("Image à afficher quand on touche une cible (CrosshairHitMarker)")]
    public Image crosshairHitMarker;
    public float crosshairHitDuration = 0.5f;

    //public ParticleSystem zombieBloodEffect;
    //public GameObject bulletHitParticlePrefab;

    protected int currentAmmo;
    protected bool isReloading;
    protected float nextTimeToFire;

    protected Coroutine crosshairHitCoroutine;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => gunData != null ? gunData.magazineSize : 0;

    protected virtual void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        if (gunData != null)
        {
            currentAmmo = gunData.magazineSize;
            OnAmmoChanged?.Invoke(currentAmmo, gunData.magazineSize);
        }
        else
        {
            currentAmmo = 0;
        }


        // Fallback 
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

        if (crosshairHitMarker != null)
            SetImageAlpha(crosshairHitMarker, 0f);
    }

    // Event :
    public event Action<int, int> OnAmmoChanged;

    public void SetCameraTransform(Transform cam)
    {
        cameraTransform = cam;
    }

    public void TryReload()
    {
        if (isReloading || currentAmmo >= gunData.magazineSize) return;
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        //Debug.Log(gunData.gunName + " is reloading....");
        animator?.SetTrigger("Reload");
        //PlayReloadSound();
        yield return new WaitForSeconds(gunData.reloadTime);

        currentAmmo = gunData.magazineSize;
        isReloading = false;
        animator?.SetBool("IsEmpty", false);
        animator?.SetBool("LastBullet", false);
        OnAmmoChanged?.Invoke(currentAmmo, gunData.magazineSize);
    }

    public void TryShoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        if (currentAmmo <= 0)
        {
            animator?.SetBool("IsEmpty", true);
            //Debug.Log(gunData.gunName + " IsEmpty\", true");
            return;
        }

        if (currentAmmo == 1) animator.SetBool("LastBullet", true);

        nextTimeToFire = Time.time + 1f / gunData.fireRate;
        HandleShoot();
    }

    protected virtual void HandleShoot()
    {
        animator?.SetTrigger("Shoot");
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, gunData.magazineSize);
        //Debug.Log(gunData.gunName + " Shot! , Bullets left : " + currentAmmo);

        muzzleFlash?.Play();
        PlayFireSound();
        Shoot();
    }

    private void PlayFireSound()
    {
        if (audioSource != null && gunData != null && gunData.fireSound != null)
            audioSource.PlayOneShot(gunData.fireSound);
    }

    public void PlayReloadSoundEvent()
    {
        Debug.Log("On recharge ! ");
        if (audioSource != null && gunData != null && gunData.reloadSound != null)
            audioSource.PlayOneShot(gunData.reloadSound);
    }


    public abstract void Shoot();

    public void ShowCrosshairHit()
    {
        if (crosshairHitMarker == null) return;

        if (crosshairHitCoroutine != null) StopCoroutine(crosshairHitCoroutine);
        crosshairHitCoroutine = StartCoroutine(CrosshairHitCoroutine());
    }

    private IEnumerator CrosshairHitCoroutine()
    {
        SetImageAlpha(crosshairHitMarker, 1f);

        float elapsed = 0f;
        while (elapsed < crosshairHitDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / crosshairHitDuration);
            SetImageAlpha(crosshairHitMarker, a);
            yield return null;
        }

        SetImageAlpha(crosshairHitMarker, 0f);
        crosshairHitCoroutine = null;
    }

    private void SetImageAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        c.a = Mathf.Clamp01(a);
        img.color = c;
    }

}