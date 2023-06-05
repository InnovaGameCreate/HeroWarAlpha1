/*
    FlagSceneで利用しているだけなので統合時消します　
    なお、かわりに各プレイヤにこの変数を持たせたいです    

    小林
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagOnPlayer : MonoBehaviour
{
    public Camp myCamp = Camp.A;        // 所属陣営

    public int hp { get; set; } = 100;
    public int max_hp { get; set; } = 100;
    public int speed_heal { get; set; } = 5;
    public int speed_move { get; set; } = 8;
    public int speed_attack { get; set; } = 60;
    public int range_hit { get; set; } = 300;
    public int attack { get; set; } = 15;
    public int defense { get; set; } = 0;
    public int static_hitrate { get; set; } = 90;
    public int dynamic_hitrate { get; set; } = 90;

    public int range_search { get; set; } = 2000;
}
