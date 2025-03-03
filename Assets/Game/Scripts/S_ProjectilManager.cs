using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class S_ProjectilManager : MonoBehaviour
{
    public GameObject BulletPrefab;

    public int bullet_anount = 100;
    public TextMeshProUGUI bullet_value_textholder;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bullet_value_textholder.text = bullet_anount.ToString();
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(BulletPrefab, this.transform.position, transform.rotation, transform);
        bullet_anount--;
    }
}
