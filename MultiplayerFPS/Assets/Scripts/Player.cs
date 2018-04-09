using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
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

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    //going to have a component and a boolean saying whether or not it was enabled on start, and we need to re-enable it
    [SerializeField]
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    public void SetupPlayer() {

        if (isLocalPlayer)
        {
            //switch cameras, only want to do this on the local player, not all clients
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadcastNewPlayer();
    }

    [Command]
    private void CmdBroadcastNewPlayer() {

        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients() {
        //only want to do this list on the players first setup
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                //store whether or not a component was disabled at the start
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
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

        //DISABLE Game Objects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //disable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        //spawn death effect
        GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
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

        //want to make sure all clients receive this spawn point information so that everything spawns in the right place
        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log("Player " + transform.name + " respawned!");
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

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            //enable our gameobjects
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true;
        }

        //create spawn effect
        GameObject _gfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 2f);
    }
}
