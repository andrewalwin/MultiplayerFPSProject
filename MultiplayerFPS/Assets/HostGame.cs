using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour {

    [SerializeField]
    private uint roomSize = 6;
    //named via input field on GUI
    private string roomName;
    private string roomPwd;

    private NetworkManager networkManager;

    private void Start()
    {
        //call currently active network manager
        networkManager = NetworkManager.singleton;

        //check if our matchmaker is null, if so then activate it
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
        Debug.Log("ROOM NAME CHANGED: " + _name);
    }

    public void CreateRoom()
    {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " with max players: " + roomSize);
            //create room
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
        else
        {
            Debug.Log("NAME EMPTY");
        }
    }
}
