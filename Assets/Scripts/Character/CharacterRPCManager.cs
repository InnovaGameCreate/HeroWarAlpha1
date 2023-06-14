using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Fusion;

namespace Unit
{
    public class CharacterRPCManager : NetworkBehaviour
    {
        [Header("��{�f�[�^�̎Q��")]
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
                    Debug.Log("�����l�̐ݒ肪����܂���");
                    Init();
                })
                .AddTo(this);
        }

        /// <summary>
        /// �����ݒ�
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


            //���ݏ�Ԃ̓���
            MyCharacterProfile
                .OnCharacterStateChanged
                .Subscribe(CharacterState =>
                {
                    RPC_StateShow(CharacterState.ToString());
                }
             ).AddTo(this);
        }


        /// <summary>
        /// ���o���̐؂�ւ�
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ChangeVisible(bool value)
        {
            if (MyCharacterVisible != null)
            {
                Debug.Log($"MyCharacterVisible��{value}�ɐ؂�ւ��܂���");
                MyCharacterVisible.Visible(value);
            }
        }

        /// <summary>
        /// �̗̓o�[�̍X�V
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetHp(float value)
        {
            if (MyDesplayProfile != null)
            {
                //Debug.Log($"MyHp��{value}�ɐ؂�ւ��܂���");
                MyDesplayProfile.SetHp(value); 
            }
        }
        /// <summary>
        /// ��Ԃ̍X�V
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_StateShow(string value)
        {
            if (MyDesplayProfile != null)
            {
                //Debug.Log($"RPC_StateShow��{value}�ɐ؂�ւ��܂���");
                MyDesplayProfile.StateShow(value);
            }
        }
    }
}