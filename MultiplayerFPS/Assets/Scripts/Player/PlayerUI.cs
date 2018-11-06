using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    //health UI 
    [SerializeField]
    private Image healthFill;
    [SerializeField]
    private Image healthBackground;


    //ammo UI 
    [SerializeField]
    private Text ammoText;

    //ability UI
    [SerializeField]
    private Image[] abilityImages;
    [SerializeField]
    private Image[] abilityDarkMasks;
    [SerializeField]
    private Text[] abilityCooldownTexts;

    //Player stuff
    private Player player;
    private Health playerHealth;
    private WeaponManager playerWeaponManager;
    private AbilityManager playerAbilityManager;

    void Update() {
        if (playerWeaponManager != null)
        {
            UpdateAmmoCount(playerWeaponManager.GetCurrentAmmo(), playerWeaponManager.GetCurrentAmmoCount());
        }
    }

    void UpdateHealthBar()
    {
        float healthPercentage = (float)playerHealth.GetCurrentHealth() / (float)playerHealth.GetMaxHealth();
        healthFill.fillAmount = healthPercentage;
        healthFill.color = Color.Lerp(Color.red * 1.5f, Color.green, healthPercentage);
    }

    public void UpdateAmmoCount(int currentAmmo, int maxAmmo)
    {
        ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
        //gameObject.transform.SetParent(player.gameObject.transform);

        if ((playerHealth = _player.GetComponent<Health>()) != null)
        {
            playerHealth.healthChanged += UpdateHealthBar;
        }
        else
        {
            healthFill.enabled = false;
            healthBackground.enabled = false;
        }

        playerWeaponManager = _player.GetComponent<WeaponManager>();
        playerAbilityManager = _player.GetComponent<AbilityManager>();

        if (playerAbilityManager != null)
        {
            SetAbilities(playerAbilityManager.abilityCooldowns);
        }
    }

    private void SetAbilities(AbilityCooldown[] abilities)
    {
        if(abilities.Length >= abilityImages.Length)
        {
            for(int i = 0; i < abilityImages.Length; i++) {
                abilities[i].abilityImage = abilityImages[i];
                abilities[i].darkMask = abilityDarkMasks[i];
                abilities[i].cooldownText = abilityCooldownTexts[i];

                abilities[i].SetupUI();
            }
        }

        else if (abilities.Length < abilityImages.Length)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].abilityImage = abilityImages[i];
                abilities[i].darkMask = abilityDarkMasks[i];
                abilities[i].cooldownText = abilityCooldownTexts[i];

                abilities[i].SetupUI();
            }

            for(int j = abilities.Length; j < abilityImages.Length; j++)
            {
                abilityImages[j].enabled = false;
                abilityDarkMasks[j].enabled = false;
                abilityCooldownTexts[j].enabled = false;
            }
        }
    }
}
