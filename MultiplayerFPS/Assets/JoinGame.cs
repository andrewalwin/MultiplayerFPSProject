using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

    private NetworkManager networkManager;
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager != null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshGameRooms();

    }

    public void RefreshGameRooms()
    {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if(matches.Count == 0)
        {
            status.text = "No matches found";
            return;
        }

        ClearRoomList();
        foreach(MatchInfoSnapshot match in matches)
        {
            //dont need to set position due to scroll view doing it for us
            GameObject _roomListItemGameObject = Instantiate(roomListItemPrefab);
            _roomListItemGameObject.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGameObject.GetComponent<RoomListItem>();
            if(_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }
            //component sitting on gameobject that handles name/room size and a callback function for joining
            roomList.Add(_roomListItemGameObject);
        }

        if(roomList.Count == 0)
        {
            status.text = "No matches found";
        }
    }

    private void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        //clear our lists references
        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
