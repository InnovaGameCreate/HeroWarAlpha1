using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Fusion;

namespace Unit
{
    public class CharacterRPCManager : NetworkBehaviour
    {
        [Header("基本データの参照")]
        [SerializeField]
        private CharacterStatus MyCharacterStatus;
        [SerializeField]
        private CharacterProfile MyCharacterProfile;        
        [SerializeField]
        private CharacterVisible MyCharacterVisible;
        [SerializeField]
        private DesplayProfile MyDesplayProfile;
        private void Start()
        {
            MyCharacterProfile
                .OninitialSetting
                .Where(value => value == true)
                .Subscribe(_ =>
                {
                    Debug.Log("初期値の設定がされました");
                    Init();
                })
                .AddTo(this);
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        private void Init()
        {
            float maxHp = MyCharacterProfile.MyHp;
            //Debug.Log($"MaxHp = {maxHp}");
            MyCharacterStatus
                .OniVisibleChanged
                .Subscribe(value =>
                {
                    Debug.Log("Subscribe:RPC_ChangeVisible");
                    RPC_ChangeVisible(value);
                }
            )
            .AddTo(this);
            RPC_ChangeVisible(true);

            MyCharacterProfile
                 .OncharacterHPChanged
                 .Subscribe(characterHP =>
                 {
                     float currentHP = characterHP / maxHp;
                     RPC_SetHp(currentHP);
                 }
             ).AddTo(this);


            //現在状態の同期
            MyCharacterProfile
                .OnCharacterStateChanged
                .Subscribe(CharacterState =>
                {
                    RPC_StateShow(CharacterState.ToString());
                }
             ).AddTo(this);
        }


        /// <summary>
        /// 視覚化の切り替え
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ChangeVisible(bool value)
        {
            if (MyCharacterVisible != null)
            {
                Debug.Log($"MyCharacterVisibleを{value}に切り替えました");
                MyCharacterVisible.Visible(value);
            }
        }

        /// <summary>
        /// 体力バーの更新
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetHp(float value)
        {
            if (MyDesplayProfile != null)
            {
                //Debug.Log($"MyHpを{value}に切り替えました");
                MyDesplayProfile.SetHp(value); 
            }
        }
        /// <summary>
        /// 状態の更新
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_StateShow(string value)
        {
            if (MyDesplayProfile != null)
            {
                //Debug.Log($"RPC_StateShowを{value}に切り替えました");
                MyDesplayProfile.StateShow(value);
            }
        }
    }
}