using UnityEngine;
using System.Threading.Tasks;
using UniRx;
using System;
using Fusion;
using Online;

namespace Unit
{
    public class CharacterStatus : NetworkBehaviour
    {
        CharacterProfile MyCharacterProfile;
        CharacterMove MyCharacterMove;
        Animator MyAnimator;
        private LogManager DeadkLogManager;
        private ReactiveProperty<bool> iVisible = new ReactiveProperty<bool>(true);
        [Networked] private TickTimer life { get; set; }

        public IObservable<bool> OniVisibleChanged//characterHPが変更された際に発光されるイベント
        {
            get { return iVisible; }
        }
        
        public override void Spawned()
        {
            base.Spawned();
            MyAnimator = GetComponent<Animator>();
            MyCharacterProfile = GetComponent<CharacterProfile>();
            MyCharacterMove = GetComponent<CharacterMove>();
            DeadkLogManager = FindObjectOfType<LogManager>();

            if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Enemy)
            {
                iVisible.Value = false;
            }

            MyCharacterProfile
                .OncharacterHPChanged
                .Where(characterHP => characterHP <= 0)
                .Subscribe(_ =>
                {
                    life = TickTimer.CreateFromSeconds(Runner, 3.0f);
                    StateAction(CharacterState.Dead);
                    MyCharacterProfile.ChangeCharacterState(CharacterState.Dead);
                }).AddTo(this);

            MyCharacterMove
                .OnMoveCompleted
                .Where(Completerd => Completerd == true)
                .Subscribe(_ =>
                {
                    MyCharacterProfile.ChangeCharacterState(CharacterState.Idle);
                })
                .AddTo(this);

            SetInputAuthority();
        }

        private async void SetInputAuthority()
        {
            await Task.Delay(100);

            MyCharacterProfile
                .OnCharacterStateChanged
                .Where(characterState => characterState != CharacterState.Dead)
                .Subscribe(StateAction)
                .AddTo(this);
        }
        
        /// <summary>
        /// 各状態の行動
        /// </summary>
        private async void StateAction(CharacterState State)//状態を変更する関数
        {
            if (Object.HasInputAuthority)
            {
                RPC_Render(State);
            }
            else
            {
                Debug.Log("RPC_Renderが動いていません。"+ Object.InputAuthority);
            }
            
            switch (State)
            {
                case CharacterState.Idle:
                    MyAnimator.SetBool("Move", false);
                    MyAnimator.SetBool("VigilancMove", false);
                    MyAnimator.SetTrigger("Idle");

                    if (!MyCharacterMove.isArrival())
                    {
                        MyCharacterProfile.ChangeCharacterState(CharacterState.Move);
                    }

                    await Task.Delay(1000);
                    if (MyCharacterProfile.GetCharacterOwnerType() != OwnerType.Player)
                    {
                        MyCharacterProfile.ChangeCharacterState(CharacterState.VigilanceMove);
                        MyCharacterMove.RandomTargetPosition();
                        MyCharacterMove.MoveTarget();
                    }
                    break;
                case CharacterState.Attack:
                    MyAnimator.SetBool("Move", false);
                    MyCharacterMove.StopMove(true);
                    MyAnimator.SetTrigger("Attack");
                    break;
                case CharacterState.Move:
                    MyAnimator.SetBool("Move", true);
                    MyCharacterMove.StopMove(false);
                    break;
                case CharacterState.VigilanceMove:
                    MyAnimator.SetBool("VigilancMove", true);
                    MyCharacterMove.StopMove(false);
                    break;
                case CharacterState.Reload:
                    MyAnimator.SetTrigger("Reload");
                    MyCharacterMove.StopMove(true);
                    break;
                case CharacterState.Dead:
                    //if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player) DeadkLogManager.AddText(gameObject.name + "が死亡しました", transform.position);
                    MyAnimator.SetTrigger("Die");
                    await Task.Delay(3000);
                    Runner.Despawn(Object);
                    break;
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_Render(CharacterState State)
        {
            MyCharacterProfile.ChangeCharacterState(State);
        }
        /// <summary>
        /// キャラクターが発見された時の動作
        /// </summary>
        public async void Idiscovered(bool value)
        {
            Debug.Log($"{gameObject.name}は発見されました(bool = {value})");
            if(value == true)
            {
                iVisible.Value = true;
            }
            else
            {
                await Task.Delay(3000);
                iVisible.Value = true;
            }
        }
        /*
        public override void FixedUpdateNetwork()
        {
            if (MyCharacterProfile.GetCharacterState() == CharacterState.Dead)
            {
                if (life.Expired(Runner))
                {
                    Debug.Log("despawn");
                     Runner.Despawn(Object);
                }
                   
            }
        }
        */
    }
}