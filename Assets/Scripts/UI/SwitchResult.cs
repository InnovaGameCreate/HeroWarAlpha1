using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwitchResult : MonoBehaviour
{
    [SerializeField]
    private int Result = 0;
    const int WIN = 0;
    const int LOSE = 1;
    [SerializeField]
    TextMeshProUGUI[] SwitchUI;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Result == WIN)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
