using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] LogText;
    private Vector3 LastEventPosition;
    GameObject CameraObject;

    private void Start()
    {
        CameraObject = FindObjectOfType<Camera>().gameObject;
         StartCoroutine(MoveLastEventPosition());
    }
    public void AddText(string Text, Vector3 eventPosition)
    {
        LastEventPosition = eventPosition;
        for (int i = 0; i < 5; i++)
        {
            LogText[i].text = LogText[i + 1].text;
        }
        LogText[5].text = Text;
    }

    IEnumerator MoveLastEventPosition()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKey(KeyCode.Space));
            MoveEventPosition();
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Space));
        }
    }

    private void MoveEventPosition()
    {
        CameraObject.transform.position = LastEventPosition + new Vector3(0, 30, -10);
    }
}
