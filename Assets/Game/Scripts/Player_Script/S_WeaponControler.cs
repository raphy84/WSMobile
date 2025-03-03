using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponData weaponData;
    public Transform firePoint;
    public int remainingBullets;

    private void Start()
    {
        remainingBullets = weaponData.bulletCount;
    }

    public void Fire(Vector2 targetPosition)
    {
        if (remainingBullets <= 0)
        {
            Destroy(gameObject);
            return;
        }

        remainingBullets--;

        for (int i = 0; i < weaponData.bulletCount; i++)
        {
            float angle = Random.Range(-weaponData.dispersion, weaponData.dispersion);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(weaponData.bulletPrefab, firePoint.position, firePoint.rotation * rotation);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = (targetPosition - (Vector2)firePoint.position).normalized * weaponData.bulletSpeed;
        }
    }
}

