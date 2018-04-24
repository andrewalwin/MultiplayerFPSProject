using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon{

    [SerializeField]
    public string name = "Glock";

    [SerializeField]
    public int damage = 10;
    [SerializeField]
    public float range = 100f;
    //0 = single fire, anything above is automatic
    [SerializeField]
    public float fireRate = 0f; 
        

    public GameObject wpnGraphics;

}
