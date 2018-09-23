using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(Health))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private bool[] wasEnabled;

    private bool firstSetup = true;

    public void SetupPlayer()
    {

        if (isLocalPlayer)
        {
            //switch cameras, only want to do this on the local player, not all clients
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadcastNewPlayer();
    }

    [Command]
    private void CmdBroadcastNewPlayer()
    {

        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
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

    [Command]
    private void CmdTakeDamage()
    {
        RpcTakeDamage();
    }

    [ClientRpc]
    public void RpcTakeDamage()
    {
        if (isDead)
        {
            return;
        }

        if (GetComponent<Health>().GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " IS DEAD");

        //CALL RESPAWN
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
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
        if (_col != null)
        {
            _col.enabled = true;
        }

    }
}