using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Fusion;
using Online;
using UnityEngine.SceneManagement;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    private static NetworkSceneManagerDefault _networkSceneManagerDefault;
    [SerializeField] UIManager _uiManager;
    Dictionary<PlayerRef, ScoreObject> _scoreObjects = new Dictionary<PlayerRef, ScoreObject>();
    PlayerRef p1, p2;

    private void Awake()
    {
        _networkSceneManagerDefault = FindObjectOfType<NetworkSceneManagerDefault>();
        Instance = this;
    }

    public async void Register(ScoreObject score)
    {
        await Task.Delay(1000);
        /*_scoreObjects.Add(score.Object.InputAuthority, score);
        if (_scoreObjects.Count == 1)
        {
            //一人目はAチーム
            _scoreObjects[score.Object.InputAuthority].camp = Camp.A;
            p1 = score.Object.InputAuthority;
        }
        else
        {
            //二人目はBチーム
            _scoreObjects[score.Object.InputAuthority].camp = Camp.B;
            p2 = score.Object.InputAuthority;
        }
        */
        if (_uiManager != null)
        {
            _uiManager.OnPlayerJoined(score);
        }
    }

    public void Unregister(ScoreObject score)
    {
        if (score.Object == null || _scoreObjects.ContainsKey(score.Object.InputAuthority) == false)
        {
            return;
        }

        if (_uiManager != null)
        {
            _uiManager.OnPlayerLeft(score);
        }

        _scoreObjects.Remove(score.Object.InputAuthority);
    }

    public ScoreObject GetScoreObjectFor(PlayerRef player)
    {
        if (_scoreObjects.ContainsKey(player) == false)
        {
            return null;
        }

        return _scoreObjects[player];
    }

    public void OnScoreChanged(ScoreObject scoreObject, float score, Camp camp)
    {
        if (_uiManager != null)
        {
            _uiManager.OnScoreChanged(scoreObject, score, camp);
        }
    }

    public void FinishGame(Camp camp)
    {
        if (camp == Camp.A)
        {
            RoomPlayer.Local.hasWined = GameLauncher.Runner.GameMode != GameMode.Host;
        }
        else
        {
            RoomPlayer.Local.hasWined = GameLauncher.Runner.GameMode == GameMode.Host;
        }
        Debug.Log("勝敗：" + RoomPlayer.Local.hasWined);
        _networkSceneManagerDefault.Runner.SetActiveScene(2);
    }
}