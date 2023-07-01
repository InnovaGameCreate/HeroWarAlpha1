using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace Unit
{
    public class CharacterSearch : MonoBehaviour
    {
        [Header("参照元")]
        [SerializeField]
        CharacterProfile MyCharacterProfile;
        private LogManager AttackLogManager;

        private bool canLoopAction = false;//ループ処理が可能かどうか


        private float attackRange;
        private List<GameObject> allEnemy = new List<GameObject>();
        private List<GameObject> inAttackRangeEnemy = new List<GameObject>();
        private GameObject attackTargetObject = null;
        private GameObject lastAttackTargetObject = null;
        private bool isInit = false;
        private void Awake()
        {
            AttackLogManager = FindObjectOfType<LogManager>();
            MyCharacterProfile
                .OninitialSetting
                .Where(value => value == true)
                .Subscribe(_ =>
                {
                    if (!isInit)
                    {
                        Debug.Log("CharacterSearch:初期値の設定がされました");
                        Init();
                    }
                })
                .AddTo(this);
        }
        /// <summary>
        /// allEnemyの敵に対して索敵範囲内かの確認、索敵範囲内の場合は未発見の敵（undiscoveredEnemy）に格納
        /// </summary>
        IEnumerator SearchSystemloop()
        {
            while (true)
            {
                yield return new WaitUntil(() => canLoopAction);

                //敵が攻撃範囲かどうかの確認
                foreach (var item in allEnemy)
                {
                    if (!inAttackRangeEnemy.Contains(item) && item != null)
                    {
                        if (checkSightPass(item) && Vector3.Distance(transform.position, item.transform.position) <= attackRange && item.GetComponent<CharacterProfile>().MyHp > 0)
                        {
                            if (MyCharacterProfile.isHasInputAuthority()) Debug.Log($"攻撃範囲内だったため、undiscoveredEnemyに{item}を追加：{Vector3.Distance(transform.position, item.transform.position) } =< {attackRange} ");
                            inAttackRangeEnemy.Add(item);
                        }
                    }
                }

                //攻撃対象の設定
                if(inAttackRangeEnemy.Count != 0)
                {
                    attackTargetObject = getClosestEnemy();
                    if (attackTargetObject != lastAttackTargetObject)
                    {
                        if (MyCharacterProfile.isHasInputAuthority()) Debug.Log($"攻撃範囲内だったため、SetTargetを{attackTargetObject}を追加：");
                        MyCharacterProfile.SetTarget(attackTargetObject);
                    }
                    lastAttackTargetObject = attackTargetObject;
                }
                else
                {
                    if (lastAttackTargetObject != null)
                    {
                        Debug.Log("攻撃対象をnullに設定");
                        lastAttackTargetObject = null;
                        MyCharacterProfile.SetTarget(null);
                    }
                }

                yield return new WaitForSeconds(0.2f);
                if (MyCharacterProfile.isHasInputAuthority()) Debug.Log($"CharacterSearchの各値について。allEnemy = {allEnemy.Count} : inAttackRangeEnemy = {inAttackRangeEnemy.Count}");
                //Listの整理
                resetList();
            }
        }

        /// <summary>
        /// 初期値の入力
        /// </summary>
        void Init()
        {
            isInit = true;
            attackRange = MyCharacterProfile.MyattackRange;
            StartCoroutine(setUndiscoveredEnemy());
            StartCoroutine(SearchSystemloop());
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
                    if (!item.GetComponent<CharacterProfile>().isHasInputAuthority() && MyCharacterProfile.isHasInputAuthority())
                    {
                        if (!allEnemy.Contains(item)) allEnemy.Add(item);
                    }
                    else if(item.GetComponent<CharacterProfile>().isHasInputAuthority() && !MyCharacterProfile.isHasInputAuthority())
                    {

                    }
                }

                if (allEnemy.Count == 5)
                {
                    canLoopAction = true;
                    if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("敵の数がそろっているので索敵を開始します。");
                    yield return new WaitUntil(() => allEnemy.Count != 5);
                    yield return new WaitForSeconds(5f);
                }
                else
                {
                    canLoopAction = false;
                    if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("敵の数がそろっていないので索敵処理を停止します。");
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
            var diff = checkTarget.transform.position - transform.position;
            var distance = diff.magnitude;
            var direction = diff.normalized;


            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position + new Vector3(0, 3, 0), direction, distance + 1);
            Debug.DrawRay(transform.position + new Vector3(0, 3, 0), direction * (distance + 1), Color.red, 1);            //デバック用のDrawRay

            //ステージの障害物に祭儀られている場合はfalseを返す
            foreach (var item in hits)
            {
                if(item.transform.CompareTag("Stage"))
                {
                    if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("対象との間にStageのオブジェクトを検知したため視界が通りませんでした");
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
                            if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("対象との間にBushのオブジェクトを検知し距離が離れていたため視界が通りませんでした:" +
                                (Vector3.Distance(transform.position, hits[hits.Length].transform.position) - Vector3.Distance(transform.position, hits[i].transform.position)));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 最も近い敵に取得
        /// </summary>
        GameObject getClosestEnemy()
        {
            if (inAttackRangeEnemy.Count >= 2)
            {
                //取得したrayを近い順番通りに並べる
                for (int j = 0; j < inAttackRangeEnemy.Count; j++)
                {
                    for (int k = j + 1; k < inAttackRangeEnemy.Count; k++)
                    {
                        if (Vector3.Distance(transform.position, inAttackRangeEnemy[j].transform.position)
                            > Vector3.Distance(transform.position, inAttackRangeEnemy[k].transform.position))
                        {
                            var temp = inAttackRangeEnemy[j];
                            inAttackRangeEnemy[j] = inAttackRangeEnemy[k];
                            inAttackRangeEnemy[k] = temp;
                        }
                    }
                }
            }


            //debug用のray
            var diff = inAttackRangeEnemy[0].transform.position - transform.position;
            var distance = diff.magnitude;
            var direction = diff.normalized;
            Debug.DrawRay(transform.position + new Vector3(0, 4, 0), direction * (distance + 1), Color.blue, 1);            //デバック用のDrawRay

            return inAttackRangeEnemy[0];
        }

        /// <summary>
        /// すべてのListについてnullのものをListから削除
        /// </summary>
        private void resetList()
        {
            List<GameObject> tmpList = new List<GameObject>();//削除する項目を一時的に保存

            //敵が攻撃範囲ではない場合は攻撃範囲の敵から削除する
            foreach (var item in inAttackRangeEnemy)
            {
                if(item == null)
                {
                    tmpList.Add(item);
                }
                else if (Vector3.Distance(transform.position, item.transform.position) >= attackRange || item.GetComponent<CharacterProfile>().MyHp <= 0)
                {
                    tmpList.Add(item);
                }
            }

            foreach (var item in tmpList)//保存した項目に従って削除
            {
                inAttackRangeEnemy.Remove(item);
            }
            tmpList.Clear();

            //allEnemyでnullを削除する
            allEnemy.RemoveAll(s => s == null);
        }
    }
}
