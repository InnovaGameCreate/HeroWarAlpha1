using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;
using Fusion;
using TMPro;
using UnityEngine.UI;

namespace Unit
{
    public class CharacterProfile : NetworkBehaviour, IDamagable
    {
        NavMeshAgent agent;
        private ReactiveProperty<float> characterHP = new ReactiveProperty<float>(10f);//�L�����N�^�[�̗̑�
        [SerializeField]
        private OwnerType CharacterOwnerType;
        //[Networked] 
        private ReactiveProperty<CharacterState> CharacterState { get; set; } = new ReactiveProperty<CharacterState>();//�L�����N�^�[�̏��
        private ReactiveProperty<GameObject> TargetObject = new ReactiveProperty<GameObject>(null);//�L�����N�^�[�̏��
        
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


        public IObservable<float> OncharacterHPChanged//characterHP���ύX���ꂽ�ۂɔ��s�����C�x���g
        {
            get { return characterHP; }
        }
        public IObservable<CharacterState> OnCharacterStateChanged//CharacterState���ύX���ꂽ�ۂɔ��s�����C�x���g
        {
            get { return CharacterState; }
        }
        public IObservable<GameObject> OnCharacterTargetObject//CharacterState���ύX���ꂽ�ۂɔ��s�����C�x���g
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

        /// <summary>
        /// キャラクターの状態を取得
        /// </summary>
        public CharacterState GetCharacterState()//�L�����N�^�[���N�ɋA�����邩��Q�Ƃ��邽��
        {
            return CharacterState.Value;
        }
        /// <summary>
        /// キャラクターの状態を変更
        /// </summary>
        public void ChangeCharacterState(CharacterState State)
        {
            CharacterState.Value = State;
            //if (CharacterOwnerType == OwnerType.Player) Debug.Log("Change CharacterState = " + State.ToString());
        }


        /// <summary>
        /// 攻撃対象を設定
        /// </summary>
        public void SetTarget(GameObject newTarget)
        {
            if(GetCharacterState() != global::CharacterState.Dead)
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
        /// <param name="damage"></param>
        public void AddDamage(float damage)

        {
            Debug.Log("ダメージ：" + damage);
            //damage = 0.1f;
            float Penetrationdamage = armor - damage;            
            if (Penetrationdamage < 0)
            {
                if (characterHP.Value > 0)
                {
                    characterHP.Value += Penetrationdamage;   
                }
            }
            Debug.Log("残り体力：" + characterHP.Value);
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
            
            Debug.Log(attackPower);
        }
    }
}