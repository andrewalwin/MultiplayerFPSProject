using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerOff : MonoBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    //position to put our weapon
    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject[] weaponPrefabList;

    private GameObject weaponIns;
    private GameObject[] weaponInsList;
    private int currentWeaponIndex;

    private Weapon currentWeapon;

    private WeaponGraphics currentWeaponGraphics;

    private ObjectPooler objectPooler;

    // Use this for initialization
    void Start () {
        objectPooler = ObjectPooler.instance;
        weaponInsList = new GameObject[weaponPrefabList.Length];

        for(int i = weaponPrefabList.Length -1; i >= 0; i--)
        {
            GameObject prefabWeapon = weaponPrefabList[i];
            Weapon prefabWeaponScript = prefabWeapon.GetComponent<Weapon>();
            if (prefabWeapon != null)
            {
                objectPooler.AddPool(prefabWeaponScript.projectilePrefab, prefabWeaponScript.projectilePrefab.name, prefabWeaponScript.clipSize);

                EquipWeapon(prefabWeapon);
                weaponInsList[i] = weaponIns;
                weaponIns.gameObject.SetActive(false);
            }

        }

        currentWeaponIndex = 0;
        weaponInsList[currentWeaponIndex].SetActive(true);
    }

    void EquipWeapon(GameObject _weapon)
    {
        GameObject _weaponIns = (GameObject)Instantiate(_weapon, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        weaponIns = _weaponIns;
        currentWeapon = weaponIns.GetComponent<Weapon>();
 
        //currentWeaponGraphics = _weaponIns.GetComponentInChildren<WeaponGraphics>();
        //if (currentWeaponGraphics == null)
        //{
        //    Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);
        //}

        Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));

        weaponIns.SetActive(true);
    }

    private void Update()
    {
        //rotate weapon to face crosshair
        Ray rayCrosshair = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(rayCrosshair, out hit, 100, -1))
        {
        //    weaponIns.transform.LookAt(hit.point);
        }
        //else
        //{      
        if (weaponIns != null)
        {
            weaponIns.transform.LookAt(rayCrosshair.GetPoint(100));
        }
        //}
    }


    public void SwitchWeapon()
    {
        //set our current weapon to inactive
        weaponIns.SetActive(false);
        //attempt to stop our weapons reload in case we swap in the middle of reloading
        weaponIns.GetComponent<Weapon>().stopReload();
        //find the next weapon to equip and set it to active
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabList.Length;
        weaponIns = weaponInsList[currentWeaponIndex];
        weaponInsList[currentWeaponIndex].SetActive(true);
        currentWeapon = weaponIns.GetComponent<Weapon>();
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
        return currentWeapon.GetCurrentAmmo();
    }
    
    public int GetCurrentAmmoCount()
    {
        return currentWeapon.GetAmmoCount();
    }
}
