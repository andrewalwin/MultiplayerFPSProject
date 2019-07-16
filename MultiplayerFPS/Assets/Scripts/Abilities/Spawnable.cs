using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour {

    [SerializeField]
    private GameObject spawnEffect;

	public void OnSpawn()
    {

    }

    public void OnSpawn(Vector3 floorPosition)
    {
        Instantiate(spawnEffect, floorPosition, gameObject.transform.rotation);
        OnSpawn();
    }
}
