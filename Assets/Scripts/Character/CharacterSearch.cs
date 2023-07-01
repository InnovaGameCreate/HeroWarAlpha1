using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace Unit
{
    public class CharacterSearch : MonoBehaviour
    {
        [Header("�Q�ƌ�")]
        [SerializeField]
        CharacterProfile MyCharacterProfile;
        private LogManager AttackLogManager;

        private bool canLoopAction = false;//���[�v�������\���ǂ���


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
                        Debug.Log("CharacterSearch:�����l�̐ݒ肪����܂���");
                        Init();
                    }
                })
                .AddTo(this);
        }
        /// <summary>
        /// allEnemy�̓G�ɑ΂��č��G�͈͓����̊m�F�A���G�͈͓��̏ꍇ�͖������̓G�iundiscoveredEnemy�j�Ɋi�[
        /// </summary>
        IEnumerator SearchSystemloop()
        {
            while (true)
            {
                yield return new WaitUntil(() => canLoopAction);

                //�G���U���͈͂��ǂ����̊m�F
                foreach (var item in allEnemy)
                {
                    if (!inAttackRangeEnemy.Contains(item) && item != null)
                    {
                        if (checkSightPass(item) && Vector3.Distance(transform.position, item.transform.position) <= attackRange && item.GetComponent<CharacterProfile>().MyHp > 0)
                        {
                            if (MyCharacterProfile.isHasInputAuthority()) Debug.Log($"�U���͈͓����������߁AundiscoveredEnemy��{item}��ǉ��F{Vector3.Distance(transform.position, item.transform.position) } =< {attackRange} ");
                            inAttackRangeEnemy.Add(item);
                        }
                    }
                }

                //�U���Ώۂ̐ݒ�
                if(inAttackRangeEnemy.Count != 0)
                {
                    attackTargetObject = getClosestEnemy();
                    if (attackTargetObject != lastAttackTargetObject)
                    {
                        if (MyCharacterProfile.isHasInputAuthority()) Debug.Log($"�U���͈͓����������߁ASetTarget��{attackTargetObject}��ǉ��F");
                        MyCharacterProfile.SetTarget(attackTargetObject);
                    }
                    lastAttackTargetObject = attackTargetObject;
                }
                else
                {
                    if (lastAttackTargetObject != null)
                    {
                        Debug.Log("�U���Ώۂ�null�ɐݒ�");
                        lastAttackTargetObject = null;
                        MyCharacterProfile.SetTarget(null);
                    }
                }

                yield return new WaitForSeconds(0.2f);
                if (MyCharacterProfile.isHasInputAuthority()) Debug.Log($"CharacterSearch�̊e�l�ɂ��āBallEnemy = {allEnemy.Count} : inAttackRangeEnemy = {inAttackRangeEnemy.Count}");
                //List�̐���
                resetList();
            }
        }

        /// <summary>
        /// �����l�̓���
        /// </summary>
        void Init()
        {
            isInit = true;
            attackRange = MyCharacterProfile.MyattackRange;
            StartCoroutine(setUndiscoveredEnemy());
            StartCoroutine(SearchSystemloop());
        }

        /// <summary>
        /// �������̓G�S�̂��擾
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
                    if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("�G�̐���������Ă���̂ō��G���J�n���܂��B");
                    yield return new WaitUntil(() => allEnemy.Count != 5);
                    yield return new WaitForSeconds(5f);
                }
                else
                {
                    canLoopAction = false;
                    if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("�G�̐���������Ă��Ȃ��̂ō��G�������~���܂��B");
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        /// <summary>
        /// �Ώۂ����F�ł��邩�ǂ����B�ǂ���уu�b�V���̌v�Z
        /// </summary>
        private bool checkSightPass(GameObject checkTarget)
        {
            if (checkTarget == null) return false;
            //��Q������p��ray�̐��l
            var diff = checkTarget.transform.position - transform.position;
            var distance = diff.magnitude;
            var direction = diff.normalized;


            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position + new Vector3(0, 3, 0), direction, distance + 1);
            Debug.DrawRay(transform.position + new Vector3(0, 3, 0), direction * (distance + 1), Color.red, 1);            //�f�o�b�N�p��DrawRay

            //�X�e�[�W�̏�Q���ɍՋV���Ă���ꍇ��false��Ԃ�
            foreach (var item in hits)
            {
                if(item.transform.CompareTag("Stage"))
                {
                    if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("�ΏۂƂ̊Ԃ�Stage�̃I�u�W�F�N�g�����m�������ߎ��E���ʂ�܂���ł���");
                    return false;
                }
            }

            //�擾����ray���߂����Ԓʂ�ɕ��ׂ�
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
                //�u�b�V���ɂ��Ă̊m�F
                for (int i = 0; i < hits.Length - 1; i++)
                {
                    if (hits[i].transform.CompareTag("Bush"))
                    {
                        //�u�b�V�����瑊��܂ł̋�����30�ȉ��������ꍇ�͎��E���ʂ炸false��Ԃ�
                        if (Vector3.Distance(transform.position, hits[hits.Length].transform.position)
                            - Vector3.Distance(transform.position, hits[i].transform.position)
                            >= 30)
                        {
                            if (MyCharacterProfile.isHasInputAuthority()) Debug.Log("�ΏۂƂ̊Ԃ�Bush�̃I�u�W�F�N�g�����m������������Ă������ߎ��E���ʂ�܂���ł���:" +
                                (Vector3.Distance(transform.position, hits[hits.Length].transform.position) - Vector3.Distance(transform.position, hits[i].transform.position)));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �ł��߂��G�Ɏ擾
        /// </summary>
        GameObject getClosestEnemy()
        {
            if (inAttackRangeEnemy.Count >= 2)
            {
                //�擾����ray���߂����Ԓʂ�ɕ��ׂ�
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


            //debug�p��ray
            var diff = inAttackRangeEnemy[0].transform.position - transform.position;
            var distance = diff.magnitude;
            var direction = diff.normalized;
            Debug.DrawRay(transform.position + new Vector3(0, 4, 0), direction * (distance + 1), Color.blue, 1);            //�f�o�b�N�p��DrawRay

            return inAttackRangeEnemy[0];
        }

        /// <summary>
        /// ���ׂĂ�List�ɂ���null�̂��̂�List����폜
        /// </summary>
        private void resetList()
        {
            List<GameObject> tmpList = new List<GameObject>();//�폜���鍀�ڂ��ꎞ�I�ɕۑ�

            //�G���U���͈͂ł͂Ȃ��ꍇ�͍U���͈͂̓G����폜����
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

            foreach (var item in tmpList)//�ۑ��������ڂɏ]���č폜
            {
                inAttackRangeEnemy.Remove(item);
            }
            tmpList.Clear();

            //allEnemy��null���폜����
            allEnemy.RemoveAll(s => s == null);
        }
    }
}
