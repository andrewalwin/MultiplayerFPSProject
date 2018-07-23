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

    //whether or not we can shoot
    private bool canShoot;

    //whether or not we're in the middle of reloading
    private bool reloading;

    [SerializeField]
    public GameObject wpnGraphics;


    void Start()
    {
        currentAmmo = clipSize;
        ammoCount = maxAmmo - clipSize;

        canShoot = true;
        reloading = false;
    }

    void Update()
    {
        //check if we can shoot
        if (Input.GetKey(KeyCode.Mouse0) && (Time.time > nextShot))
        {
            Debug.Log(currentAmmo + "/" + ammoCount);
            nextShot = Time.time + fireDelay;
            //if we have enough ammo, then shoot
            if (currentAmmo > 0 && firePoint != null && canShoot)
            {
                Shoot();
            }
            //if we're out of ammo, try to reload
            else if (currentAmmo <= 0)
            {
                canShoot = false;
                if (ammoCount > 0 && !reloading)
                {
                    Debug.Log("RELOADING");
                    StartCoroutine("Reload", 0.5f);
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
            GameObject projectileIns = ObjectPooler.instance.SpawnFromPool(projectilePrefab.name, firePoint.transform.position, firePoint.transform.rotation);
            //projectileIns.GetComponent<Rigidbody>().AddForce(firePoint.transform.forward * fireSpeed);
            projectileIns.GetComponent<Rigidbody>().velocity = firePoint.transform.forward * fireSpeed;
            DoShootEffect();
            currentAmmo--;
        }
    }

    private void DoShootEffect()
    {
        if(fireEffect != null)
        {
            ParticleSystem pS = fireEffect;
            if (!pS.isPlaying) pS.Play();

            else if (pS.isPlaying) pS.Stop();

            else if (pS.isPlaying) pS.Stop();

            if (!pS.isPlaying) pS.Play();
        }
    }

    IEnumerator Reload(float reloadTime)
    { 
        //before coroutine is done set reloading to true so we can't stack reloads/restart the coroutine
        reloading = true;

        yield return new WaitForSeconds(reloadTime);
        //set our ammo counts to the correct post-reload amounts
        currentAmmo = Mathf.Min(clipSize, ammoCount);
        ammoCount = Mathf.Clamp(ammoCount - clipSize, 0, maxAmmo);
        //player can now shoot and is not reloading
        canShoot = true;
        reloading = false;
    }

    //when we swap weapons we need to cancel the reload to avoid unwanted behavior
    public void stopReload()
    {
        StopCoroutine("Reload");
        reloading = false;
    }
}