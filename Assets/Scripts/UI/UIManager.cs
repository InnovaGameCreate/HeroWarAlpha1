using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    UIScoreElement element;
    Dictionary<ScoreObject, List<UIScoreElement>> _scoreUi = new Dictionary<ScoreObject, List<UIScoreElement>>();
    [SerializeField] UIScoreElement _prefab;

    /*private void Start()
    {
        scoreManager.scoreArray[(int)Camp.A].Subscribe(e => UpdateScoreSlider(e, Camp.A));  //右側(A)のスコアを監視しUIを更新する
        scoreManager.scoreArray[(int)Camp.B].Subscribe(e => UpdateScoreSlider(e, Camp.B));   //右側(B)のスコアを監視しUIを更新する

        timer.second.Subscribe(e => UpdateTimerText());  //右側(B)のスコアを監視しUIを更新する
    }*/
    private void Awake()
    {
        element = Instantiate(_prefab, transform);  //スコアバーの生成
    }

    public void OnScoreChanged(ScoreObject scoreObject, float score, Camp camp)    //スコアが変わったら
    {
        
        if (_scoreUi.ContainsKey(scoreObject) == false)  //scoreObjectというキーがあるかどうか
        {
            return; //無ければ抜ける
        }
        var list = _scoreUi[scoreObject];
        
        foreach (var item in list)
        {
            item.UpdateScoreSlider(score, camp);
        }
    }


    public void OnPlayerJoined(ScoreObject scoreObject)
    {
        if (_scoreUi.ContainsKey(scoreObject) == false)
        {
            _scoreUi.Add(scoreObject, new List<UIScoreElement>());
        }
        _scoreUi[scoreObject].Add(element); //各プレイヤーとスコアバーを紐づける
    }

    public void OnPlayerLeft(ScoreObject scoreObject)
    {
        if (_scoreUi.ContainsKey(scoreObject))
        {
            foreach (var item in _scoreUi[scoreObject])
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            _scoreUi[scoreObject].Clear();
        }
    }

    /*
    private void Update()
    {
        element.UpdateTimerText();
    }
    */
}
