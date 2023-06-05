using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Click : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI selectText;

    public void SelectA()
    {
        Destroy(this.gameObject);
        selectText.text = "Matching";
    }
    public void OnClick()
    {
        Destroy(this.gameObject);
        //Instantiate(Branches, transform.position, Quaternion.identity);
    }
}
