using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("UI Refs")] public GameObject panel;

    public Button buyWeaponButton;
    public TMP_Text priceWeaponText;
    public Button buyDoubleJumpButton;
    public TMP_Text priceDoubleJumpText;
    public Button buyRegenButton;
    public TMP_Text priceRegenText;
    public Button closeButton;
    public TMP_Text moneyText;
    public TMP_Text feedbackText;
    public UiManager uiManager;

    private Shop openedShop;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);

        buyWeaponButton.onClick.AddListener(OnBuyWeapon);
        buyDoubleJumpButton.onClick.AddListener(OnBuyDoubleJump);
        buyRegenButton.onClick.AddListener(OnBuyRegen);
        closeButton.onClick.AddListener(Close);

        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnMoneyChanged += UpdateMoneyUI;

        UpdateMoneyUI(MoneyManager.Instance != null ? MoneyManager.Instance.Money : 0);
    }

    private void OnDestroy()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnMoneyChanged -= UpdateMoneyUI;
    }

    public void Open(Shop shop)
    {
        openedShop = shop;
        if (panel != null) panel.SetActive(true);

        priceWeaponText.text = $"{shop.priceWeapon.ToString()} $";
        priceDoubleJumpText.text = $"{shop.priceDoubleJump.ToString()} $";
        priceRegenText.text = $"{shop.priceRegen.ToString()} $";

        uiManager?.PauseGame(false);
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        openedShop = null;

        uiManager?.ResumeGame();
    }

    private void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = $"{money} $";
    }

    private void ShowFeedback(string s)
    {
        if (feedbackText != null)
        {
            StopAllCoroutines();
            feedbackText.text = s;
            StartCoroutine(ClearFeedbackCoroutine());
        }
    }

    private IEnumerator ClearFeedbackCoroutine()
    {
        yield return new WaitForSeconds(2f);
        feedbackText.text = "";
    }

    private void OnBuyWeapon()
    {
        if (openedShop == null) return;

        if (openedShop.TryBuyWeapon())
            ShowFeedback("Red pistol bought !");
        else
            ShowFeedback("Not enough money.");
        Close();
    }

    private void OnBuyDoubleJump()
    {
        if (openedShop == null) return;
        if (openedShop.TryBuyDoubleJump())
            ShowFeedback("double jump activated !");
        else
            ShowFeedback("Not enough money.");
        Close();
    }

    private void OnBuyRegen()
    {
        if (openedShop == null) return;
        if (openedShop.TryBuyRegen())
            ShowFeedback("Regeneration activated !");
        else
            ShowFeedback("Not enough money.");
        Close();
    }
}