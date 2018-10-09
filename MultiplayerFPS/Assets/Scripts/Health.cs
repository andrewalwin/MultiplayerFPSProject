using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    [SyncVar]
    private int currentHealth;

    public delegate void HealthChangedDelegate();
    public HealthChangedDelegate healthChanged;

    void Awake() {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        Recover(maxHealth);
    }

    public void Damage(int damageAmount)
    {
        currentHealth = Mathf.Clamp((currentHealth - damageAmount), 0, maxHealth);
        if (healthChanged != null)
        {
            healthChanged();
        }
        CmdDamage(damageAmount);
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
        if(!isLocalPlayer)
        currentHealth = Mathf.Clamp((currentHealth - damageAmount), 0, maxHealth);
        if (healthChanged != null)
        {
            healthChanged();
        }
    }

    public void Recover(int recoverAmount)
    {
        CmdRecover(recoverAmount);
    }

    public void Recover(float recoverAmount)
    {
        int roundedRecover = Mathf.RoundToInt(recoverAmount);
        Recover(roundedRecover);
    }
    
    [Command]
    private void CmdRecover(int recoverAmount)
    {
        RpcRecover(recoverAmount);
    }

    [ClientRpc]
    private void RpcRecover(int recoverAmount)
    {
        currentHealth = Mathf.Clamp((currentHealth + recoverAmount), 0, maxHealth);
        if (healthChanged != null)
        {
            healthChanged();
        }
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
