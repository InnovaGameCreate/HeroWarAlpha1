using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIScoreElement : MonoBehaviour
{
    [SerializeField]
    private Slider[] scoreSlideres;   //  スコアスライダー
    [SerializeField]
    private Text[] scoreTexts; // スコア表示テキスト
    [SerializeField]
    private Text timerText; // タイマーの時間表示テキスト
    [SerializeField]
    private Timer timer;
    // Start is called before the first frame update

    private void Awake()
    {
        timer = GameObject.Find("Timer").GetComponent<Timer>();
    }
    public void UpdateScoreSlider(float score, Camp camp)
    {
        float fill = score / ScoreObject.winPoint;   //色のついた部分を示す変数　(現在のスコア)/(最大スコア)
        
        scoreSlideres[(int)camp].value = (camp == Camp.A) ? 1 - fill : fill;  // スライダーのvalueの変更
        scoreTexts[(int)camp].text = score.ToString("000");
    }

    public void UpdateTimerText()
    {
        timerText.text = timer.second.ToString();   // タイマーの表示テキスト
    }
}
