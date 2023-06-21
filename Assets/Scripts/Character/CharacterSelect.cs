using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unit
{
    public class CharacterSelect : MonoBehaviour, IPointerClickHandler
    {
        CharacterProfile MyCharacterProfile;
        CharacterMove characterMove;
        public void OnPointerClick(PointerEventData eventData)//�L�����N�^�[���N���b�N���ꂽ�ꍇ�ɑI����ԂɂȂ�
        {
            Debug.Log("�N���b�N����܂���");
            if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
            {
                Debug.Log("iSelect��true�ɂ��܂���");
                characterMove.iSelect(true);
            }
            StartCoroutine(SelectCancel());
        }

        void Start()
        {
            MyCharacterProfile = GetComponent<CharacterProfile>();
            characterMove = GetComponent<CharacterMove>();

        }
        public void MultSelected(bool value)
        {
            characterMove.iSelect(value);
        }

        IEnumerator SelectCancel()
        {
            while (true)
            {
                yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Mouse1));
                characterMove.iSelect(false);
                Debug.Log("iSelect��false�ɂ��܂���");
                yield return new WaitForSeconds(1f);
            }
        }
    }
}