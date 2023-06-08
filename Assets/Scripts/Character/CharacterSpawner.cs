using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using Fusion;
using Online;

namespace Unit
{
    public class CharacterSpawner : NetworkBehaviour
    {
        [SerializeField]
        private GameObject SpawnPrefab;
        CharacterProfile _profile;
        [SerializeField]
        private bool isAutoSpawn = false;
        [SerializeField]
        CharacterData isAutoSpawnCharacterData;
        [SerializeField]
        private GameObject SpawnEffect;

        [SerializeField] private int playerNum;
        public RoomPlayer RoomPlayer { get; set; }
        
        private CharacterData InstantiateCharacterData;
        /*
        void Start()
        {
            if (RoomPlayer.Local != null)
            {
                _playerRef = RoomPlayer.Local.Object.InputAuthority;
            }
        }
        */

        public override void Spawned()
        {
            base.Spawned();
            Init(isAutoSpawnCharacterData);
        }

        public void Init(CharacterData data)
        {
            InstantiateCharacterData = data;
            ReSpawn();
        }

        /// <summary>
        /// キャラクターの生成
        /// </summary>
        private async void ReSpawn()
        {
            await Task.Delay(3300);
            var Effect = Instantiate(SpawnEffect, transform.position, Quaternion.identity);
            await Task.Delay(1700);
            if (GameLauncher.Runner.GameMode == GameMode.Host)
            {
                var obj = Runner.Spawn(SpawnPrefab, transform.position, Quaternion.identity, RoomPlayer.Object.InputAuthority);
                _profile = obj.GetComponent<CharacterProfile>();
                _profile.Init(InstantiateCharacterData);
                _profile.SetCharacterOwnerType(OwnerType.Player);
                _profile.local = RoomPlayer.Object.InputAuthority;
                Debug.Log(RoomPlayer.Object.InputAuthority);

                _profile
                    .OnCharacterStateChanged
                    .Where(CharacterState => CharacterState == CharacterState.Dead)
                    .Subscribe(CharacterState =>
                    {
                        Runner.Despawn(obj);
                        Debug.Log("生成" + playerNum);
                        ReSpawnTimer();
                    }
                ).AddTo(this);
            }
        }
        /// <summary>
        /// リスポーン用のタイマー
        /// </summary>
        private async void ReSpawnTimer()
        {
            await Task.Delay(5000);
            ReSpawn();
        }
    }
}
