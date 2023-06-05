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
            if (MyCharacterProfile.GetCharacterOwnerType() == OwnerType.Player)
            {
                characterMove.iSelect(true);
            }
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
    }
}