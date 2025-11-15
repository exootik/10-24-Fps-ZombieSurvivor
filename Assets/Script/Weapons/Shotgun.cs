using System.Collections;
using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun config")] [Tooltip("Nombre de projectiles (pellets) par tir")]
    public int pelletCount = 8;

    [Tooltip("Angle de dispersion en degrés")]
    public float spreadAngle = 10f;

    public override void Shoot()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("[Shotgun]: cameraTransform is null. Falling back to Camera.main if present.");
            if (Camera.main != null) cameraTransform = Camera.main.transform;
            else return;
        }

        for (var i = 0; i < Mathf.Max(1, pelletCount); i++)
        {
            var dir = GetSpreadDirection(cameraTransform.forward, spreadAngle);
            var ray = new Ray(cameraTransform.position, dir);

            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit, gunData.shootingRange, gunData.targetLayerMask,
                    QueryTriggerInteraction.Ignore))
                //Debug.Log(gunData.gunName + " hit " + hit.collider.name);
                targetPoint = hit.point;
            else
                targetPoint = ray.origin + ray.direction * gunData.shootingRange;

            StartCoroutine(BulletFireCoroutine(targetPoint, hit));
        }
    }

    private IEnumerator BulletFireCoroutine(Vector3 target, RaycastHit hit)
    {
        GameObject bulletTrail = null;
        if (gunData.bulletTrailPrefab != null && gunMuzzle != null)
            bulletTrail = Instantiate(gunData.bulletTrailPrefab, gunMuzzle.position, Quaternion.identity);

        var speed = Mathf.Max(1f, gunData.bulletSpeed);
        while (bulletTrail != null && Vector3.Distance(bulletTrail.transform.position, target) > 0.1f)
        {
            bulletTrail.transform.position =
                Vector3.MoveTowards(bulletTrail.transform.position, target, Time.deltaTime * speed);
            yield return null;
        }

        if (bulletTrail != null) Destroy(bulletTrail);

        // Quand on touche quelque chose :
        if (hit.collider != null)
        {
            // Tous les objets static : 
            if (hit.collider.gameObject.isStatic)
            {
                BulletHitFX(hit);
            }
            else
            {
                //Si l'objet implémente IDamageable -> appeler TakeDamage
                var damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(gunData.shootDamage);
                    ShowCrosshairHit();
                }
            }
        }
    }

    private void BulletHitFX(RaycastHit hit)
    {
        var hitPosition = hit.point + hit.normal * 0.01f;

        if (bulletHolePrefab != null)
        {
            var bulletHole = Instantiate(bulletHolePrefab, hitPosition, Quaternion.LookRotation(hit.normal));
            bulletHole.transform.SetParent(hit.collider.transform);
            Destroy(bulletHole, 10f);
        }
    }

    private Vector3 GetSpreadDirection(Vector3 forward, float angleDeg)
    {
        var half = angleDeg * 0.5f;
        var yaw = Random.Range(-half, half);
        var pitch = Random.Range(-half, half);

        var dir = Quaternion.AngleAxis(yaw, cameraTransform.up) * forward;
        dir = Quaternion.AngleAxis(pitch, cameraTransform.right) * dir;
        return dir.normalized;
    }
}