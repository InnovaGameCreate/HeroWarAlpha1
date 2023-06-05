using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Timer : MonoBehaviour
{
    private const int limitTime = 100;    //��������(�b)
    public ReactiveProperty<int> second; // ����(�b)

    void Start()
    {
        second.Value = 0;
        StartCoroutine(TimerFunc());
    }

    /* �v���C���Ԃ��J�E���g���� */
    private IEnumerator TimerFunc()
    {
        while (second.Value < limitTime)
        {
            yield return new WaitForSeconds(1);
            second.Value++;
        }
    }
}
