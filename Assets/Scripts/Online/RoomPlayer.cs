using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Online;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomPlayer : NetworkBehaviour
{
    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static RoomPlayer Local;
    private PlayerRef _ref;

    public NetworkBool hasWined;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log(GameLauncher.Runner.GameMode);
        }
        
        Players.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public static void RemovePlayer(NetworkRunner runner, PlayerRef player)
    {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == player);
        if (roomPlayer != null)
        {
            Players.Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
    }
}
