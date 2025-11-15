using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("Config")] public int priceWeapon = 200;

    public GameObject weaponPrefab;
    public int priceDoubleJump = 150;
    public int priceRegen = 100;

    [Header("Refs")] public ShopUI shopUI;

    public WeaponManager weaponManager;
    public PlayerMovement playerMovement;
    public PlayerInfoManager playerInfoManager;

    public void OpenShop()
    {
        if (shopUI != null)
            shopUI.Open(this);
        else
            Debug.LogWarning("ShopUI not assigned on Shop.");
    }

    public bool TryBuyWeapon()
    {
        if (weaponPrefab == null) return false;
        if (MoneyManager.Instance == null) return false;

        if (MoneyManager.Instance.TrySpend(priceWeapon))
        {
            if (weaponManager == null)
            {
                Debug.LogWarning("WeaponManager not assigned on Shop.");
                return true;
            }

            var inst = Instantiate(weaponPrefab);
            inst.SetActive(false);

            var gun = inst.GetComponent<Gun>();
            if (gun == null)
            {
                Debug.LogWarning("Purchased prefab has no Gun component.");
                Destroy(inst);
                return true;
            }

            weaponManager.RegisterNewWeapon(gun);

            return true;
        }

        return false;
    }

    public bool TryBuyDoubleJump()
    {
        if (MoneyManager.Instance.TrySpend(priceDoubleJump))
            if (playerMovement != null)
            {
                playerMovement.SetAllowDoubleJump(true);
                return true;
            }

        return false;
    }

    public bool TryBuyRegen()
    {
        if (MoneyManager.Instance.TrySpend(priceRegen))
            if (playerInfoManager != null)
            {
                playerInfoManager.RestoreMaxHealth();
                return true;
            }

        return false;
    }
}