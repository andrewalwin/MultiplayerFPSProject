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

    private WeaponManager playerWepManager;
    private Health playerHealth;


    private void Start()
    {
        playerWepManager = GetComponent<WeaponManager>();
        playerHealth = GetComponent<Health>();
        playerHealth.healthChanged += CheckHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if(playerHealth != null)
            {
                playerHealth.Damage(30);
            }
        }
    }

    public void CheckHealth()
    {
        Debug.Log("HEALTH CHECK: " + GetComponent<Health>().GetCurrentHealth());
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
        CmdDie();
        StartCoroutine(Respawn());
    }

    [Command]
    private void CmdDie()
    {
        RpcDie();
    }
    
    [ClientRpc]
    private void RpcDie()
    {
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        Collider _col = GetComponent<CapsuleCollider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.detectCollisions = false;
        }

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        //want to make sure all clients receive this spawn point information so that everything spawns in the right place
        //yield return new WaitForSeconds(0.1f);

        SetDefaults();
    }

    private void SetDefaults()
    {
        isDead = false;
        CmdSetDefaults();

    }

    [Command]
    private void CmdSetDefaults()
    {
        RpcSetDefaults();
    }

    [ClientRpc]
    private void RpcSetDefaults()
    {
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            //loop through components and enable them if they were enabled at start
            disableOnDeath[i].enabled = true;
        }

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            //enable our gameobjects
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<CapsuleCollider>();
        if (_col != null)
        {
            _col.enabled = true;
        }


        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.detectCollisions = true;
        }

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
            Debug.Log("RESPAWN FOR : " + gameObject.name);
            playerWepManager.Respawn();
        }

        if (!isLocalPlayer)
        {
            GetComponent<PlayerSetup>().DisableComponents();
        }

    }
}