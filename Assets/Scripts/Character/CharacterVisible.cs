using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Fusion;

namespace Unit
{
    public class UnitSearchElement
    {
        public GameObject UnitObject;
        public float SearchRange;
    }

    public class CharacterVisible : NetworkBehaviour
    {
        [Header("基本データの参照")]
        [SerializeField]
        private CharacterProfile MyCharacterProfile;
        [SerializeField]
        private GameObject[] VisibleObjects;
        private CharacterStatus MyStatus;

        
        private List<UnitSearchElement> allEnemy = new List<UnitSearchElement>();
        private List<UnitSearchElement> undiscoveredEnemy = new List<UnitSearchElement>();
        private List<UnitSearchElement> discoveredEnemy = new List<UnitSearchElement>();
        private bool canLoopAction = false;
        void Start()
        {
            MyCharacterProfile
                .OninitialSetting
                .Where(value => value == true)
                .Subscribe(_ =>
                {
                    if (!MyCharacterProfile.isHasInputAuthority()) Debug.Log("CharacterVisible:初期値の設定がされました");
                    StartCoroutine(Init());
                })
                .AddTo(this);

            MyStatus = GetComponent<CharacterStatus>();         
        }

        IEnumerator Init()
        {
            if (MyCharacterProfile.isHasStateAuthority())
                StartCoroutine(setUndiscoveredEnemy());
                StartCoroutine(SearchSystemloop());
                yield return new WaitForSeconds(2f);
            if (HasInputAuthority)
            {
                Visible(true);
            }
        }

        public void Visible(bool value)
        {
            if(HasInputAuthority)
            {
                //Debug.Log($"HasInputAuthority : Visible{value}");
            }
            else
            {
                foreach (var Objects in VisibleObjects)
                {
                    Objects.SetActive(value);
                }
                //Debug.Log($"NotHasInputAuthority : Visible{value}");
            }
        }

        /// <summary>
        /// allEnemyの敵に対して索敵範囲内かの確認、索敵範囲内の場合は未発見の敵（undiscoveredEnemy）に格納
        /// </summary>
        IEnumerator SearchSystemloop()
        {
            while (true)
            {
                yield return new WaitUntil(() => canLoopAction);
                //未発見の敵の発見
                foreach (var item in allEnemy)
                {
                    if (!undiscoveredEnemy.Contains(item) && item.UnitObject != null)
                    {
                        if (Vector3.Distance(transform.position, item.UnitObject.transform.position) <= item.SearchRange)
                        {
                            //Debug.Log($"索敵範囲内だったため、undiscoveredEnemyに{item}を追加：{Vector3.Distance(transform.position, item.UnitObject.transform.position) } =< {item.SearchRange} ");
                            undiscoveredEnemy.Add(item);
                        }
                    }
                }

                //未発見の敵を発見できるかの確認
                foreach (var item in undiscoveredEnemy)
                {
                    if (!discoveredEnemy.Contains(item))
                    {
                        if (checkSightPass(item.UnitObject) && item.UnitObject != null)
                        {
                            discoveredEnemy.Add(item);

                            if (MyCharacterProfile.isHasInputAuthority())//logに発見したことを通知
                            {
                                //AttackLogManager.AddText($"{transform.parent.name}が{EnterObject.name}を発見！", discoveredObjects[0].transform.position);
                            }
                        }
                    }
                }


                if(discoveredEnemy.Count != 0)
                {
                    //Debug.Log("MyStatus.Idiscovered(true)");
                        MyStatus.Idiscovered(true);//発見した場合は相手を表示
                }
                else
                {
                    //Debug.Log("MyStatus.Idiscovered(false)");
                    MyStatus.Idiscovered(false);//発見した場合は相手を表示
                }

                yield return new WaitForSeconds(0.2f);
                //Debug.Log($"CharacterVisibleの各値について。allEnemy = {allEnemy.Count} :undiscoveredEnemy = {undiscoveredEnemy.Count}: discoveredEnemy = {discoveredEnemy.Count}");
                //Listの整理
                resetList();
            }

        }

