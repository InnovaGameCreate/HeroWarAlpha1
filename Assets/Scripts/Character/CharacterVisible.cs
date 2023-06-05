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

        //設定したオブジェクトの表示を全て切り替える
        private void ChangeVisible(bool value)
        {
            //Debug.Log($"オブジェクトを表示{value}");
            foreach (var Objects in VisibleObjects)
            {
                Objects.SetActive(value);
            }
        }
    }
}