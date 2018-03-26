using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof (WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    //weapon reference, for swapping weapons and models
    private PlayerWeapon currentWeapon;
    //reference to the weaponGFX layer


    private const string PLAYER_TAG = "Player";

    //reference to player camera to raycast
    [SerializeField]
    private Camera cam;

    //layerMask for our raycast to hit, DONT want to hit triggers, ourself, and other things used purely for gameplay state, just things we can see
    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;

    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera Attached");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //j
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                //when we let go of fire1
                CancelInvoke("Shoot");
            }
        }
    }
    
    [Client]
    private void Shoot()
    {
        Debug.Log("SHOOTING");
        RaycastHit _hit;
        //define the ray: start, direction, out hit, distance, layerMask (lots of other overloads as well)
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            //we hit something
            if(_hit.collider.tag == PLAYER_TAG)//could also check layer and stuff
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " has been shot!");
        //Destroy(GameObject.Find(_ID));

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
