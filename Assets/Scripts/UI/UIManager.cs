using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    UIScoreElement element;
    Dictionary<ScoreObject, List<UIScoreElement>> _scoreUi = new Dictionary<ScoreObject, List<UIScoreElement>>();
    [SerializeField] UIScoreElement _prefab;

    /*private void Start()
    {
        scoreManager.scoreArray[(int)Camp.A].Subscribe(e => UpdateScoreSlider(e, Camp.A));  //�E��(A)�̃X�R�A���Ď���UI���X�V����
        scoreManager.scoreArray[(int)Camp.B].Subscribe(e => UpdateScoreSlider(e, Camp.B));   //�E��(B)�̃X�R�A���Ď���UI���X�V����

        timer.second.Subscribe(e => UpdateTimerText());  //�E��(B)�̃X�R�A���Ď���UI���X�V����
    }*/
    private void Awake()
    {
        element = Instantiate(_prefab, transform);  //�X�R�A�o�[�̐���
    }

    public void OnScoreChanged(ScoreObject scoreObject, float score, Camp camp)    //�X�R�A���ς������
    {
        
        if (_scoreUi.ContainsKey(scoreObject) == false)  //scoreObject�Ƃ����L�[�����邩�ǂ���
        {
            return; //������Δ�����
        }
        var list = _scoreUi[scoreObject];
        
        foreach (var item in list)
        {
            item.UpdateScoreSlider(score, camp);
        }
    }


    public void OnPlayerJoined(ScoreObject scoreObject)
    {
        if (_scoreUi.ContainsKey(scoreObject) == false)
        {
            _scoreUi.Add(scoreObject, new List<UIScoreElement>());
        }
        _scoreUi[scoreObject].Add(element); //�e�v���C���[�ƃX�R�A�o�[��R�Â���
    }

    public void OnPlayerLeft(ScoreObject scoreObject)
    {
        if (_scoreUi.ContainsKey(scoreObject))
        {
            foreach (var item in _scoreUi[scoreObject])
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            _scoreUi[scoreObject].Clear();
        }
    }

    /*
    private void Update()
    {
        element.UpdateTimerText();
    }
    */
}
