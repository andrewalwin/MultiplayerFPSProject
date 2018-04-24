﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerOff : MonoBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    //position to put our weapon
    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private PlayerWeapon secondaryWeapon;

    private PlayerWeapon currentWeapon;

    private WeaponGraphics currentGraphics;

    //storing our weapon gameobject
    private GameObject weaponIns;

    // Use this for initialization
    void Start () {

        EquipWeapon(primaryWeapon);
		
	}

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        //load weapon graphics
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.wpnGraphics, weaponHolder.position, weaponHolder.rotation);
        //need to parent our weapon to our weaponholder so it follows us
        _weaponIns.transform.SetParent(weaponHolder);

        //save our weapon instantiation
        weaponIns = _weaponIns;

        //look for the graphics component on our instance
 
            currentGraphics = _weaponIns.GetComponentInChildren<WeaponGraphics>();
            if (currentGraphics == null)
            {
                Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);
            }

        Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void SwitchWeapon()
    {
        if(GetCurrentWeapon() == primaryWeapon)
        {
            GameObject.Destroy(weaponIns);
            EquipWeapon(secondaryWeapon);
        }
        else if(GetCurrentWeapon() == secondaryWeapon)
        {
            GameObject.Destroy(weaponIns);
            EquipWeapon(primaryWeapon);
        }
    }
}
