/*
    FlagSceneで移動に利用しているだけなので統合時消します　小林
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class FlagOnMove : MonoBehaviour
{
    void Start()
    {
        // プレイヤー移動のイベント
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.W))
            .Subscribe(_ => this.gameObject.transform.Translate(new Vector3(0, 0, 3 * Time.deltaTime)));
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.S))
            .Subscribe(_ => this.gameObject.transform.Translate(new Vector3(0, 0, -3 * Time.deltaTime)));
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.D))
            .Subscribe(_ => this.gameObject.transform.Translate(new Vector3(3 * Time.deltaTime, 0, 0)));
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.A))
            .Subscribe(_ => this.gameObject.transform.Translate(new Vector3(-3 * Time.deltaTime, 0, 0)));
    }
}
