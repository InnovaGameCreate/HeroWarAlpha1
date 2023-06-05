using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Unit
{
    public class CharacterVisible : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] VisibleObjects;
        private CharacterStatus MyStatus;
        void Start()
        {
            MyStatus = GetComponent<CharacterStatus>();
            MyStatus
                .OniVisibleChanged
                .Subscribe(value =>
                {
                    ChangeVisible(value);
                }
            );
        }

        //�ݒ肵���I�u�W�F�N�g�̕\����S�Đ؂�ւ���
        private void ChangeVisible(bool value)
        {
            //Debug.Log($"�I�u�W�F�N�g��\��{value}");
            foreach (var Objects in VisibleObjects)
            {
                Objects.SetActive(value);
            }
        }
    }
}