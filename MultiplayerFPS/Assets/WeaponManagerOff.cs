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
    private Weapon primaryWeapon;

    [SerializeField]
    private Weapon secondaryWeapon;

    private Weapon currentWeapon;

    private WeaponGraphics currentGraphics;

    //storing our weapon gameobject
    private GameObject weaponIns;

    private ObjectPooler objectPooler;

    // Use this for initialization
    void Start () {

        EquipWeapon(primaryWeapon);
        objectPooler = FindObjectOfType<ObjectPooler>();
        Debug.Log(primaryWeapon.projectilePrefab.name);
        ObjectPoolItem primaryProjItem = new ObjectPoolItem(primaryWeapon.clipSize, primaryWeapon.projectilePrefab, true, primaryWeapon.projectilePrefab.name);
        ObjectPoolItem secondaryProjItem = new ObjectPoolItem(secondaryWeapon.clipSize, secondaryWeapon.projectilePrefab, true, secondaryWeapon.projectilePrefab.name);
        objectPooler.itemsToPool.Add(primaryProjItem);
        objectPooler.itemsToPool.Add(secondaryProjItem);
        objectPooler.PopulatePools();

    }

    void EquipWeapon(Weapon _weapon)
    {
        currentWeapon = _weapon;

        //load weapon graphics
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.gameObject, weaponHolder.position, weaponHolder.rotation);
        //need to parent our weapon to our weaponholder so it follows us
        _weaponIns.transform.SetParent(weaponHolder);

        //save our weapon instantiation
        weaponIns = _weaponIns;

        //look for the graphics component on our instance
 
            currentGraphics = _weaponIns.GetComponentInChildren<WeaponGraphics>();
            if (currentGraphics == null)
            {
                Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);
            }

        Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
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
            weaponIns.transform.LookAt(rayCrosshair.GetPoint(100));
        //}
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void SwitchWeapon()
    {
        if(GetCurrentWeapon() == primaryWeapon)
        {
            GameObject.Destroy(weaponIns);
            EquipWeapon(secondaryWeapon);
        }
        else if(GetCurrentWeapon() == secondaryWeapon)
        {
            GameObject.Destroy(weaponIns);
            EquipWeapon(primaryWeapon);
        }
    }
}
