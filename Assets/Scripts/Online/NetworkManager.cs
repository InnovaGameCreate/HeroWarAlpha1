using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    static public OwnerType yourType;
    public GameObject playerPrefab; 
    public GameObject enemyPrefab;
    public GameObject[] SpawnPlace;
    int num = 0;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
        if (runner.GameMode == GameMode.Host)
        {
            yourType = OwnerType.Player;
            if (num == 0)
            {
                num++;
                var obj = runner.Spawn(playerPrefab, SpawnPlace[0].transform.position, Quaternion.identity, player);
                var profile = obj.GetComponent<Unit.CharacterProfile>();
                Unit.CharacterDataList Data = Resources.Load<Unit.CharacterDataList>("CharacterDataList");
                var InstantiateCharacterData = Data.characterDataList[1];
                profile.Init(InstantiateCharacterData);
            }
            else
            {
                var obj = runner.Spawn(enemyPrefab, SpawnPlace[1].transform.position, Quaternion.identity, player);
                var profile = obj.GetComponent<Unit.CharacterProfile>();
                Unit.CharacterDataList Data = Resources.Load<Unit.CharacterDataList>("CharacterDataList");
                var InstantiateCharacterData = Data.characterDataList[1];
                profile.Init(InstantiateCharacterData);
            }
        }
        else if (runner.GameMode == GameMode.Client)
        {
            yourType = OwnerType.Enemy;
        }

        Debug.Log("Your type is " + yourType);

        
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }


    private NetworkRunner _runner;
    async void StartGame()
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }
}
