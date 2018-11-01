using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/ProjectileAbility")]
public class ProjectileAbility : Ability
{

    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float fireSpeed;
    public ProjectileLauncher launcher;

    public override void Initialize(GameObject obj)
    {
        launcher = obj.GetComponentInChildren<ProjectileLauncher>();
        if (launcher != null)
        {
            launcher.projectilePrefab = projectilePrefab;
            launcher.fireSpeed = fireSpeed;
        }
    }
    public override void ActivateAbility()
    {
        Debug.Log("LAUN?: " + (launcher != null));
        if (launcher != null)
        {
            launcher.Launch();
        }
    }
}
