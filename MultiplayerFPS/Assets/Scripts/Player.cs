using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    //every time this value changes its pushed out to all the clients, very useful feature
    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    //going to have a component and a boolean saying whether or not it was enabled on start, and we need to re-enable it
    [SerializeField]
    private bool[] wasEnabled;

	public void Setup () {

        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            //store whether or not a component was disabled at the start
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
	}

 //   void Update() {
 //       if (!isLocalPlayer)
 //       {
 //           return;
 //       }
 //       if (Input.GetKeyDown(KeyCode.K))
 //       {
 //           RpcTakeDamage(9000);
 //       }
	//}

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
        {
            return;
        }
        //dont need to do anything else with health, since its synced out
        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        //DISABLE COMPONENTS
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        Debug.Log(transform.name + " IS DAED");

        //CALL RESPAWN
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        //call our match settings from our instance of the game manager
        yield return new WaitForSeconds(3f);

        //networkmanager.singleton is the instance of the network manager in our scene
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition(); //returns one of the spawn points in our network manager
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log("Player " + transform.name + " respawned!");

        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            //loop through components and enable them if they were enabled at start
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true;
        }
    }
}
