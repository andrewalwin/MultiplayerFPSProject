using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;

    public delegate void HealthChangedDelegate();
    public HealthChangedDelegate healthChanged;

	void Awake () {
        currentHealth = maxHealth;
	}
	
    public void Damage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        healthChanged();
    }

    public void Damage(float damageAmount)
    {
        int roundedDamage = (int)Mathf.Ceil(damageAmount);
        Damage(roundedDamage);
    }

    public void Recover(int recoverAmount)
    {
        currentHealth = Mathf.Max(maxHealth, currentHealth + recoverAmount);
        healthChanged();
    }

    public void Recover(float recoverAmount)
    {
        int roundedRecover = Mathf.RoundToInt(recoverAmount);
        Recover(roundedRecover);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
