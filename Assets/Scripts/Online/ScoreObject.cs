using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System.Threading.Tasks;
using UniRx.Triggers;
using Fusion;
using UnityEngine.SceneManagement;

public class ScoreObject : NetworkBehaviour
{
    public const float addPoint = 20f;          //  (フレーム当たり)加算する点数
    public const float winPoint = 500f;   // 勝ちとなるスコア
    [Networked(OnChanged = nameof(OnScoreChanged))]
    [SerializeField] float _score { get; set; }
    public ReactiveProperty<float> scoreArray;// プレイヤーのスコア配列
    public Camp camp;
    
    void Start()
    {
        // スコアの加算イベント
        this.UpdateAsObservable()
            .Where(_ => Flag.holder == camp && scoreArray.Value < winPoint)
            .Subscribe(_ => AddPoint())
            .AddTo(this);

        // スコアがゲーム終了の勝ち点を超えた時のイベント
        this.UpdateAsObservable()
            .Where(_ => scoreArray.Value >= winPoint)
            .Subscribe(_ => ScoreManager.Instance.FinishGame(camp))
            .AddTo(this);
    }

    private async void OnEnable()
    {
        await Task.Delay(1000);
        if (Object.InputAuthority.ToString() == "[Player:0]")
        {
            camp = Camp.A;
        }
        else if (Object.InputAuthority.ToString() == "[Player:1]")
        {
            camp = Camp.B;
        }
    }

    public override void Spawned()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.Register(this);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.Unregister(this);
        }
    }

    static void OnScoreChanged(Changed<ScoreObject> changed)
    {
        ScoreManager.Instance?.OnScoreChanged(changed.Behaviour, changed.Behaviour._score, changed.Behaviour.camp);
    }

    private void AddPoint() //得点の加算
    {
        if (Flag.holder != Camp.NONE)
        {
            scoreArray.Value += addPoint * Time.deltaTime;
            camp = Flag.holder;
            _score = scoreArray.Value;
        }
    }
}
