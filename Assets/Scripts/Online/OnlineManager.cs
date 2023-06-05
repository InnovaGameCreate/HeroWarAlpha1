using System;
using Fusion;
using UnityEngine;

namespace Online
{
    public class OnlineManager : NetworkBehaviour
    {
        private Camera _mainCamera;
        [SerializeField] private Vector3 camera1;
        [SerializeField] private Vector3 camera2;
        public GameObject playerPrefab; 
        public GameObject[] spawnPlace;

        public static OnlineManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            SetCamera();
            //SpawnPlayer();
        }

        public override void Spawned()
        {
            base.Spawned();
        }

        private void SpawnPlayer()
        {
            for (int i = 0; i < RoomPlayer.Players.Count; i++)
            {
                var obj = GameLauncher.Runner.Spawn(playerPrefab, spawnPlace[i].transform.position, Quaternion.identity, RoomPlayer.Players[i].Object.InputAuthority);
                var profile = obj.GetComponent<Unit.CharacterProfile>();
                Unit.CharacterDataList data = Resources.Load<Unit.CharacterDataList>("CharacterDataList");
                var instantiateCharacterData = data.characterDataList[0];
                profile.Init(instantiateCharacterData);
                //profile.SetCharacterOwnerType(i <= 0 ? OwnerType.Player : OwnerType.Enemy);
            }
        }

        private void SetCamera()
        {
            switch (GameLauncher.Runner.GameMode)
            {
                case GameMode.Host:
                    _mainCamera.transform.position = camera1;
                    _mainCamera.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
                    return;
                case GameMode.Client:
                    _mainCamera.transform.position = camera2;
                    _mainCamera.transform.rotation = Quaternion.Euler(60f, 180f, 0f);
                    return;
                case GameMode.Single:
                case GameMode.Shared:
                case GameMode.Server:
                case GameMode.AutoHostOrClient:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
