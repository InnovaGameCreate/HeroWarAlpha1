using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;
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
        public IObservable<bool> OnMoveCompleted//moveCompletedが変更されたときに送られるイベント
        {
            get { return moveCompleted; }
        }
        public IObservable<bool> isSelect//IsSelectが変更されたときに送られるイベント
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
            else
            {
                StopCoroutine(Move());
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
            yield return new WaitUntil(() => CharacterToTragetDistance() < 2);
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
                while (true)
                {

                    yield return new WaitUntil(() => IsSelect.Value && (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse0)));
                    if (Input.GetKey(KeyCode.Q))
                    {
                        MyCharacterProfile.ChangeCharacterState(CharacterState.VigilanceMove);
                        Debug.Log("VigilanceMove()");
                    }
                    else
                    {
                        MyCharacterProfile.ChangeCharacterState(CharacterState.Move);
                        Debug.Log("Move()");
                    }

                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        Vector3 mousePosition = Input.mousePosition;
                        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                        RaycastHit hit;
                        Physics.Raycast(ray, out hit);
                        RPC_MoveChara(hit.point);

                        //moveTargetPosition.Value = new Vector3(MovePoint.x, transform.position.y, MovePoint.z);

                    } //MoveTarget();

                    yield return new WaitForSeconds(0.5f);
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
            //Debug.Log($"setMoveTargetPosition:moveTargetPosition.Value = {moveTargetPosition.Value} : movePosition = {movePosition}");
            MoveTarget();
        }
    }
}