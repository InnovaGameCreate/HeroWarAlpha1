using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using Fusion;

namespace Unit
{
    public class CharacterAttack : CharacterStateAction

    {
        CharacterProfile MyCharacterProfile;
        DrawAttackLine MyDrawer;
        private bool isCharacterlive = true;
        private bool isAttack = true;
        private bool loseSight = false;//敵を見失う
        private float damage;
        private float attackRange;
        private float reloadTime;
        [SerializeField]
        private GameObject BulletPrefab;
        [SerializeField]
        private Transform FirePosition;
        private SphereCollider AttaclColleder;
        private List<GameObject> targetObjects = new List<GameObject>();
        private GameObject TargetObject;
        
        public override void Spawned()
        {
            base.Spawned();
            MyDrawer = GetComponent<DrawAttackLine>();
            MyCharacterProfile = GetComponentInParent<CharacterProfile>();
            AttaclColleder = GetComponent<SphereCollider>();

            MyCharacterProfile
                .OnCharacterStateChanged
                .Where(CharacterState => CharacterState == CharacterState.Dead)
                .Subscribe(_ =>
                {
                    isCharacterlive = false;
                    EndStateAction();
                }
            ).AddTo(this);

            MyCharacterProfile
                .OnCharacterTargetObject
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    if(MyCharacterProfile.GetCharacterState() == CharacterState.Move)
                    {
                        MyCharacterProfile.ChangeCharacterState(CharacterState.MoveAttack);                 //攻撃状態に移行
                    }
                    else
                    {
                        MyCharacterProfile.ChangeCharacterState(CharacterState.Attack);                 //攻撃状態に移行
                    }
                    if (HasInputAuthority) Debug.Log("攻撃開始：" + Object.InputAuthority);
                    TargetObject = x;
                    StateAction();
                }
            ).AddTo(this);
            SetInputAuthority();
        }

        private async void SetInputAuthority()
        {
            await Task.Delay(100);
            
            damage = MyCharacterProfile.MyAttackPower;
            attackRange = MyCharacterProfile.MyattackRange;
            AttaclColleder.radius = MyCharacterProfile.MysearchRange;
            reloadTime = MyCharacterProfile.MyReloadSpeed;
        }

        IEnumerator Attack()
        {
            if (TargetObject.gameObject.TryGetComponent(out IDamagable DamageCs) && isCharacterlive)
            {
                isAttack = false;
                loseSight = false;
                Debug.Log("攻撃中");

                StartCoroutine(EnemyKill());
                while (true)
                {
                    if (!isCharacterlive) break;
                    if (HasInputAuthority) MyDrawer.DrawLine(TargetObject.transform.position);           //線を引く
                    if (CanAttackState())
                    {
                        transform.parent.gameObject.transform.LookAt(TargetObject.transform.position);  //攻撃対象に向く
                        if (HasStateAuthority) DamageCs.AddDamage(damage);                              //敵キャラクターにダメージを与える
                        yield return ShotEffect();                                                      //攻撃エフェクトを発生させる

                        yield return new WaitForSeconds(reloadTime);
                        if (TargetObject == null) break;                                                    //攻撃対象がいないとブレイク
                        if (!isCharacterlive || !AttackRangeCheck(TargetObject) || !CanAttackState()) break;//対象がいない場合はブレイク

                    }
                    loseSight = true;
                }
            }
        }

        //敵を倒したか判断するためのコルーチン
        IEnumerator EnemyKill()
        {
            var TargetState = TargetObject.GetComponent<CharacterProfile>().GetCharacterState();         //敵の状態を取得
            yield return new WaitUntil(() => TargetState == CharacterState.Dead || loseSight);           //敵が死亡しているか、見失った場合まで処理を待機
            if (isCharacterlive) MyCharacterProfile.ChangeCharacterState(CharacterState.Idle);           //待機状態に移行
            if (loseSight) loseSight = false;
            //if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
            MyDrawer.ClearLine();                                                                        //線を消す
            MyCharacterProfile.SetTarget(null);                                                          //いらない敵をLIstから削除
            isAttack = true;
        }

        IEnumerator ShotEffect()//射撃エフェクト
        {
            for (int i = 0; i < 3; i++)
            {
                //Instantiate(BulletPrefab, FirePosition.transform.position, FirePosition.transform.rotation);
                if (Object.HasInputAuthority) RPC_ShotBullet();
                yield return new WaitForSeconds(0.2f);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_ShotBullet()
        {
            Instantiate(BulletPrefab, FirePosition.transform.position, FirePosition.transform.rotation);
        }


        /// <summary>
        /// 対象が射程範囲内かどうか
        /// </summary>
        private bool AttackRangeCheck(GameObject TargetGameobject)
        {
            if (attackRange > ObjectsDistance(TargetGameobject)) return true;
            else return false;
        }

        /// <summary>
        /// 二つのオブジェクトの距離
        /// </summary>
        private float ObjectsDistance(GameObject TagetObject)
        {
            return Vector3.Distance(TagetObject.transform.position, transform.position);
        }

        /// <summary>
        /// characterが戦闘できる状態かどうか
        /// </summary>
        private bool CanAttackState()
        {
            if (MyCharacterProfile.GetCharacterState() == CharacterState.Attack
                || MyCharacterProfile.GetCharacterState() == CharacterState.MoveAttack)
            {
                return true;
            }
            else return false;
        }

        public override void StartStateAction()
        {
        }

        public override void StateAction()
        {
            StopCoroutine(Attack());
            StopCoroutine(EnemyKill());
            StopCoroutine(ShotEffect());
            StartCoroutine(Attack());
        }

        public override void EndStateAction()
        {
            StopCoroutine(Attack());
            StopCoroutine(EnemyKill());
            StopCoroutine(ShotEffect());
        }
    }
}