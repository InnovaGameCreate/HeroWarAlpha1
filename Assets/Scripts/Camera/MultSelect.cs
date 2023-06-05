using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;
using System.Linq;


public class MultSelect : MonoBehaviour
{
    private Vector3 SelectStartPos;
    private Vector3 SelectEndPos;
    [SerializeField]
    private GameObject MarkPoint;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(Select());
    }

    IEnumerator Select()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKey(KeyCode.Mouse0));//範囲選択の開始地点の決定
            SelectStartPos = ScreenPos();
            var MarkStartPoint = Instantiate(MarkPoint, SelectStartPos, Quaternion.identity);

            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Mouse0));//範囲選択の終了地点の決定
            SelectEndPos = ScreenPos();
            var MarkEndPoint = Instantiate(MarkPoint, SelectEndPos, Quaternion.identity);

            if (SelectAreaValue() <= 10)
            {
                Destroy(MarkStartPoint);
                Destroy(MarkEndPoint);
            }
            else
            {
                var OwnerObjects = FindObjectsOfType<CharacterProfile>();
                List<CharacterSelect> SelectedObject = new List<CharacterSelect>();
                foreach (var characters in OwnerObjects)
                {
                    if (InSelectArea(characters.gameObject))
                    {
                        if (characters.GetCharacterOwnerType() == OwnerType.Player)
                        {
                            var CharacterMoveCs = characters.GetComponent<CharacterSelect>();
                            CharacterMoveCs.MultSelected(true);

                            SelectedObject.Add(CharacterMoveCs);
                        }
                    }
                }
                yield return new WaitUntil(() => Input.GetKey(KeyCode.Mouse0));
                Destroy(MarkStartPoint);
                Destroy(MarkEndPoint);
                foreach (var charactersCs in SelectedObject)
                {
                    charactersCs.MultSelected(false);
                }
            }
        }
    }

    private Vector3 ScreenPos()//画面での座標計算
    {
        var MousePosition = Input.mousePosition;
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        var raycastHitList = Physics.RaycastAll(ray).ToList();
        if (raycastHitList.Any())
        {
            var point = raycastHitList.First().point;
            var Ydistance = transform.position.y - point.y;
            MousePosition.z = Ydistance;
        }

        var objPosition = Camera.main.ScreenToWorldPoint(MousePosition);

        float radium = objPosition.y / Mathf.Cos(30f * Mathf.Deg2Rad);
        float ModZ = radium * Mathf.Sin(30f * Mathf.Deg2Rad);

        var SelectPoint = new Vector3(objPosition.x, 0, objPosition.z + ModZ);

        return SelectPoint;
    }

    private bool InSelectArea(GameObject targetObject)
    {
        if ((targetObject.transform.position.x >= SelectStartPos.x
        && targetObject.transform.position.x <= SelectEndPos.x)
        || (targetObject.transform.position.x <= SelectStartPos.x
        && targetObject.transform.position.x >= SelectEndPos.x))
        {
            if ((targetObject.transform.position.z >= SelectStartPos.z
            && targetObject.transform.position.z <= SelectEndPos.z)
            || (targetObject.transform.position.z <= SelectStartPos.z
            && targetObject.transform.position.z >= SelectEndPos.z))
            {
                return true;
            }
        }
        return false;
    }

    private float SelectAreaValue()
    {
        float xRange = SelectStartPos.x - SelectEndPos.x;
        float zRange = SelectStartPos.z - SelectEndPos.z;
        float Value = Mathf.Abs(xRange * zRange);
        return Value;
    }
}