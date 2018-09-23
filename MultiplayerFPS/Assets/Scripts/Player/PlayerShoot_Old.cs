//using UnityEngine;
//using UnityEngine.Networking;

//[RequireComponent (typeof (WeaponManager))]
//public class PlayerShoot_Old : NetworkBehaviour {

//    //weapon reference, for swapping weapons and models
//    private PlayerWeapon currentWeapon;
//    //reference to the weaponGFX layer


//    private const string PLAYER_TAG = "Player";

//    //reference to player camera to raycast
//    [SerializeField]
//    private Camera cam;

//    //layerMask for our raycast to hit, DONT want to hit triggers, ourself, and other things used purely for gameplay state, just things we can see
//    [SerializeField]
//    private LayerMask mask;

//    private WeaponManager weaponManager;
//    private Player player;

//    void Start()
//    {
//        if(cam == null)
//        {
//            Debug.LogError("PlayerShoot: No Camera Attached");
//            this.enabled = false;
//        }

//        weaponManager = GetComponent<WeaponManager>();
//        player = GetComponent<Player>();
//    }

//    void Update()
//    {
//        currentWeapon = weaponManager.GetCurrentWeapon();

//        if (currentWeapon.fireRate <= 0)
//        {
//            if (Input.GetButtonDown("Fire1"))
//            {
//                Shoot();
//            }
//        }
//        else
//        {
//            if (Input.GetButtonDown("Fire1"))
//            {
//                //j
//                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
//            }
//            else if (Input.GetButtonUp("Fire1"))
//            {
//                //when we let go of fire1
//                CancelInvoke("Shoot");
//            }
//        }
//    }
    
//    //called on server when player shoots
//    [Command]
//    void CmdOnShoot()
//    {
//        RpcDoShootEffect();
//    }

//    //called on all clients when we need to show shoot effect
//    [ClientRpc]
//    void RpcDoShootEffect()
//    {
//        //play our particle system
//        ParticleSystem pS = weaponManager.GetCurrentGraphics().muzzleFlash;
//        if (!pS.isPlaying) pS.Play();

//        else if (pS.isPlaying) pS.Stop();

//        else if (pS.isPlaying) pS.Stop();

//        else if (!pS.isPlaying) pS.Play();
//    }

//    //Called on server when something is hit, takes in hit point and surface normal
//    [Command]
//    void CmdOnHit(Vector3 _pos, Vector3 _normal)
//    {
//        RpcDoHitEffect(_pos, _normal);
//    }
    
//    //called on all clients when something is hit, spawns in hit effects
//    [ClientRpc]
//    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
//    {
//        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
//        Destroy(_hitEffect, 2f);
//    }

//    [Client]
//    private void Shoot()
//    {
//        if (!isLocalPlayer || player.isDead || player == null)
//        {
//            return;
//        }
//        //are shooting, call OnShoot method on server
//        CmdOnShoot();

//        RaycastHit _hit;
//        //define the ray: start, direction, out hit, distance, layerMask (lots of other overloads as well)
//        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
//        {
//            //we hit something
//            if(_hit.collider.tag == PLAYER_TAG)//could also check layer and stuff
//            {
//                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
//            }

//            //hit something, call hit effect (change to be different for player/surface)
//            CmdOnHit(_hit.point, _hit.normal);
//        }
//    }

//    [Command]
//    void CmdPlayerShot(string _playerID, int _damage)
//    {
//        Debug.Log(_playerID + " has been shot!");
//        //Destroy(GameObject.Find(_ID));

//        Player _player = GameManager.GetPlayer(_playerID);
//        _player.RpcTakeDamage(_damage);
//    }
//}
