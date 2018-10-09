using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupPlatform : NetworkBehaviour {

    [SerializeField]
    GameObject pickupPrefab;

    [SerializeField]
    Transform spawnPoint;

    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnPickup();
    }

    void SpawnPickup()
    {
        if (pickupPrefab != null)
        {
            CmdSpawnPickup();
        }
    }

    [Command]
    void CmdSpawnPickup()
    {
        GameObject pickupIns = Instantiate(pickupPrefab, spawnPoint.position, Quaternion.identity);
        //pickupIns.transform.SetParent(spawnPoint);
        NetworkServer.Spawn(pickupIns);
        //RpcSpawnPickup(pickupIns);
    }

    [ClientRpc]
    void RpcSpawnPickup()
    {
        GameObject pickupIns = Instantiate(pickupPrefab, spawnPoint.position, Quaternion.identity);
        //pickupIns.transform.SetParent(spawnPoint);
        NetworkServer.Spawn(pickupIns);
    }

    void Update () {
		
	}
}
