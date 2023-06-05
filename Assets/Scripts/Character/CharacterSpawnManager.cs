using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Unit
{
    public class CharacterSpawnManager : MonoBehaviour
    {
        PlayerDeck Deck;
        [SerializeField]
        private GameObject[] Spanwer;
        void Awake()
        {
            Deck = FindObjectOfType<PlayerDeck>();
            Spawn();
        }

        private void Spawn()
        {
            for (int i = 0; i < Deck.MyPlayerCharacterDeck.Length; i++)
            {
                CharacterDataList Data = Resources.Load<CharacterDataList>("CharacterDataList");
                //Debug.Log(Data);
                Spanwer[i].GetComponent<CharacterSpawner>().Init(Data.characterDataList[Deck.MyPlayerCharacterDeck[i]]);
            }
        }
    }
}