        /// <summary>
        /// 未発見の敵全体を取得
        /// </summary>
        IEnumerator setUndiscoveredEnemy()
        {
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                var allEnemys = GameObject.FindGameObjectsWithTag("Unit");
                foreach (var item in allEnemys)
                {
                    var tmpUnit = new UnitSearchElement();
                    tmpUnit.UnitObject = item;
                    tmpUnit.SearchRange = item.GetComponent<CharacterProfile>().MysearchRange;
                    if (!item.GetComponent<CharacterProfile>().isHasInputAuthority() && MyCharacterProfile.isHasInputAuthority())
                    {
                        if (!allEnemy.Contains(tmpUnit))
                        {
                            allEnemy.Add(tmpUnit);
                        }
                    }
                    else if (item.GetComponent<CharacterProfile>().isHasInputAuthority() && !MyCharacterProfile.isHasInputAuthority())
                    {
                        if (!allEnemy.Contains(tmpUnit))
                        {
                            allEnemy.Add(tmpUnit);
                        }
                    }
                }

                
                if (allEnemy.Count == 5)
                {
                    canLoopAction = true;
                    //Debug.Log("敵の数がそろっているので索敵を開始します。");
                    yield return new WaitUntil(() => allEnemy.Count != 5);
                    yield return new WaitForSeconds(5f);
                }
                else
                {
                    canLoopAction = false;
                    //Debug.Log("敵の数がそろっていないので索敵処理を停止します。");
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
        /// <summary>
        /// 対象を視認できるかどうか。壁およびブッシュの計算
        /// </summary>
        private bool checkSightPass(GameObject checkTarget)
        {
            if (checkTarget == null) return false;
            //障害物判定用のrayの数値
            var diff = transform.position - checkTarget.transform.position;
            var distance = diff.magnitude;
            var direction = diff.normalized;


            RaycastHit[] hits;
            hits = Physics.RaycastAll(checkTarget.transform.position + new Vector3(0, 3, 0), direction, distance + 1);
            Debug.DrawRay(checkTarget.transform.position + new Vector3(0, 3, 0), direction * (distance + 1), Color.red, 1);            //デバック用のDrawRay

            //ステージの障害物に祭儀られている場合はfalseを返す
            foreach (var item in hits)
            {
                if (item.transform.CompareTag("Stage"))
                {
                    //if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("対象との間にStageのオブジェクトを検知したため視界が通りませんでした");
                    return false;
                }
            }

            //取得したrayを近い順番通りに並べる
            for (int j = 0; j < hits.Length; j++)
            {
                for (int k = j + 1; k < hits.Length; k++)

                {
                    if (Vector3.Distance(transform.position, hits[j].transform.position)
                        > Vector3.Distance(transform.position, hits[k].transform.position))
                    {
                        var temp = hits[j];
                        hits[j] = hits[k];
                        hits[k] = temp;
                    }
                }
            }

            if (hits.Length >= 2)
            {
                //ブッシュについての確認
                for (int i = 0; i < hits.Length - 1; i++)
                {
                    if (hits[i].transform.CompareTag("Bush"))
                    {
                        //ブッシュから相手までの距離が30以下だった場合は視界が通らずfalseを返す
                        if (Vector3.Distance(transform.position, hits[hits.Length].transform.position)
                            - Vector3.Distance(transform.position, hits[i].transform.position)
                            >= 30)
                        {
                            //if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("対象との間にBushのオブジェクトを検知し距離が離れていたため視界が通りませんでした:" +
                             //   (Vector3.Distance(transform.position, hits[hits.Length].transform.position) - Vector3.Distance(transform.position, hits[i].transform.position)));
                            return false;
                        }
                    }
                }
            }

            return true;

        }
        /// <summary>
        /// すべてのListについてnullのものをListから削除
        /// </summary>
        private void resetList()
        {
            List<UnitSearchElement> tmpList = new List<UnitSearchElement>();//削除する項目を一時的に保存

            //視界が通っていない場合はdiscoveredEnemyから削除する
            foreach (var item in discoveredEnemy)
            {
                if (item.UnitObject == null)
                {
                    tmpList.Add(item);
                }
                else if (!checkSightPass(item.UnitObject))
                {
                    tmpList.Add(item);
                }
            }

            foreach (var item in tmpList)//保存した項目に従って削除
            {
                discoveredEnemy.Remove(item);//保存した項目に従って削除
            }
            tmpList.Clear();

            //視界外の場合はundiscoveredEnemyから削除する
            foreach (var item in undiscoveredEnemy)
            {
                if (item.UnitObject == null)
                {
                    tmpList.Add(item);
                }
                else if (Vector3.Distance(transform.position, item.UnitObject.transform.position) >= item.SearchRange)
                {
                    tmpList.Add(item);
                }
            }

            foreach (var item in tmpList)//保存した項目に従って削除
            {
                discoveredEnemy.Remove(item);//保存した項目に従って削除

            }
            tmpList.Clear();

            //allEnemyでnullを削除する
            allEnemy.RemoveAll(s => s == null);
        }
    }
}