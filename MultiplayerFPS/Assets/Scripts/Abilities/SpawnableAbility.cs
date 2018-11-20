using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SpawnableAbility")]
public class SpawnableAbility : Ability
{

    [SerializeField]
    private GameObject spawnablePrefab;
    [SerializeField]
    private GameObject spawnablePreviewPrefab;

    [SerializeField]
    private LayerMask ignoreCollisionsMask;

    [SerializeField]
    private float spawnRange;
    [SerializeField]
    private float maxSpawnAngle;

    public RaycastDisplaySpawnable spawnableDisplay;

    public override void Initialize(GameObject obj)
    {
        if (spawnableDisplay != null)
        {
            //spawnableDisplay.spawnableObject = spawnablePrefab;
            //can also set our "fake" object if we go that method
            //spawnableDisplay.spawnRange = spawnRange;
            //spawnableDisplay.spawnAngleMax = maxSpawnAngle;
            //spawnableDisplay.ignoreCollisions = ignoreCollisionsMask;
        }
    }

    public override void ActivateAbility()
    {
        if (spawnableDisplay != null)
        {
            //spawnableDisplay.ToggleDisplaySpawnable();
            //make ToggleDisplaySpawnable() do what we do in the current Update(), which is:
            //change the isDisplaying, do the check if isDisplaying is true, and if it is, then call the display coroutine.If isDisplaying is false, then stop the coroutine, and do whatever resetting of variables we need

        }
    }
}
