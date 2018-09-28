using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    //position to put our weapon
    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Camera weaponCamera;

    //the order of the types of weapons in these arrays should remain the same. Ex: both arrays in order contain a: rifle, shotgun, glock...
    [SerializeField]
    private GameObject[] weaponPrefabList;
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
            Debug.Log(gameObject.name + " is not local player");
            return;
        }

        //weapon prefabs instantiate as disabled, must enable with EquipWeapon();
        if(weaponPrefabList != null)
        {
            for(int i = 0; i < weaponPrefabList.Length; i++)
            {
                Weapon prefabWeaponScript = weaponPrefabList[i].GetComponent<Weapon>();
                objectPooler.AddPool(prefabWeaponScript.projectilePrefab, prefabWeaponScript.projectilePrefab.name, prefabWeaponScript.clipSize);
            }
            CmdSpawnAllWeapons();
        }
        //currentWeapon = prefabWeapon.GetComponent<Weapon>();
        //prefabWeapon.gameObject.transform.SetParent(weaponHolder);
        //Util.SetLayerRecursively(prefabWeapon, LayerMask.NameToLayer(weaponLayerName));
        //objectPooler.AddPool(currentWeapon.projectilePrefab, currentWeapon.projectilePrefab.name, currentWeapon.clipSize);        
        //weaponInsList[currentWeaponIndex].SetActive(true);
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

    void EquipWeapon(int weaponInsIndex)
    {
        if(weaponInsList[weaponInsIndex] == null)
        {
            SpawnWeapon(weaponInsIndex);
            EquipWeapon(weaponInsIndex);
        }
        else
        {
            currentWeapon = weaponInsList[weaponInsIndex].GetComponent<Weapon>();
            CmdEquipWeapon(weaponInsIndex);
        }
    }

    [Command]
    void CmdEquipWeapon(int weaponInsIndex)
    {
        weaponInsList[weaponInsIndex].SetActive(true);
        RpcEquipWeapon(weaponInsList[weaponInsIndex]);
    }

    [ClientRpc]
    void RpcEquipWeapon(GameObject equippedWeapon)
    {
        equippedWeapon.SetActive(true);
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

    void SpawnWeapon(int weaponPrefabIndex)
    {
        //NEED to add the parenting to the weaponHolder, as well as code to spawn the weaponHolder. Atm it spawns middle of the screen and wont rotate
        CmdSpawnWeapon(weaponPrefabIndex);
    }

    [Command]
    void CmdSpawnWeapon(int weaponPrefabIndex)
    {
                if (weaponPrefabList[weaponPrefabIndex] != null)
                {
                    GameObject wepIns = (GameObject)Instantiate(weaponPrefabList[weaponPrefabIndex], weaponHolder.position, weaponHolder.rotation, weaponHolder);
                    weaponInsList[weaponPrefabIndex] = wepIns;
                    wepIns.transform.parent = gameObject.transform;
                    currentWeaponIns = wepIns;
                    currentWeapon = wepIns.GetComponent<Weapon>();
                    currentWeapon.SetWeaponCamera(weaponCamera);
                    NetworkServer.SpawnWithClientAuthority(wepIns, gameObject);
                    RpcSpawnWeapon(wepIns, wepIns.transform.localPosition, wepIns.transform.localRotation, wepIns.transform.parent.gameObject);
                }
    }

    [ClientRpc]
    void RpcSpawnWeapon(GameObject obj, Vector3 localPos, Quaternion localRot, GameObject parent)
    {
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = localRot;
        obj.GetComponent<Weapon>().SetWeaponCamera(weaponCamera);

        if (!isLocalPlayer)
        {
            Util.SetLayerRecursively(obj, 0);
        }
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
                        wepIns.transform.parent = gameObject.transform;
                        currentWeaponIns = wepIns;
                        currentWeapon = wepIns.GetComponent<Weapon>();
                        currentWeapon.SetWeaponCamera(weaponCamera);
                        NetworkServer.SpawnWithClientAuthority(wepIns, gameObject);
                        RpcSpawnWeapon(wepIns, wepIns.transform.localPosition, wepIns.transform.localRotation, wepIns.transform.parent.gameObject);
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


    [Command]
    void CmdAssignWeaponAuthority(GameObject obj)
    {
        if (gameObject.GetComponent<NetworkIdentity>() != null)
        {
            if (connectionToClient.isReady)
            {
                for (int i = 0; i < weaponPrefabList.Length; i++)
                {
                    GameObject wepIns = (GameObject)Instantiate(weaponPrefabList[i], weaponHolder.position, weaponHolder.rotation, weaponHolder);
                    weaponInsList[i] = wepIns;
                    wepIns.transform.parent = gameObject.transform;
                    currentWeaponIns = wepIns;
                    currentWeapon = wepIns.GetComponent<Weapon>();
                    currentWeapon.SetWeaponCamera(weaponCamera);
                    NetworkServer.SpawnWithClientAuthority(wepIns, gameObject);
                    RpcSyncWeaponOnce(wepIns, wepIns.transform.localPosition, wepIns.transform.localRotation, wepIns.transform.parent.gameObject);
                }
            }
            else
            {
                StartCoroutine(WaitForReadyAuthority(obj));
            }
            //obj.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    [ClientRpc]
    void RpcSyncWeaponOnce(GameObject obj, Vector3 localPos, Quaternion localRot, GameObject parent)
    {
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = localRot;
        obj.GetComponent<Weapon>().SetWeaponCamera(weaponCamera);

        if (!isLocalPlayer)
        {
            Util.SetLayerRecursively(obj, 0);
        }
    }

    void EquipWeapon3(GameObject obj)
    {
        CmdEquipWeapon3(obj);
    }

    [Command]
    void CmdEquipWeapon3(GameObject obj)
    {
        if (connectionToClient.isReady)
        {
            GameObject weaponIns = (GameObject)Instantiate(prefabWeapon, weaponHolder.position, weaponHolder.rotation);
            weaponIns.transform.SetParent(weaponHolder);
            NetworkServer.SpawnWithClientAuthority(weaponIns, connectionToClient);
            currentWeapon = prefabWeapon.GetComponent<Weapon>();
            Util.SetLayerRecursively(prefabWeapon, LayerMask.NameToLayer(weaponLayerName));
            RpcEquipWeapon3(prefabWeapon, weaponHolder.gameObject);
        }
        else
        {
            StartCoroutine(WaitForReadyEquip(obj));
        }
    }

    [ClientRpc]
    void RpcEquipWeapon3(GameObject obj, GameObject parent)
    {
        if (!NetworkServer.active)
        {
            GameObject weaponIns = (GameObject)Instantiate(obj, weaponHolder.position, weaponHolder.rotation);
            weaponIns.transform.SetParent(parent.transform);
        }
    }

    IEnumerator WaitForReadyEquip(GameObject obj)
    {
        while (!connectionToClient.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        EquipWeapon3(obj);
    }

    IEnumerator WaitForReadyAuthority(GameObject obj)
    {
        while (!connectionToClient.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        CmdAssignWeaponAuthority(obj);
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
