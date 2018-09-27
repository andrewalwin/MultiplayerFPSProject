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
            //prefabWeapon.SetActive(false);
            return;
        }
        //for (int i = weaponPrefabList.Length - 1; i >= 0; i--)
        //{
        //    GameObject prefabWeapon = weaponPrefabList[i];
        //    Weapon prefabWeaponScript = prefabWeapon.GetComponent<Weapon>();
        //    if (prefabWeapon != null)
        //    {
        //        objectPooler.AddPool(prefabWeaponScript.projectilePrefab, prefabWeaponScript.projectilePrefab.name, prefabWeaponScript.clipSize);

        //        EquipWeapon(prefabWeapon);
        //        weaponInsList[i] = weaponIns;
        //        weaponIns.gameObject.SetActive(false);
        //    }

        //}
        //GameObject prefabWeapon = weaponPrefabList[0];
        //Weapon prefabWeaponScript = prefabWeapon.GetComponent<Weapon>();
        // if (prefabWeapon != null)
        //{
        //    objectPooler.AddPool(prefabWeaponScript.projectilePrefab, prefabWeaponScript.projectilePrefab.name, prefabWeaponScript.clipSize);
        //   EquipWeapon(prefabWeapon);
        //}

        if(prefabWeapon != null)
        {
            //GameObject weaponIns = (GameObject)Instantiate(prefabWeapon, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            //NetworkServer.SpawnWithClientAuthority(weaponIns, connectionToClient);
            //currentWeapon = prefabWeapon.GetComponent<Weapon>();
            //Util.SetLayerRecursively(prefabWeapon, LayerMask.NameToLayer(weaponLayerName));
            Weapon prefabWeaponScript = prefabWeapon.GetComponent<Weapon>();
            objectPooler.AddPool(prefabWeaponScript.projectilePrefab, prefabWeaponScript.projectilePrefab.name, prefabWeaponScript.clipSize);
            CmdAssignWeaponAuthority(prefabWeapon);
            //CmdAssignWeaponAuthority(prefabWeapon);

            //EquipWeapon(prefabWeapon);
        }
        //currentWeapon = prefabWeapon.GetComponent<Weapon>();
        //prefabWeapon.gameObject.transform.SetParent(weaponHolder);
        //Util.SetLayerRecursively(prefabWeapon, LayerMask.NameToLayer(weaponLayerName));
        //objectPooler.AddPool(currentWeapon.projectilePrefab, currentWeapon.projectilePrefab.name, currentWeapon.clipSize);        
        //weaponInsList[currentWeaponIndex].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (isLocalPlayer)
            {
                CmdSwap(currentWeaponIndex);
            }
        }
    }

    [Command]
    private void CmdSwap(int wepIndex)
    {
        weaponInsList[wepIndex].SetActive(false);
        RpcSwap(weaponInsList[wepIndex]);
    }

    [ClientRpc]
    private void RpcSwap(GameObject swappedWeapon)
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

    void EquipWeapon(GameObject obj)
    {
        CmdEquipWeapon(obj);
    }

    [Command]
    void CmdEquipWeapon(GameObject obj)
    {
        if (connectionToClient.isReady)
        {
            GameObject weaponIns = (GameObject)Instantiate(prefabWeapon, weaponHolder.position, weaponHolder.rotation);
            weaponIns.transform.SetParent(weaponHolder);
            NetworkServer.SpawnWithClientAuthority(weaponIns, connectionToClient);
            currentWeapon = prefabWeapon.GetComponent<Weapon>();
            Util.SetLayerRecursively(prefabWeapon, LayerMask.NameToLayer(weaponLayerName));
            RpcEquipWeapon(prefabWeapon, weaponHolder.gameObject);
        }
        else
        {
            StartCoroutine(WaitForReadyEquip(obj));
        }
    }

    [ClientRpc]
    void RpcEquipWeapon(GameObject obj, GameObject parent)
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
        EquipWeapon(obj);
    }

    IEnumerator WaitForReadyAuthority(GameObject obj)
    {
        while (!connectionToClient.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        CmdAssignWeaponAuthority(obj);
    }

    void EquipWeapon2(GameObject _weapon)
    {
        //GameObject _weaponIns = (GameObject)Instantiate(_weapon, weaponHolder.position, weaponHolder.rotation);
       // _weaponIns.transform.SetParent(weaponHolder);

        //weaponIns = _weaponIns;
        //currentWeapon = weaponIns.GetComponent<Weapon>();

        //currentWeaponGraphics = _weaponIns.GetComponentInChildren<WeaponGraphics>();
        //if (currentWeaponGraphics == null)
        //{
        //    Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);
        //}

        //Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));

        //weaponIns.SetActive(true);
        CmdEquipWeapon2(_weapon);
    }

    [Command]
    void CmdEquipWeapon2(GameObject _weapon)
    {
        GameObject _weaponIns = new GameObject();
        _weaponIns.SetActive(false);
        _weaponIns = (GameObject)Instantiate(_weapon, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        NetworkServer.SpawnWithClientAuthority(_weaponIns, connectionToClient);

        currentWeaponIns = _weaponIns;
        currentWeapon = currentWeaponIns.GetComponent<Weapon>();

        Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));

        currentWeaponIns.SetActive(true);
       RpcEquipWeapon2(_weaponIns, _weaponIns.transform.parent.gameObject, _weaponIns.transform.localPosition, _weaponIns.transform.localRotation);
    }

    [ClientRpc]
    void RpcEquipWeapon2(GameObject _weapon, GameObject parent, Vector3 localPosition, Quaternion localRotation)
    {
        if (!isLocalPlayer)
        {
            GameObject _weaponIns = new GameObject();
            _weaponIns.SetActive(false);
            _weaponIns = (GameObject)Instantiate(_weapon, weaponHolder.position, weaponHolder.rotation);
            _weaponIns.transform.SetParent(weaponHolder);
            NetworkServer.Spawn(_weaponIns);
            _weaponIns.SetActive(true);
        }
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
