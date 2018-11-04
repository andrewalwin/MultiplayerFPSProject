using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour {

    [SerializeField]
    private AbilityCooldown[] abilityCooldowns;

    [SerializeField]
    private GameObject[] abilityUIObj;

    private Image[] abilityImages;
    private Text[] abilityCooldownText;


    // Use this for initialization
    void Start()
    {
        abilityImages = new Image[abilityUIObj.Length];
        abilityCooldownText = new Text[abilityUIObj.Length];
        
        for(int i = 0; i < abilityUIObj.Length; i++)
        {
            abilityImages[i] = abilityUIObj[i].GetComponentInChildren<Image>();
            abilityCooldownText[i] = abilityUIObj[i].GetComponentInChildren<Text>();
        }

        for(int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (abilityUIObj.Length >= (1 + 1))
            {
                abilityCooldowns[i].abilityImage = abilityImages[i];
                abilityCooldowns[i].darkMask = abilityImages[i];
                abilityCooldowns[i].cooldownText = abilityCooldownText[i];
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
