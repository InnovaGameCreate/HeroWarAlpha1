using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;
using Fusion;

namespace Unit
{
    public class CharacterProfile : NetworkBehaviour, IDamagable
    {
        NavMeshAgent agent;
        private ReactiveProperty<float> characterHP = new ReactiveProperty<float>(10f);//キャラクターの体力
        [SerializeField]
        private OwnerType CharacterOwnerType;
        //[Networked] 
        private ReactiveProperty<CharacterState> CharacterState { get; set; } = new ReactiveProperty<CharacterState>(global::CharacterState.Idle);//キャラクターの現在の状態
        private ReactiveProperty<GameObject> TargetObject = new ReactiveProperty<GameObject>();//攻撃対象をオブジェクト
        private ReactiveProperty<bool> initialSetting = new ReactiveProperty<bool>(false);//初期値の設定が行われているかどうか

        private float attackPower;
        private int recoverySpeed;
        private float moveSpeed;
        private float attackRange;
        private float armor;
        private float staticHitRate;
        private float moveHitRate;
        private float reloadSpeed;
        private float searchRange;
        private UnitType unitType;

        [Networked]
        public PlayerRef local { get;set; }

        public IObservable<bool> OninitialSetting//数値の初期設定を行ったかどうかを通知する
        {
            get { return initialSetting; }
        }

        public IObservable<float> OncharacterHPChanged//characterHPの変化があったときに発行される
        {
            get { return characterHP; }
        }
        public IObservable<CharacterState> OnCharacterStateChanged//CharacterStateの変化があったときに発行される
        {
            get { return CharacterState; }
        }
        public IObservable<GameObject> OnCharacterTargetObject//TargetObjectの変化があったときに発行される
        {
            get { return TargetObject; }
        }

        public float MyAttackPower { get => attackPower; }
        public int MyRecoverySpeed { get => recoverySpeed; }
        public float MyMoveSpeed { get => moveSpeed; }
        public float MyattackRange { get => attackRange; }
        public float MyArmor { get => armor; }
        public float MyStaticHitRate { get => staticHitRate; }
        public float MyMoveHitRate { get => moveHitRate; }
        public float MyReloadSpeed { get => reloadSpeed; }
        public float MysearchRange { get => searchRange; }
        public float MyHp { get => characterHP.Value; }
        public UnitType MyUnitType { get => unitType; }


        private void FixedUpdate()
        {
            Debug.Log($"CharacterState:{CharacterState}");
        }
        private void Awake()
        {
            initialSetting.Value = false;
        }
        /// <summary>
        /// キャラクターの所有者を取得
        /// </summary>
        public OwnerType GetCharacterOwnerType()
        {
            return CharacterOwnerType;
        }

        public void SetCharacterOwnerType(OwnerType ownerType)
        {
            CharacterOwnerType = ownerType;
        }

        public bool isHasInputAuthority()
        {
            bool value = false;
            if (HasInputAuthority) value = true;
            return value;
        }
        public bool isHasStateAuthority()
        {
            bool value = false;
            if (HasStateAuthority) value = true;
            return value;
        }

        /// <summary>
        /// キャラクターの状態を取得
        /// </summary>
        public CharacterState GetCharacterState()
        {
            return CharacterState.Value;
        }
        /// <summary>
        /// キャラクターの状態を変更
        /// </summary>
        public void ChangeCharacterState(CharacterState State)
        {
            CharacterState.Value = State;
        }


        /// <summary>
        /// 攻撃対象を設定
        /// </summary>
        public void SetTarget(GameObject newTarget)
        {
            if (GetCharacterState() != global::CharacterState.Dead)
            {
                TargetObject.Value = newTarget;
                if (newTarget != null) Debug.Log($"{newTarget.name } を発見");
            }
            else
            {
                Debug.LogError("キャラクターが既に死亡しています");
            }
        }

        /// <summary>
        /// キャラクターにダメージを与える
        /// </summary>
        public void AddDamage(float damage)

        {
            Debug.Log("ダメージ：" + damage);
            float Penetrationdamage = armor - damage;            
            if (Penetrationdamage < 0)
            {
                if (characterHP.Value > 0)
                {
                    characterHP.Value += Penetrationdamage;   
                }
            }
        }

        /// <summary>
        /// 自分の体力の変更
        /// </summary>
        public void ChangeHPValue(float value)
        {
            if (characterHP.Value != value)
            {
                characterHP.Value = value;
            }
        }

        /// <summary>
        /// キャラクターの初期値を設定
        /// </summary>
        public void Init(CharacterData Data)
        {
            attackPower = Data.attackPower;
            recoverySpeed = Data.recoverySpeed;
            moveSpeed = Data.moveSpeed;
            attackRange = Data.attackRange;
            armor = Data.armor;
            staticHitRate = Data.staticHitRate;
            moveHitRate = Data.moveHitRate;
            reloadSpeed = Data.reloadSpeed;
            searchRange = Data.searchRange;
            characterHP.Value = Data.hp;
            unitType = Data.UnitType;

            agent = GetComponent<NavMeshAgent>();
            agent.speed = Data.moveSpeed;

            initialSetting.Value = true;
            Debug.Log(attackPower);
        }
    }
}