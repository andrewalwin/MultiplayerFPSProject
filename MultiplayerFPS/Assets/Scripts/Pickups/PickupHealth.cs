using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : Pickup {

    [SerializeField]
    private float healAmount;

    override protected void ApplyEffect(GameObject obj)
    {
        Health playerHealth = obj.GetComponent<Health>();
        if(playerHealth != null){
            playerHealth.Recover(healAmount);
        }
    }
}
