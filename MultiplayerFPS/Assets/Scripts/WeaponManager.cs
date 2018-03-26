using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    //position to put our weapon
    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;

	void Start () {
        //when the player spawns they equip their primary weapon
        EquipWeapon(primaryWeapon);

	}

    void EquipWeapon (PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        //load weapon graphics
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.wpnGraphics, weaponHolder.position, weaponHolder.rotation);
        //need to parent our weapon to our weaponholder so it follows us
        _weaponIns.transform.SetParent(weaponHolder);

        if (isLocalPlayer)
        {
            //setting the weapon and all children of the parent to this layer in case our weapon is made of multiple objects
            _weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
            foreach (Transform child in _weaponIns.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(weaponLayerName);
            }

        }
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
	
}
