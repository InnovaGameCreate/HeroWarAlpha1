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
        public void OnPointerClick(PointerEventData eventData)//キャラクターがクリックされた場合に選択状態になる
        {
            Debug.Log("クリックされました");
            if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
            {
                Debug.Log("iSelectをtrueにしました");
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
                Debug.Log("iSelectをfalseにしました");
                yield return new WaitForSeconds(1f);
            }
        }
    }
}