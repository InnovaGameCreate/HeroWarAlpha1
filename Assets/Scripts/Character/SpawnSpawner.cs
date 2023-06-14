using Fusion;
using Online;
using Unit;
using UnityEngine;

/*
 * CharacterSpawnerにプレイヤー情報を与えるためにSpawnさせる為のクラスです。
 * 恐らく、α2以降は別のクラスでこの機能を実装することになると思います。
 * 
 * ノカミ
 */

public class SpawnSpawner : NetworkBehaviour
{
    [SerializeField] private Vector3[] spawnPoints;

    [SerializeField] private CharacterSpawner characterSpawnerPrefab;
    [SerializeField] private int unitCount = 5;

    void Start()
    {
        if (GameLauncher.Runner.GameMode == GameMode.Host)
        {
            foreach (var player in RoomPlayer.Players)
            {
                for (int i = 0; i < unitCount; i++)
                {
                    var index = RoomPlayer.Players.IndexOf(player);
                    var point = spawnPoints[index];
                    var characterSpawner = GameLauncher.Runner.Spawn(
                        characterSpawnerPrefab,
                        point + new Vector3(i * 30.0f, 0, 0),
                        Quaternion.identity,
                        player.Object.InputAuthority
                        
                    );
                    characterSpawner.RoomPlayer = player;
                }
            }
        }
    }
}