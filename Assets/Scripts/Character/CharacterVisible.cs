using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Fusion;

namespace Unit
{
    public class CharacterVisible : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] VisibleObjects;
        private CharacterStatus MyStatus;
        void Start()
        {
            MyStatus = GetComponent<CharacterStatus>();
            /*
            MyStatus
                .OniVisibleChanged
                .Subscribe(value =>
                {
                    RPC_ChangeVisible(value);
                }
            )
            .AddTo(this); ;
            */
        }
        public void Visible(bool value)
        {
            Debug.Log($"Visible{value}:{VisibleObjects.Length}");
                foreach (var Objects in VisibleObjects)
                {
                    Objects.SetActive(value);
                }
        }
        /// <summary>
        /// éãäoâªÇÃêÿÇËë÷Ç¶
        /// </summary>
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_ChangeVisible(bool value)
        {   
            if(!HasInputAuthority)
            {
                foreach (var Objects in VisibleObjects)
                {
                    Objects.SetActive(value);
                }
            }
        } 
    }
}