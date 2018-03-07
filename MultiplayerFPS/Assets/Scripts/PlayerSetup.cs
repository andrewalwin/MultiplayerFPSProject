﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
//NetworkBehaviour allows our script to act as an object which is networked
public class PlayerSetup : NetworkBehaviour {

    //List of components we want to disable
    //all components are derived from Behaviour
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    //reference to our player graphics
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    Camera sceneCamera;

    void Start()
    {
        //if this object isnt being controlled by the system
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //only want to disable scene camera if we are the local player (not each time another player joins)
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false); //tagged our cameara as the main camera so we can easily disable it
            }

            //disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Create player UI
            //pos, scale, etc will be default
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }

        GetComponent<Player>().Setup();
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        //change layer of object and its child objects (unity doesn't have good way to disable children)
        obj.layer = newLayer;

        //go through each child of the player graphic object and disable it down the hierarchy
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    //method already in network behaviour class, called when a client is setup locally
    public override void OnStartClient()
    {
        base.OnStartClient();

        //will have network id always since class derives from NetworkBehaviour, and we require player
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    //called when object is destroyed or we disconnect
    void OnDisable()
    {
        Destroy(playerUIInstance);

       if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }

}
