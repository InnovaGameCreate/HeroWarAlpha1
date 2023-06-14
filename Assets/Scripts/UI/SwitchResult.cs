using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwitchResult : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI[] SwitchUI;
    
    void Start()
    {
        if(RoomPlayer.Local.hasWined)
        {
            SwitchUI[0].enabled = true;
            SwitchUI[1].enabled = false;
        }
        else
        { 
            SwitchUI[0].enabled = false;
            SwitchUI[1].enabled = true;
        }
            
    }
}
