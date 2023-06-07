using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMouseEvent : MonoBehaviour
{
    private GameObject clickUnit = null;    // クリックしたUnitの識別

    // コンポーネント
    [SerializeField]
    private UIManager uiManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickUnit = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    clickUnit = hit.collider.gameObject;
                }
            }
        }

        /*
        
        // Unitをクリックしていないなら
        if (clickUnit == null)
        {
            uiManager.showStatusPanel(null);
        }
        else
        {
            uiManager.showStatusPanel(clickUnit.GetComponentInParent<FlagOnPlayer>());
        }
        */

    }
}
