using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.AI;
using System.Collections;

namespace Unit
{
    public class CharacterSearch : MonoBehaviour
    {
        [SerializeField]
        CharacterProfile MyCharacterProfile;
        private LogManager AttackLogManager;
        private float attackRange;
        private float searchRange;
        private List<GameObject> targetObjects = new List<GameObject>();
        private List<GameObject> discoveredObjects = new List<GameObject>();
        private List<GameObject> searchAreaInObjects = new List<GameObject>();
        private GameObject TargetObject;
        private GameObject UnitTargetObject;
        private SphereCollider AttaclColleder;
        private bool attackWaiting = false;
        void Start()
        {
            AttackLogManager = FindObjectOfType<LogManager>();
            AttaclColleder = GetComponent<SphereCollider>();
            AttaclColleder.radius = MyCharacterProfile.MysearchRange;
            attackRange = MyCharacterProfile.MyattackRange;
            searchRange = MyCharacterProfile.MysearchRange;

            MyCharacterProfile.OnCharacterTargetObject
                .Where(x => x == null)
                .Subscribe(_ =>
                {
                    UnitTargetObject = null;
                    if (CanShotEnemy())
                    {
                        Debug.Log("敵発見！");
                        MyCharacterProfile.SetTarget(TargetObject);
                    }
                }).AddTo(this);

            MyCharacterProfile.OnCharacterTargetObject
                .Subscribe(x =>
                {
                    UnitTargetObject = x;
                }).AddTo(this);

            StartCoroutine(discoverTarget());

        }

