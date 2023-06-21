using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Fusion;

namespace Unit
{
    public class CharacterMove : CharacterStateAction

    {
        private ReactiveProperty<bool> moveCompleted = new ReactiveProperty<bool>(false);
        //[Networked]
        private ReactiveProperty<Vector3> moveTargetPosition { get; set; } = new ReactiveProperty<Vector3>();
        private NavMeshAgent Agent;
        CharacterProfile MyCharacterProfile;
        private Camera mainCamera;
        private ReactiveProperty<bool> IsSelect = new ReactiveProperty<bool>(false);

        public IObservable<Vector3> OnMoveTargetPositionChanged
        {
            get { return moveTargetPosition; }
        }
        public IObservable<bool> OnMoveCompleted//characterHPが変更された際に発光されるイベント
        {
            get { return moveCompleted; }
        }
        public IObservable<bool> isSelect//characterHPが変更された際に発光されるイベント
        {
            get { return IsSelect; }
        }
        public void iSelect(bool value)
        {
            IsSelect.Value = value;
            if (IsSelect.Value)
            {
                StateAction();
            }
        }
        public bool isArrival()
        {
            return moveCompleted.Value;
        }

        public override void Spawned()
        {
            base.Spawned();
            mainCamera = Camera.main;
            MyCharacterProfile = GetComponent<CharacterProfile>();
            Agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTarget()
        {
            Agent.destination = moveTargetPosition.Value;
            StopCoroutine(CompletedMove());
            StartCoroutine(CompletedMove());
        }


        public void RandomTargetPosition()
        {
            moveTargetPosition.Value = transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10), 0, UnityEngine.Random.Range(-10f, 10));
        }

        IEnumerator CompletedMove()
        {
            moveCompleted.Value = false;
            yield return new WaitUntil(() => CharacterToTragetDistance() < 1);
            moveCompleted.Value = true;
        }

        private float CharacterToTragetDistance()//キャラクターと目標地点までの距離
        {
            var Dinstance = Vector3.Distance(transform.position, moveTargetPosition.Value);
            return Dinstance;
        }

        public void StopMove(bool value)//移動停止
        {
            Agent.isStopped = value;
        }
        IEnumerator Move()
        {
            if (Object.HasInputAuthority)
            {
                yield return new WaitUntil(() => IsSelect.Value && (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse0)));
                if (Input.GetKey(KeyCode.Q)) MyCharacterProfile.ChangeCharacterState(CharacterState.VigilanceMove);
                else  MyCharacterProfile.ChangeCharacterState(CharacterState.Move);
    
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit);
                    Debug.Log("Move()");
                    RPC_MoveChara(hit.point);
        
                    //moveTargetPosition.Value = new Vector3(MovePoint.x, transform.position.y, MovePoint.z);
                    //MoveTarget();
                }
            }
            yield break;
        }

        public override void StartStateAction()
        {

        }

        public override void StateAction()
        {
            StopCoroutine(Move());
            StartCoroutine(Move());
        }

        public override void EndStateAction()
        {

        }

        /*この関数使わなくてもネットワーク上で動きますが、今後使うかもしれないので一応残しておきます*/
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_MoveChara(Vector3 movePosition , RpcInfo info = default)
        {
            moveTargetPosition.Value = movePosition;
            Debug.Log($"setMoveTargetPosition:moveTargetPosition.Value = {moveTargetPosition.Value} : movePosition = {movePosition}");
            MoveTarget();
        }
    }
}