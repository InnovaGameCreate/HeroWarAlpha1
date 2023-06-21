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
            /*
            RPC_ChangeVisible(false);

            MyStatus = GetComponent<CharacterStatus>();
            
            MyStatus
                .OniVisibleChanged
                .Subscribe(RPC_ChangeVisible)
                .AddTo(this);
             */
        }
        public void Visible(bool value)
        {
            if(HasInputAuthority)
            {
                Debug.Log($"HasInputAuthority : Visible{value}:{VisibleObjects.Length}");
            }
            else
            {
                Debug.Log($"NotHasInputAuthority : Visible{value}:{VisibleObjects.Length}");
                foreach (var Objects in VisibleObjects)
                {
                    Objects.SetActive(value);
                }
            }
        }
        /// <summary>
        /// Ž‹Šo‰»‚ÌØ‚è‘Ö‚¦
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ChangeVisible(bool value)
        {
            if (HasInputAuthority) return;
            Debug.Log("見えない物を変えます" + value);
            foreach (var Objects in VisibleObjects)
            {
                Objects.SetActive(value);
            }
        } 
    }
}