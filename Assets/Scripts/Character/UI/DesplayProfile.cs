using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;

namespace Unit
{
    public class DesplayProfile : NetworkBehaviour
    {
        [Header("�X�e�[�^�X��\��TMP")]
        [SerializeField]
        private TextMeshProUGUI CharacterStateTMP;
        [Header("���j�b�g�Ɋւ���摜")]
        [SerializeField]
        private Image InSelectImage;
        [SerializeField]
        private Image CharacterAttackAreaImage;
        [SerializeField]
        private Image CharacterSearchAreaImage;
        [SerializeField]
        private Image UnitImage;
        [Header("HP�o�[")]
        [SerializeField]
        private Image HpBar;
        private float MaxHP;
        CharacterProfile MyCharacterProfile;
        CharacterMove MyCharacterMove;
        private GameObject Camera;
        private List<Sprite> IconList;
        
        public override void Spawned()
        {
            base.Spawned();
            Camera = FindObjectOfType<Camera>().gameObject;
            MyCharacterProfile = GetComponentInParent<CharacterProfile>();
            MyCharacterMove = GetComponentInParent<CharacterMove>();
            InSelectImage.color = Color.clear;

            //�\���������G�͈͂ƍU���͈͂̓K��
            Vector2 AttackAreaSize = new Vector2(MyCharacterProfile.MyattackRange * 20, MyCharacterProfile.MyattackRange * 20);
            Vector2 SearchAreaSize = new Vector2(MyCharacterProfile.MysearchRange * 20, MyCharacterProfile.MysearchRange * 20);
            CharacterAttackAreaImage.rectTransform.sizeDelta = AttackAreaSize;
            CharacterSearchAreaImage.rectTransform.sizeDelta = SearchAreaSize;

            //�A�C�R���̕ύX
            setUnitImage(MyCharacterProfile.MyUnitType);

            //���ݑ̗͂̓���
            MaxHP = MyCharacterProfile.MyHp;

            MyCharacterProfile
                .OncharacterHPChanged
                .Subscribe(characterHP =>
                {
                    if(MaxHP < characterHP)
                    {
                        MaxHP = characterHP;
                    }
                    //HpBar.fillAmount = characterHP / MaxHP;
                    var currentHP = characterHP / MaxHP;
                    if (HasInputAuthority) RPC_SetHp(currentHP);
                }
            ).AddTo(this);

            //���ݏ�Ԃ̓���
            MyCharacterProfile
                .OnCharacterStateChanged
                .Subscribe(characterState =>
                {
                    //CharacterStateTMP.transform.LookAt(Camera.transform);
                    //CharacterStateTMP.text = CharacterState.ToString();
                    if (Object.HasInputAuthority) RPC_StateShow(characterState);
                }
            ).AddTo(this);

            //�I�����ꂽ�ۂɁA���ꂪ������悤�ɂ���Image��\��
            MyCharacterMove
                .isSelect
                .Subscribe(SelectValue =>
                {
                    if (SelectValue == true)
                    {
                        InSelectImage.color = Color.green;
                        CharacterAttackAreaImage.color = Color.red;
                        CharacterSearchAreaImage.color = Color.yellow;
                    }
                    else
                    {
                        InSelectImage.color = Color.clear;
                        CharacterAttackAreaImage.color = Color.clear;
                        CharacterSearchAreaImage.color = Color.clear;
                    }
                }
            ).AddTo(this);
        }

        /// <summary>
        /// ���j�b�g�C���[�W��ݒ�
        /// </summary>
        private void setUnitImage(UnitType iconType)
        {
            UnitIcon unitIconResources = Resources.Load<UnitIcon>("UnitIcon");
            IconList = unitIconResources.unitIconList;
            var IconImage = IconList.FirstOrDefault(icon => icon.name.Equals(iconType.ToString()));
            if(IconImage != null)
            {
                UnitImage.sprite = IconImage;
            }
        }

        //�l�b�g���[�N����ď�Ԃ�\���e�L�X�g���X�V
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_StateShow(CharacterState state)
        {
            CharacterStateTMP.transform.LookAt(Camera.transform);
            CharacterStateTMP.text = state.ToString();
        }

        //�l�b�g���[�N����ď�Ԃ�\���e�L�X�g���X�V
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_SetHp(float hp)
        {
            HpBar.fillAmount = hp;
        }
    }
}