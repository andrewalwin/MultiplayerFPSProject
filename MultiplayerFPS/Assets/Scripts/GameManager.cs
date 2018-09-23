using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public MatchSettings matchSettings;

    //marking as gameobject to disable all audio listeners and other things
    [SerializeField]
    private GameObject sceneCamera;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void SetSceneCameraActive(bool isActive)
    {
        if(sceneCamera == null)
        {
            return;
        }
        sceneCamera.SetActive(isActive);
    }
    

    #region Player registering

    private const string PLAYER_ID_PREFIX = "Player ";

    //static player dictionary
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);

        //rename player transform to player ID for easy fetching
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    /*
    private void OnGUI()
    {
        //visualize our dictionary

        //area for our gui to reside in
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        //loop through dictionary
        foreach(string _playerID in players.Keys)
        {
            GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
        }

        GUILayout.EndArea();
        GUILayout.EndVertical();
    }
    */
#endregion
}
