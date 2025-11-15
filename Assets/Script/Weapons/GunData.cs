using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;

    public LayerMask targetLayerMask;

    [Header("Fire Config")] public float shootingRange;

    public float fireRate;
    public int shootDamage;

    [Header("Reload Config")] public int magazineSize;

    public float reloadTime;

    [Header("VFX")] public GameObject bulletTrailPrefab;

    public float bulletSpeed;

    [Header("SFX")] public AudioClip fireSound;

    public AudioClip reloadSound;
}