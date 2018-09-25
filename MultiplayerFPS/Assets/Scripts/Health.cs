using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;

    public delegate void HealthChangedDelegate();
    public HealthChangedDelegate healthChanged;

    void Awake() {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        CmdDamage(damageAmount);
        if (healthChanged != null)
        {
            healthChanged();
        }
    }

    public void Damage(float damageAmount)
    {
        int roundedDamage = (int)Mathf.Ceil(damageAmount);
        Damage(roundedDamage);
    }
    
    [Command]
    public void CmdDamage(int damageAmount)
    {
        RpcDamage(damageAmount);
    }

    [ClientRpc]
    private void RpcDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
    }

    public void Recover(int recoverAmount)
    {
        CmdRecover(recoverAmount);
    }

    public void Recover(float recoverAmount)
    {
        int roundedRecover = Mathf.RoundToInt(recoverAmount);
        CmdRecover(roundedRecover);
    }
    
    [Command]
    private void CmdRecover(int recoverAmount)
    {
        RpcRecover(recoverAmount);
    }

    [ClientRpc]
    private void RpcRecover(int recoverAmount)
    {
        currentHealth = Mathf.Max(maxHealth, currentHealth + recoverAmount);
        healthChanged();
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
