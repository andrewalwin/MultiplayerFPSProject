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

    //list of our weapons
    [SerializeField]
    private Weapon[] weaponList;

    //storing our weapon gameobject
    private GameObject weaponIns;

    //references to the gameobjects we instantiate from our weaponList
    private GameObject[] weaponInsList;

    private Weapon currentWeapon;
    private int currentWeaponIndex;

    private WeaponGraphics currentGraphics;

    private ObjectPooler objectPooler;

    // Use this for initialization
    void Start () {
        objectPooler = ObjectPooler.instance;

        weaponInsList = new GameObject[weaponList.Length];

        //start by equipping our primary weapon
        for(int i = weaponList.Length -1; i >= 0; i--)
        {
            //add the object pools for our weapons projectiles
            objectPooler.AddPool(weaponList[i].projectilePrefab, weaponList[i].projectilePrefab.name, weaponList[i].clipSize);
            //spawn the non primary weapons then disable them
            EquipWeapon(weaponList[i]);

            weaponInsList[i] = weaponIns;

            weaponIns.gameObject.SetActive(false);
        }

        currentWeaponIndex = 0;
        weaponInsList[currentWeaponIndex].SetActive(true);

        //add the object pools for our weapons projectiles
        //objectPooler.AddPool(primaryWeapon.projectilePrefab, primaryWeapon.projectilePrefab.name, primaryWeapon.clipSize);
        //objectPooler.AddPool(secondaryWeapon.projectilePrefab, secondaryWeapon.projectilePrefab.name, secondaryWeapon.clipSize);
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
        weaponIns.SetActive(false);
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponList.Length;
        weaponIns = weaponInsList[currentWeaponIndex];
        weaponInsList[currentWeaponIndex].SetActive(true);
    }
}
