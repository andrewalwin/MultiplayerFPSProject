using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    [SerializeField]
    private Image healthFill;

    private PlayerMove player;

    private Health playerHealth;

	void Start () {	
	}

    void UpdateHealthBar()
    {
        float healthPercentage = (float)playerHealth.GetCurrentHealth() / (float)playerHealth.GetMaxHealth();
        healthFill.fillAmount = healthPercentage;
        healthFill.color = Color.Lerp(Color.red * 1.5f, Color.green, healthPercentage);
    }

    void UpdateAmmoCount()
    {

    }

    public void SetPlayer(PlayerMove _player)
    {
        player = _player;
        if((playerHealth = _player.GetComponent<Health>()) != null)
        {
            playerHealth.healthChanged += UpdateHealthBar;
        }
    }
}