        //対象のオブジェクトが視界範囲内に入った場合
        private void OnTriggerEnter(Collider EnterObject)
        {
            if (EnterObject.TryGetComponent(out CharacterProfile profile)  && EnterObject.CompareTag("Unit"))
            {
                if (MyCharacterProfile.GetCharacterOwnerType() != profile.GetCharacterOwnerType() 
                    || (MyCharacterProfile.local != profile.local))
                {
                    if (!searchAreaInObjects.Contains(EnterObject.gameObject))
                    {
                        searchAreaInObjects.Add(EnterObject.gameObject);
                    }
                    if (checkBush(null, EnterObject.gameObject))
                    {
                        EnterObject.GetComponent<CharacterStatus>().Idiscovered(true);
                        discoveredObjects.Add(EnterObject.gameObject);
                        if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
                        {
                            //AttackLogManager.AddText($"{transform.parent.name}が{EnterObject.name}を発見！", discoveredObjects[0].transform.position);
                        }
                        if (!attackWaiting)
                        {
                            StartCoroutine(AttackWait());
                        }
                    }
                }
            }
        }
        IEnumerator AttackWait()
        {
            attackWaiting = true;
            while (true)
            {
                yield return new WaitUntil(() => CanAttackState() && targetObjects.Count != 0);
                if (CanShotEnemy() && UnitTargetObject != TargetObject)
                {
                    UnitTargetObject = TargetObject;
                    MyCharacterProfile.SetTarget(TargetObject);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        //対象のオブジェクトが視界範囲外に出た場合に、発見したオブジェクトから
        private void OnTriggerExit(Collider ExitObject)
        {
            if(discoveredObjects.Contains(ExitObject.gameObject))
            {
                ExitObject.GetComponent<CharacterStatus>().Idiscovered(false);
                discoveredObjects.Remove(ExitObject.gameObject);
            }
            if (targetObjects.Count == 0)
            {
                if (attackWaiting)
                {
                    attackWaiting = false;
                    StopCoroutine(AttackWait());
                }
            }
        }

        IEnumerator discoverTarget()
        {
            while (true)
            {
                yield return new WaitUntil(() => searchAreaInObjects.Count > 0);
                searchAreaInObjects.RemoveAll(item => item == null);
                for (int i = 0; i < searchAreaInObjects.Count; i++)
                {
                    var item = searchAreaInObjects[i];
                    if (!discoveredObjects.Contains(item) && !targetObjects.Contains(item))
                    {
                        if (SearchRangeChack(item))
                        {
                            if (checkBush(null, item))
                            {
                                item.GetComponent<CharacterStatus>().Idiscovered(true);
                                discoveredObjects.Add(item.gameObject);
                                if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
                                {
                                    AttackLogManager.AddText($"{transform.parent.name}が{item.name}を発見！", discoveredObjects[0].transform.position);
                                }
                                if (!attackWaiting)
                                {
                                    StartCoroutine(AttackWait());
                                }
                            }
                        }
                    }
                    else
                    {
                        searchAreaInObjects.Remove(item);
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        //攻撃可能な敵がいるかどうか
        private bool CanShotEnemy()
        {
            if (!TargetNullCheck())                                                                     //攻撃対象がいない場合は攻撃をやめる                                  
            {
                return false;
            }
            SortTargetCharacter();
            attackRangeObject();//索敵範囲内のオブジェクトが攻撃範囲内かどうかを調べる
            if(checkBush(targetObjects, null) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// ブッシュが無いかどうか
        /// </summary>
        private bool checkBush(List<GameObject> checkObjects,GameObject monoTarget)
        {
            bool onlyCheck = false;
            if(checkObjects == null && monoTarget != null)
            {
                onlyCheck = true;
                List<GameObject> l = new List<GameObject>();
                l.Add(monoTarget);
                checkObjects = l;
            }
           
            for (int i = 0; i < checkObjects.Count; i++)
            {
                //障害物判定用のレイ
                var diff = checkObjects[i].transform.position - transform.position;
                var distance = diff.magnitude;
                var direction = diff.normalized;

                RaycastHit[] hits;
                hits = Physics.RaycastAll(transform.position + new Vector3(0, 3, 0), direction, distance + 1);
                if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
                    Debug.DrawRay(transform.position + new Vector3(0, 3, 0), direction * (distance + 1), Color.red, 5);            //デバック用のDrawRay
                else
                    Debug.DrawRay(transform.position + new Vector3(0, 2.5f, 0), direction * (distance + 1), Color.blue, 5);     //デバック用のDrawRay

                //順番を近い順に入れ替える
                for (int j = 0; j < hits.Length; j++)
                {
                    for (int k = j + 1; k < hits.Length; k++)
                    {
                        if (ObjectsDistance(hits[j].transform.gameObject) > ObjectsDistance(hits[k].transform.gameObject))
                        {
                            var temp = hits[j];
                            hits[j] = hits[k];
                            hits[k] = temp;
                        }
                    }
                }

                bool blockObject = false;                                                           //障害物があるか判断するためのboolを定義
                for (int m = 0; m < hits.Length; m++)
                {
                    if (hits[m].transform.gameObject.CompareTag("Bush"))
                    {
                        blockObject = true;                                                         //障害物がある場合はblockObjectをtrueにする。
                        if (blockObject) Debug.Log($"{hits[m].transform.gameObject.name}からブッシュを検知しました");
                    }
                }
                if (!blockObject)                                                                    //blockObjectがfalseの場合にオブジェクトをターゲットに設定してtrue値を返す。
                {
                    if(! onlyCheck) TargetObject = checkObjects[i];
                    return true;
                }
            }
            return false;
        }

        //ターゲットに格納されているキャラクターを近い順に並べ替える
        private void SortTargetCharacter()
        {
            DeleteTarget();//不要なキャラクターを削除
            if (targetObjects.Count == 0) return;           //ターゲットがいない場合は処理を終了

            //キャラクターを近い順に並べ替える
            for (int i = 0; i < targetObjects.Count; i++)
            {
                for (int j = i + 1; j < targetObjects.Count; j++)
                {
                    if (ObjectsDistance(targetObjects[i]) < ObjectsDistance(targetObjects[j]))
                    {
                        var temp = targetObjects[i];
                        targetObjects[i] = targetObjects[j];
                        targetObjects[j] = temp;
                    }
                }
            }
        }
        //空のオブジェクト、射程外の敵を削除
        private void DeleteTarget()
        {
            targetObjects.RemoveAll(item => item == null);
            discoveredObjects.RemoveAll(item => item == null);

            //targetObjects様の処理
            if (targetObjects.Count != 0)
            {
                for (int i = 0; i < targetObjects.Count; i++)
                {
                    if (!AttackRangeChack(targetObjects[i]))
                    {
                        targetObjects.Remove(targetObjects[i]);
                    }
                }
            }
            //discoveredObjects様の処理
            if (discoveredObjects.Count != 0)
            {
                for (int i = 0; i < discoveredObjects.Count; i++)
                {
                    if (!SearchRangeChack(discoveredObjects[i]))
                    {
                        discoveredObjects.Remove(discoveredObjects[i]);
                    }
                }
            }
        }

        private bool TargetNullCheck()//目標がいるかどうか
        {
            if (targetObjects.Count == 0) return false;
            else if (targetObjects[0] == null) DeleteTarget();
            return true;
        }
        //対象が射程範囲内かどうか
        private bool AttackRangeChack(GameObject TargetGameobject)
        {
            if (attackRange > ObjectsDistance(TargetGameobject)) return true;
            else return false;
        }
        //対象が索敵範囲内かどうか
        private bool SearchRangeChack(GameObject TargetGameobject)
        {
            if ((searchRange + 3) > ObjectsDistance(TargetGameobject)) return true;
            else return false;
        }

        //二つのオブジェクトの距離
        private float ObjectsDistance(GameObject TagetObject)
        {
            return Vector3.Distance(TagetObject.transform.position, transform.position);
        }

        private void attackRangeObject()
        {
            for (int i = 0; i < discoveredObjects.Count; i++)
            {
                if (attackRange > ObjectsDistance(discoveredObjects[i]))
                {
                    if (!targetObjects.Contains(discoveredObjects[i]))
                    {
                        targetObjects.Add(discoveredObjects[i]);
                    }
                }
            }
        }

        //characterが戦闘できる状態かどうか
        private bool CanAttackState()
        {
            if (MyCharacterProfile.GetCharacterState() == CharacterState.VigilanceMove
                || MyCharacterProfile.GetCharacterState() == CharacterState.Idle
                || MyCharacterProfile.GetCharacterState() == CharacterState.Attack
                || MyCharacterProfile.GetCharacterState() == CharacterState.Reload
                || MyCharacterProfile.MyMoveHitRate != 0)
            {
                if (discoveredObjects.Count != 0)
                {
                    DeleteTarget();
                    attackRangeObject();//索敵範囲内のオブジェクトが攻撃範囲内かどうかを調べる
                }
                return true;
            }
            else return false;
        }
    }
}