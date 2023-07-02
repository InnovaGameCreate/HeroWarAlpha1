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
        private float damage;
        private float reloadTime;
        [SerializeField]
        private GameObject BulletPrefab;
        [SerializeField]
        private Transform FirePosition;
        private GameObject TargetObject;
        private bool attackable = false;
        public void Start()
        {
            base.Spawned();
            MyDrawer = GetComponent<DrawAttackLine>();
            MyCharacterProfile = GetComponentInParent<CharacterProfile>();

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
                .Subscribe(x =>
                {
                    if (x != null)
                    {
                        Debug.Log($"攻撃状態を開始:HasInputAuthority = {HasInputAuthority}");
                        if (MyCharacterProfile.GetCharacterState() == CharacterState.Move)
                        {
                            MyCharacterProfile.ChangeCharacterState(CharacterState.MoveAttack);                 //攻撃状態に移行
                        }
                        else
                        {
                            MyCharacterProfile.ChangeCharacterState(CharacterState.Attack);                 //攻撃状態に移行
                        }
                        TargetObject = x;
                        StateAction();
                    }
                    else
                    {
                        Debug.Log("攻撃状態を終了:HasInputAuthority = {HasInputAuthority}");
                        EndStateAction();
                    }
                }
            ).AddTo(this);

            MyCharacterProfile
             .OninitialSetting
             .Where(value => value == true)
             .Subscribe(_ =>
             {
                 //Debug.Log("CharacterSearch:初期値の設定がされました");
                 Init();
             })
             .AddTo(this);
        }

        private void Init()
        {            
            damage = MyCharacterProfile.MyAttackPower;
            reloadTime = MyCharacterProfile.MyReloadSpeed;
        }

        IEnumerator Attack()
        {

            if (TargetObject.gameObject.TryGetComponent(out IDamagable DamageCs) && isCharacterlive)
            {
                GameObject attackTargetObject = TargetObject;
                while (true)
                {
                    Debug.Log("攻撃中");
                    if (!isCharacterlive || TargetObject != attackTargetObject)
                    {
                        MyDrawer.ClearLine();
                        MyCharacterProfile.ChangeCharacterState(CharacterState.Idle);                   //待機状態に移行
                        yield break;
                    }
                    if (HasInputAuthority) MyDrawer.DrawLine(TargetObject.transform.position);          //線を引く

                    if (CanAttackState())
                    {
                        transform.parent.gameObject.transform.LookAt(TargetObject.transform.position);  //攻撃対象に向く
                        if (MyCharacterProfile.HasStateAuthority) DamageCs.AddDamage(damage);           //敵キャラクターにダメージを与える
                        yield return ShotEffect();                                                      //攻撃エフェクトを発生させる

                        yield return new WaitForSeconds(reloadTime);
                        if (TargetObject == null)
                        {
                            Debug.Log("NullBreak");
                            break;                                                                      //攻撃対象がいないとブレイク
                        }
                    }
                    else
                    {
                        Debug.Log("CanAttackState() = False");
                        break;                                                                          //対象がいない場合はブレイク
                    }
                }
                Debug.Log("AttackLoop()Break");
                EndStateAction();
            }
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
        /// characterが戦闘できる状態かどうか
        /// </summary>
        private bool CanAttackState()
        {

            if (MyCharacterProfile.GetCharacterState() == CharacterState.Move)
            {
                MyCharacterProfile.ChangeCharacterState(CharacterState.MoveAttack);                 //攻撃状態に移行
            }
            else if(MyCharacterProfile.GetCharacterState() == CharacterState.VigilanceMove || MyCharacterProfile.GetCharacterState() == CharacterState.Idle)
            {
                MyCharacterProfile.ChangeCharacterState(CharacterState.Attack);                 //攻撃状態に移行
            }

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
            StopCoroutine(ShotEffect());
            StartCoroutine(Attack());
        }

        public override void EndStateAction()
        {
            StopCoroutine(Attack());
            StopCoroutine(ShotEffect());
        }
    }
}