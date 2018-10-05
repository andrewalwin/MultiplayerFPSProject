using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{

    //List of components we want to disable
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Create player UI
            playerUIInstance = Instantiate(playerUIPrefab, gameObject.transform);
            playerUIInstance.name = playerUIPrefab.name;

            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
            {
                Debug.LogError("No Player UI component on PlayerUI Prefab");
            }
            else
            {
                ui.SetPlayer(GetComponent<Player>());
            }
            //make sure to call setup here or else its called for all players when a new player joins
            //GetComponent<Player>().BroadcastPlayer();
        }

    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);
    }

    public void DisableComponents()
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

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }
        GameManager.UnRegisterPlayer(transform.name);
    }

}
