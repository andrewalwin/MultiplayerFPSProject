using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Weapon : NetworkBehaviour{

    //max ammo
    [SerializeField]
    private int maxAmmo;

    //ammo count before having to reload
    [SerializeField]
    public int clipSize;

    //how much total ammo is left
    public int ammoCount;
    //how much loaded ammo is left
    public int currentAmmo;

    //how fast we launch projectiles
    [SerializeField]
    private float fireSpeed;

    //how long between shots
    [SerializeField]
    public float fireDelay = .1f;

    //where to fire from
    [SerializeField]
    public Transform firePoint;

    //effect when we fire our gun
    [SerializeField]
    private ParticleSystem fireEffect;

    //what type of projectile we shoot
    [SerializeField]
    public GameObject projectilePrefab;

    //for keeping track of when we can fire
    public float nextShot = 0.0f;

    //whether or not we can shoot
    public bool canShoot;

    //whether or not we're in the middle of reloading
    public bool reloading;

    [SerializeField]
    private float weaponAccuracy;

    [SerializeField]
    private Camera weaponCamera;

    [SerializeField]
    public GameObject wpnGraphics;

    public void Start()
    {
        if (!hasAuthority)
        {
            return;
        }

        currentAmmo = clipSize;
        ammoCount = maxAmmo - clipSize;

        canShoot = true;
        reloading = false;
        nextShot = 0f;
    }

    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        //check if we can shoot
        if (Input.GetKey(KeyCode.Mouse0) && (Time.time > nextShot))
        {
            //Debug.Log(currentAmmo + "/" + ammoCount);
            nextShot = Time.time + fireDelay;
            //if we have enough ammo, then shoot
            if (currentAmmo > 0 && firePoint != null && canShoot)
            {
                //Debug.Log("SHOOTING");
                Shoot();
            }
            //if we're out of ammo, try to reload
            else if (currentAmmo <= 0)
            {
                canShoot = false;
                if (ammoCount > 0 && !reloading)
                {
                    //Debug.Log("RELOADING");
                    StartCoroutine("Reload", 0.5f);
                }
            }
        }
    }


    public void Shoot()
    {
        if (projectilePrefab != null)
        {
            float normalX = Util.GenerateBoxMullerPoint(0.5f, weaponAccuracy);
            float normalY = Util.GenerateBoxMullerPoint(0.5f, weaponAccuracy);
            Vector3 fireDirection;

            Ray rayCrosshair = weaponCamera.ViewportPointToRay(new Vector3(normalX, normalY, 0f));
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(rayCrosshair, out hit, 100, -1))
            {
                fireDirection = hit.point;
            }
            else
            {
                fireDirection = rayCrosshair.GetPoint(100);
            }

            fireDirection = fireDirection - firePoint.transform.position;


            GameObject projectileIns = ObjectPooler.instance.SpawnFromPool(projectilePrefab.name, firePoint.transform.position, firePoint.transform.rotation);
            Rigidbody projectileRb = projectileIns.GetComponent<Rigidbody>();
            projectileRb.velocity = fireDirection.normalized * fireSpeed;
            projectileIns.GetComponent<Projectile>().dealDamage = false;
            DoShootEffect();
            currentAmmo--;

            CmdShoot(projectileIns, projectileIns.transform.position, projectileIns.transform.rotation, projectileRb.velocity);
        }
    }

    [Command]
    private void CmdShoot(GameObject obj, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        RpcShoot(obj, position, rotation, velocity);
    }

    [ClientRpc]
    private void RpcShoot(GameObject obj, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        if(!hasAuthority)
        {
            GameObject projectileIns = ObjectPooler.instance.SpawnFromPool(projectilePrefab.name, position, rotation);
            projectileIns.GetComponent<Rigidbody>().velocity = velocity;
        }
    }

    private void DoShootEffect()
    {
        if(fireEffect != null && hasAuthority)
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

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
    
    public int GetAmmoCount()
    {
        return ammoCount;
    }

    public void SetWeaponCamera(Camera cam)
    {
        weaponCamera = cam;
    }
}