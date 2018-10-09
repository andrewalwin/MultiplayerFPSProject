using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : Pickup
{
    [SerializeField]
    private int ammoAmount;

    override protected void ApplyEffect(GameObject obj)
    {
        WeaponManager playerWepManager = obj.GetComponent<WeaponManager>();
        if (playerWepManager != null)
        {
            playerWepManager.RefillCurrentWeapon(ammoAmount);
        }
    }
}
