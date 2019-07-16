using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour {

    public AbilityCooldown[] abilityCooldowns;

    //TODO: 
        //player ability keys set in settings that this will give our abilities (example gives first abilitycooldown q, second w, third e...)

    private void Start()
    {
        abilityCooldowns = GetComponentsInChildren<AbilityCooldown>();

        PlayerUI pUI = GetComponentInChildren<PlayerUI>();
        if(pUI != null)
        {
            pUI.SetAbilities(abilityCooldowns);
        }
        //might want to do a check here in case the playerUI is not active right away (since by the time we call our Start() playersetup might not have finished creating the instance of our UI
    }
}
