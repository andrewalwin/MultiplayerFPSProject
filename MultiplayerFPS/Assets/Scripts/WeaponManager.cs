using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Camera weaponCamera;

    //the order of the types of weapons in these arrays should remain the same. Ex: both arrays in order contain a: rifle, shotgun, glock...
    [SerializeField]
    private GameObject[] weaponPrefabList;
    [SerializeField]
    private GameObject[] weaponInsList;
    private int currentWeaponIndex = 0;

    private GameObject currentWeaponIns;

    private Weapon currentWeapon;

    private WeaponGraphics currentWeaponGraphics;

    private ObjectPooler objectPooler;

    [SerializeField]
    private GameObject prefabWeapon;

    // Use this for initialization
    void Start() {
        objectPooler = ObjectPooler.instance;
        weaponInsList = new GameObject[weaponPrefabList.Length];

        if (!isLocalPlayer)
        {
            return;
        }

        //weapon prefabs instantiate as disabled, must enable with EquipWeapon();
        Debug.Log(gameObject.name + " list null? : " + (weaponPrefabList != null));
        if(weaponPrefabList != null)
        {
            for(int i = 0; i < weaponPrefabList.Length; i++)
            {
                Weapon prefabWeaponScript = weaponPrefabList[i].GetComponent<Weapon>();
                objectPooler.AddPool(prefabWeaponScript.projectilePrefab, prefabWeaponScript.projectilePrefab.name, prefabWeaponScript.clipSize);
            }
            SpawnAllWeapons();
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            // CmdSwap(currentWeaponIndex);

            SwapWeapon();
        }
    }

    private void OnDisable()
    {
        for(int i = 0; i < weaponInsList.Length; i++)
        {
            UnequipWeapon(i);
        }
    }

    public void Respawn()
    {
        EquipWeapon(0);
        for(int i = 0; i < weaponInsList.Length; i++)
        {
            weaponInsList[i].GetComponent<Weapon>().Start();
        }
    }

    void EquipWeapon(int weaponInsIndex)
    {
        if(weaponInsList[weaponInsIndex] == null)
        {
            SpawnWeapon(weaponInsIndex, true);
            //EquipWeapon(weaponInsIndex);
        }
        else
        {
            //currentWeapon = weaponInsList[weaponInsIndex].GetComponent<Weapon>();
            CmdEquipWeapon(weaponInsIndex);
        }
    }

    [Command]
    void CmdEquipWeapon(int weaponInsIndex)
    {
        weaponInsList[weaponInsIndex].SetActive(true);
        RpcEquipWeapon(weaponInsList[weaponInsIndex], weaponInsIndex);
    }

    [ClientRpc]
    void RpcEquipWeapon(GameObject equippedWeapon, int weaponInsIndex)
    {
        equippedWeapon.SetActive(true);
        currentWeapon = weaponInsList[weaponInsIndex].GetComponent<Weapon>();
    }

    void UnequipWeapon(int weaponInsIndex)
    {
        if(weaponInsList[weaponInsIndex] != null)
        {
            CmdUnequipWeapon(weaponInsIndex);
        }
    }

    [Command]
    void CmdUnequipWeapon(int weaponInsIndex)
    {
        weaponInsList[weaponInsIndex].SetActive(false);
        RpcUnequipWeapon(weaponInsList[weaponInsIndex]);
    }

    [ClientRpc]
    void RpcUnequipWeapon(GameObject equippedWeapon)
    {
        equippedWeapon.SetActive(false);
    }

    void SpawnWeapon(int weaponPrefabIndex, bool doEquipWeapon)
    {
        CmdSpawnWeapon(weaponPrefabIndex, doEquipWeapon);
    }

    [Command]
    void CmdSpawnWeapon(int weaponPrefabIndex, bool doEquipWeapon)
    {
                if (weaponPrefabList[weaponPrefabIndex] != null)
                {
                    GameObject wepIns = (GameObject)Instantiate(weaponPrefabList[weaponPrefabIndex], weaponHolder.position, weaponHolder.rotation, weaponHolder);
                    weaponInsList[weaponPrefabIndex] = wepIns;
                    wepIns.transform.parent = weaponHolder;
                    currentWeaponIns = wepIns;
                    currentWeapon = wepIns.GetComponent<Weapon>();
                    currentWeapon.SetWeaponCamera(weaponCamera);
                    NetworkServer.SpawnWithClientAuthority(wepIns, gameObject);
                    RpcSpawnWeapon(wepIns, gameObject.GetComponent<NetworkIdentity>().netId, weaponPrefabIndex);
                }
        if (doEquipWeapon)
        {
            EquipWeapon(weaponPrefabIndex);
        }
    }

    [ClientRpc]
    void RpcSpawnWeapon(GameObject obj, NetworkInstanceId parentId, int weaponIndex)
    {
        //obj.transform.parent = ClientScene.FindLocalObject(parentId).transform;
        obj.transform.parent = GameObject.Find("/" + ClientScene.FindLocalObject(parentId).name + "/Camera" + "/WeaponHolder").transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.GetComponent<Weapon>().SetWeaponCamera(weaponCamera);

        weaponInsList[weaponIndex] = obj;

        if (!isLocalPlayer)
        {
            Util.SetLayerRecursively(obj, 0);
        }
    }

    void SpawnAllWeapons()
    {
        CmdSpawnAllWeapons();
    }

    [Command]
    void CmdSpawnAllWeapons()
    {
        if (gameObject.GetComponent<NetworkIdentity>() != null)
        {
            if (connectionToClient.isReady)
            {
                for (int i = 0; i < weaponPrefabList.Length; i++)
                {
                    if (weaponPrefabList[i] != null)
                    {
                        GameObject wepIns = (GameObject)Instantiate(weaponPrefabList[i], weaponHolder.position, weaponHolder.rotation, weaponHolder);
                        weaponInsList[i] = wepIns;
                        wepIns.transform.parent = weaponHolder;
                        currentWeaponIns = wepIns;
                        currentWeapon = wepIns.GetComponent<Weapon>();
                        currentWeapon.SetWeaponCamera(weaponCamera);
                        NetworkServer.SpawnWithClientAuthority(wepIns, gameObject);
                        RpcSpawnWeapon(wepIns, gameObject.GetComponent<NetworkIdentity>().netId, i);
                        UnequipWeapon(i);
                    }
                }
                EquipWeapon(0);
            }
            else
            {
                StartCoroutine(WaitForReadySpawn());
            }
        }
    }

    IEnumerator WaitForReadySpawn()
    {
        while (!connectionToClient.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        CmdSpawnAllWeapons();
    }

    void SwapWeapon()
    {
        CmdSwapWeapon();
    }

    [Command]
    private void CmdSwapWeapon()
    {
        UnequipWeapon(currentWeaponIndex);
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponInsList.Length;
        EquipWeapon(currentWeaponIndex);
    }

    [ClientRpc]
    private void RpcSwapWeapon(GameObject swappedWeapon)
    {
        swappedWeapon.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //rotate weapon to face crosshair
        Ray rayCrosshair = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(rayCrosshair, out hit, 100, -1))
        {
        //    weaponIns.transform.LookAt(hit.point);
        }
        //else
        //{      
        if (currentWeaponIns != null)
        {
            currentWeaponIns.transform.LookAt(rayCrosshair.GetPoint(100));
        }
        //}
    }


    public void SwitchWeapon()
    {
        //set our current weapon to inactive
        currentWeaponIns.SetActive(false);
        //attempt to stop our weapons reload in case we swap in the middle of reloading
        currentWeaponIns.GetComponent<Weapon>().stopReload();
        //find the next weapon to equip and set it to active
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabList.Length;
        currentWeaponIns = weaponInsList[currentWeaponIndex];
        weaponInsList[currentWeaponIndex].SetActive(true);
        currentWeapon = currentWeaponIns.GetComponent<Weapon>();
        CmdSwitchWeapon(currentWeaponIndex);
    }

    [Command]
    void CmdSwitchWeapon(int weaponIndex)
    {
        RpcSwitchWeapon(weaponIndex);
    }

    [ClientRpc]
    void RpcSwitchWeapon(int weaponIndex)
    {
        if (!isLocalPlayer)
        {
            currentWeaponIns.SetActive(false);
            weaponInsList[currentWeaponIndex].SetActive(true);
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentWeaponGraphics;
    }

    public int GetCurrentAmmo()
    {
        if (currentWeapon != null)
        {
            return currentWeapon.GetCurrentAmmo();
        }
        return 0;
    }
    
    public int GetCurrentAmmoCount()
    {
        if (currentWeapon != null)
        {
            return currentWeapon.GetAmmoCount();
        }
        return 0;
    }

    public GameObject GetCurrentWeaponIns()
    {
        return currentWeaponIns;
    }
}
