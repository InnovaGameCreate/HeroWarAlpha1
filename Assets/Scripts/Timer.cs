using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Timer : MonoBehaviour
{
    private const int limitTime = 100;    //制限時間(秒)
    public ReactiveProperty<int> second; // 時間(秒)

    void Start()
    {
        second.Value = 0;
        StartCoroutine(TimerFunc());
    }

    /* プレイ時間をカウントする */
    private IEnumerator TimerFunc()
    {
        while (second.Value < limitTime)
        {
            yield return new WaitForSeconds(1);
            second.Value++;
        }
    }
}
