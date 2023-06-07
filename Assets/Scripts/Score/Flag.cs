/*
    Flag�i���j�ɃA�^�b�`����
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public enum Camp
{
    A,
    B,
    NONE,
}
public class Flag : MonoBehaviour
{
    // �R���|�[�l���g
    [SerializeField]
    //private GameObject scoreManagerObj;

    public static Camp holder = Camp.NONE;  // �t���O�̏��L��
    Color[] flagColor = { Color.blue, Color.red, Color.white }; //�t���O�̐F�iA:��,B:��,�ǂ���ł��Ȃ�:���j

    /* �t���O�̏��L�҂��Z�b�g���� */
    private void SetFlagHost(Camp camp)
    {
        holder = camp;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Unit"))
        {   
            SetFlagHost(other.GetComponentInChildren<FlagOnPlayer>().myCamp);
            gameObject.GetComponent<Renderer>().material.color = flagColor[(int)holder];    //�F��ύX
        }
    }

}
