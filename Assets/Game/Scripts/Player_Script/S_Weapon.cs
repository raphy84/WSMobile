using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject bulletPrefab;
    public int bulletCount;
    public float fireRate;
    public float bulletSpeed;
    public float dispersion;
}
