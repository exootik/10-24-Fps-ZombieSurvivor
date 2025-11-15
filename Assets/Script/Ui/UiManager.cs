using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Weapon refs")] public WeaponManager weaponManager;

    public TMP_Text ammoText;

    [Header("Menu Pause")] public InputActionReference echapAction;

    public GameObject panelPause;
    public Button buttonContinue;
    public Button buttonQuit;

    [Header("Player refs")] public PlayerLook playerLook;

    public PlayerMovement playerMovement;
    public PlayerInteraction playerInteraction;


    public TMP_Text ScoreText;

    private Gun currentSubscribedGun;

    private void OnEnable()
    {
        if (weaponManager != null)
        {
            weaponManager.OnGunEquipped += OnGunEquipped;
            weaponManager.OnGunAmmoChanged += OnGunAmmoChanged;
        }

        if (echapAction != null)
        {
            echapAction?.action.Enable();
            echapAction.action.performed += OnEchapPerformed;
        }

        if (buttonContinue != null) buttonContinue.onClick.AddListener(ResumeGame);

        if (buttonQuit != null) buttonQuit.onClick.AddListener(QuitToMainMenu);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
    }

    private void OnDisable()
    {
        if (weaponManager != null)
        {
            weaponManager.OnGunEquipped -= OnGunEquipped;
            weaponManager.OnGunAmmoChanged -= OnGunAmmoChanged;
        }

        UnsubscribeFromGun();

        if (echapAction != null && echapAction.action != null)
        {
            echapAction.action.performed -= OnEchapPerformed;
            echapAction.action.Disable();
        }

        if (buttonContinue != null) buttonContinue.onClick.RemoveAllListeners();

        if (buttonQuit != null) buttonQuit.onClick.RemoveAllListeners();


        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
    }

    private void UpdateScoreUI(int money)
    {
        if (ScoreText != null)
            ScoreText.text = $"Score : {money}";
    }

    private void OnGunEquipped(Gun newGun)
    {
        UnsubscribeFromGun();

        if (newGun == null) return;

        currentSubscribedGun = newGun;
        currentSubscribedGun.OnAmmoChanged += OnAmmoChanged;
    }

    private void OnEchapPerformed(InputAction.CallbackContext ctx)
    {
        if (panelPause.activeSelf)
            ResumeGame();
        else
            PauseGame();
    }

    private void UnsubscribeFromGun()
    {
        if (currentSubscribedGun != null)
        {
            currentSubscribedGun.OnAmmoChanged -= OnAmmoChanged;
            currentSubscribedGun = null;
        }
    }

    private void OnGunAmmoChanged(int cur, int max)
    {
        OnAmmoChanged(cur, max);
    }

    private void OnAmmoChanged(int current, int max)
    {
        if (ammoText != null) ammoText.text = $"{current} / {max}";
    }

    public void PauseGame(bool showPausePanel = true)
    {
        if (panelPause != null) panelPause.SetActive(showPausePanel);

        Time.timeScale = 0f;
        AudioListener.pause = true;

        // On desactive le look :
        if (playerLook != null)
        {
            playerLook.SetEnabled(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // On desactive les mouvements, les interactions et le systeme d'arme
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerInteraction != null) playerInteraction.enabled = false;
        if (weaponManager != null && weaponManager.inputHandler != null) weaponManager.inputHandler.enabled = false;
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame called. StackTrace:\n" + Environment.StackTrace);
        if (panelPause != null) panelPause.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (playerLook != null)
        {
            playerLook.SetEnabled(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerInteraction != null) playerInteraction.enabled = true;
        if (weaponManager != null && weaponManager.inputHandler != null) weaponManager.inputHandler.enabled = true;
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}