﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    [SerializeField]
    private Image healthFill;
    [SerializeField]
    private Image healthBackground;

    [SerializeField]
    private Text ammoText;

    private PlayerMove player;

    private Health playerHealth;

    private WeaponManagerOff playerWeaponManager;

	void Update () {	
        if(playerWeaponManager != null)
        {
            UpdateAmmoCount(playerWeaponManager.GetCurrentAmmo(), playerWeaponManager.GetCurrentMaxAmmo());
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
        Debug.Log(ammoText.text);
    }

    public void SetPlayer(PlayerMove _player)
    {
        player = _player;
        gameObject.transform.SetParent(player.gameObject.transform);

        if((playerHealth = _player.GetComponent<Health>()) != null)
        {
            playerHealth.healthChanged += UpdateHealthBar;
        }
        else
        {
            healthFill.enabled = false;
            healthBackground.enabled = false;
        }

        playerWeaponManager = _player.GetComponent<WeaponManagerOff>();
    }
}
