using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour{

    //max ammo
    [SerializeField]
    private int maxAmmo;

    //ammo count before having to reload
    [SerializeField]
    public int clipSize;

    //how much total ammo is left
    private int ammoCount;
    //how much loaded ammo is left
    private int currentAmmo;

    //how fast we launch projectiles
    [SerializeField]
    private float fireSpeed;

    //how long between shots
    [SerializeField]
    private float fireDelay = .1f;

    //where to fire from
    [SerializeField]
    private Transform firePoint;

    //effect when we fire our gun
    [SerializeField]
    private ParticleSystem fireEffect;

    //what type of projectile we shoot
    [SerializeField]
    public GameObject projectilePrefab;

    //for keeping track of when we can fire
    private float nextShot = 0.0f;

    [SerializeField]
    public GameObject wpnGraphics;


    void Start()
    {
        currentAmmo = clipSize;
        ammoCount = maxAmmo - clipSize;
    }

    void Update()
    {
        //check if we can shoot
        if (Input.GetKey(KeyCode.Mouse0) && (Time.time > nextShot))
        {
            nextShot = Time.time + fireDelay;
            //if we have enough ammo, then shoot
            if (currentAmmo > 0 && firePoint != null)
            {
                Shoot();
            }
            //if we're out of ammo, try to reload
            else if (currentAmmo <= 0)
            {
                if (ammoCount > 0)
                {
                    Invoke("Reload", 0.5f);
                }
            }
        }
    }

    public void Shoot()
    {
        if (projectilePrefab != null)
        {
            //spawn our bullet at our firePoint, and addForce to it in its forward direction (since we align its forward with the front of our gun)
            //could also call a function/set a flag IN our bullet that just has it move
            //GameObject projectileIns = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            GameObject projectileIns = FindObjectOfType<ObjectPooler>().GetPooledObject(projectilePrefab.name);
            projectileIns.transform.position = firePoint.transform.position;
            projectileIns.transform.rotation = firePoint.transform.rotation;
            projectileIns.SetActive(true);
            projectileIns.GetComponent<Rigidbody>().AddForce(firePoint.transform.forward * fireSpeed);
            currentAmmo--;
        }
    }

    public void Reload()
    {

        currentAmmo = Mathf.Min(clipSize, ammoCount);
        ammoCount -= currentAmmo;
    }
}