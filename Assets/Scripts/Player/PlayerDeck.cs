using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [SerializeField]
    private int[] PlayerCharacterDeck;

    public int[] MyPlayerCharacterDeck { get => PlayerCharacterDeck; set => PlayerCharacterDeck = value; }
}
