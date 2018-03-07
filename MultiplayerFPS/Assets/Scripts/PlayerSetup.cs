using UnityEngine;
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
        }

        GetComponent<Player>().Setup();
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
       if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }

}
