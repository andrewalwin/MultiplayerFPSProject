using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    //weapon reference, for swapping weapons and models
    [SerializeField]
    private PlayerWeapon weapon;
    //reference to the weaponGFX layer
    [SerializeField]
    private GameObject weaponGFX;
    [SerializeField]
    private string weaponLayerName = "Weapon";

    private const string PLAYER_TAG = "Player";

    //reference to player camera to raycast
    [SerializeField]
    private Camera cam;

    //layerMask for our raycast to hit, DONT want to hit triggers, ourself, and other things used purely for gameplay state, just things we can see
    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera Attached");
            this.enabled = false;
        }
        weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
    }

    void Update()
    {
        //single fire
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    
    [Client]
    private void Shoot()
    {
        RaycastHit _hit;
        //define the ray: start, direction, out hit, distance, layerMask (lots of other overloads as well)
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask))
        {
            //we hit something
            if(_hit.collider.tag == PLAYER_TAG)//could also check layer and stuff
            {
                CmdPlayerShot(_hit.collider.name, weapon.damage);
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
