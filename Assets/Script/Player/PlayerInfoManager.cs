using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour, IDamageable
{
    [Header("Player Health & Damage")] public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damagePanel;
    public float flashAlpha = 0.5f;
    public float fadeDuration = 1f;
    public GameObject deathScreen;

    public InputActionReference pBindTestActionRef;

    public UiManager uiManager;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnEnable()
    {
        if (pBindTestActionRef != null)
        {
            pBindTestActionRef.action.performed += OnBindTestPerformed;
            pBindTestActionRef.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pBindTestActionRef != null)
        {
            pBindTestActionRef.action.performed -= OnBindTestPerformed;
            pBindTestActionRef.action.Disable();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null) healthSlider.value = currentHealth;

        StopCoroutine("FlashRoutine");
        StartCoroutine("FlashRoutine");


        if (currentHealth <= 0) Die();
    }

    private void OnBindTestPerformed(InputAction.CallbackContext ctx)
    {
        // .performed correspond � l'appui (pour les actions de type Button)
        TakeDamage(5);
    }

    public void RestoreMaxHealth()
    {
        currentHealth = maxHealth;
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    private void Die()
    {
        deathScreen.SetActive(true);
        uiManager?.PauseGame(false);
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        deathScreen.SetActive(false);
        uiManager?.ResumeGame();
        TeleportToSpawn();
    }

    private IEnumerator FlashRoutine()
    {
        SetPanelAlpha(flashAlpha);

        var elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            var a = Mathf.Lerp(flashAlpha, 0f, elapsed / fadeDuration);
            SetPanelAlpha(a);
            yield return null;
        }

        SetPanelAlpha(0f);
    }

    private void SetPanelAlpha(float a)
    {
        if (damagePanel == null) return;
        var color = damagePanel.color;
        color.a = a;
        damagePanel.color = color;
    }

    private void TeleportToSpawn()
    {
        Vector3 targetPos = new Vector3(56f, -10f, -7f);

        var charController = GetComponent<CharacterController>();
        if (charController != null)
        {
            charController.enabled = false;
            transform.position = targetPos;
            charController.enabled = true;
            return;
        }
        transform.position = targetPos;
    }
}