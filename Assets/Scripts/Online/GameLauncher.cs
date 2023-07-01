using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected
}

namespace Online
{
    public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;
        public static GameLauncher Instance { get; private set; }
        public static NetworkRunner Runner;
        [SerializeField] private RoomPlayer _roomPlayer;

        //private LevelManager _levelManager;
        private NetworkSceneManagerDefault _networkSceneManagerDefault;
        
        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _networkSceneManagerDefault = GetComponent<NetworkSceneManagerDefault>();
        }

        public void StartHostOrClient() => StartGame(GameMode.AutoHostOrClient);


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;
            runner.Spawn(_roomPlayer, Vector3.zero, Quaternion.identity, player);
            if (RoomPlayer.Players.Count >= 2) _networkSceneManagerDefault.Runner.SetActiveScene(1);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            RoomPlayer.RemovePlayer(runner, player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        private void SetConnectionStatus(ConnectionStatus status)
        {
            Debug.Log($"Setting connection status to {status}");

            ConnectionStatus = status;

            if (!Application.isPlaying)
                return;

            if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
            {
                SceneManager.LoadScene("MatchingTest");
            }
        }

        public void LeaveSession()
        {
            if (Runner != null)
                Runner.Shutdown();
            else
                SetConnectionStatus(ConnectionStatus.Disconnected);
        }
        public void OnDisconnectedFromServer(NetworkRunner runner) 
        {
            Debug.Log("Disconnected from Server");
            LeaveSession();
            SetConnectionStatus(ConnectionStatus.Disconnected);
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }

        private async void StartGame(GameMode mode)
        {
            if (Runner != null) return;
            
            Runner = gameObject.AddComponent<NetworkRunner>();
            Runner.ProvideInput = true;

            await Runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "tester22",
                SceneManager = _networkSceneManagerDefault,
                PlayerCount = 2,
            });
        }
    }
}
