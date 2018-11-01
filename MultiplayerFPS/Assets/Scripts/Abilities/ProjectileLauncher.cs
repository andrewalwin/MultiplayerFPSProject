using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour {

    //attach to the object you want to fire from (ex: a gun)
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public float fireSpeed;
    [SerializeField] public Transform firePoint;

    public void Launch()
    {
        Debug.Log("TEST ABILITY LAUNCH");
    }
}
