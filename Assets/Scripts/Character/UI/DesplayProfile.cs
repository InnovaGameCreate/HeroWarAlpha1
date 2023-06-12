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
        [Header("ステータスを表すTMP")]
        [SerializeField]
        private TextMeshProUGUI CharacterStateTMP;
        [Header("ユニットに関する画像")]
        [SerializeField]
        private Image InSelectImage;
        [SerializeField]
        private Image CharacterAttackAreaImage;
        [SerializeField]
        private Image CharacterSearchAreaImage;
        [SerializeField]
        private Image UnitImage;
        [Header("HPバー")]
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

            //表示される索敵範囲と攻撃範囲の適応
            Vector2 AttackAreaSize = new Vector2(MyCharacterProfile.MyattackRange * 20, MyCharacterProfile.MyattackRange * 20);
            Vector2 SearchAreaSize = new Vector2(MyCharacterProfile.MysearchRange * 20, MyCharacterProfile.MysearchRange * 20);
            CharacterAttackAreaImage.rectTransform.sizeDelta = AttackAreaSize;
            CharacterSearchAreaImage.rectTransform.sizeDelta = SearchAreaSize;

            //アイコンの変更
            setUnitImage(MyCharacterProfile.MyUnitType);

            //現在体力の同期
            MaxHP = MyCharacterProfile.MyHp;

            /*
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
                    //RPC_SetHp(currentHP);
                    if (HasInputAuthority) RPC_SetHp(currentHP);
                }
            ).AddTo(this);*/

            //現在状態の同期
            MyCharacterProfile
                .OnCharacterStateChanged
                .Subscribe(characterState =>
                {
                    //CharacterStateTMP.transform.LookAt(Camera.transform);
                    //CharacterStateTMP.text = CharacterState.ToString();
                    if (Object.HasInputAuthority) RPC_StateShow(characterState);
                }
            ).AddTo(this);

            //選択された際に、それが分かるようにするImageを表示
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
        /// ユニットイメージを設定
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

        //ネットワークを介して状態を表すテキストを更新
        //public void StateShow(string state)
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_StateShow(CharacterState state)
        {
            CharacterStateTMP.transform.LookAt(Camera.transform);
            CharacterStateTMP.text = state;
        }

        //ネットワークを介して状態を表すテキストを更新
        //public void SetHp(float currentHP)
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_SetHp(float hp)
        {
            HpBar.fillAmount = currentHP;
        }
    }
}