using System.Collections;
using UnityEngine;

public class Pistol : Gun
{
    [Tooltip("Angle de dispersion en degrés")]
    public float spreadAngle;

    public override void Shoot()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("[Pistol]: cameraTransform is null. Falling back to Camera.main if present.");
            if (Camera.main != null) cameraTransform = Camera.main.transform;
            else return;
        }

        RaycastHit hit;
        Vector3 targetPoint;

        var forward = cameraTransform.forward;

        // Spread :
        if (spreadAngle > 0f)
        {
            var half = spreadAngle * 0.5f * Mathf.Deg2Rad;
            // random cone
            forward = Vector3.Slerp(forward,
                (Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle),
                    0f) * forward).normalized,
                1f);
        }

        var ray = new Ray(cameraTransform.position, forward);

        if (Physics.Raycast(ray, out hit, gunData.shootingRange, gunData.targetLayerMask,
                QueryTriggerInteraction.Ignore))
            //Debug.Log(gunData.gunName + " hit " + hit.collider.name);
            targetPoint = hit.point;
        else
            targetPoint = ray.origin + ray.direction * gunData.shootingRange;

        StartCoroutine(BulletFireCoroutine(targetPoint, hit));
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
            if (hit.collider.gameObject.isStatic) BulletHitFX(hit);
            // Si l'objet implémente IDamageable -> appeler TakeDamage
            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(gunData.shootDamage);
                ShowCrosshairHit();
            }
        }
    }

    private void BulletHitFX(RaycastHit hit)
    {
        var hitPosition = hit.point + hit.normal * 0.01f;

        if (bulletHolePrefab != null)
        {
            var bulletHole = Instantiate(bulletHolePrefab, hitPosition, Quaternion.LookRotation(hit.normal));
            // Parent to the hit object so the hole follows moving objects
            bulletHole.transform.SetParent(hit.collider.transform);
            Destroy(bulletHole, 10f);
        }
    }
}