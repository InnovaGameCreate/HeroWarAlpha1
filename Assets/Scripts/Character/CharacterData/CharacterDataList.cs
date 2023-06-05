using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    [CreateAssetMenu(menuName = "Create CharacterDataList", fileName = "CharacterDataList")]
    public class CharacterDataList : ScriptableObject
    {
        public List<CharacterData> characterDataList;
    }
}
