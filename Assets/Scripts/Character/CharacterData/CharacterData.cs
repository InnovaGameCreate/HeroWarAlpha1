using UnityEngine;
namespace Unit
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CreaterData")]

    public class CharacterData : ScriptableObject
    {
        public int characterID;
        public string characterName = "ƒLƒƒƒ‰ƒNƒ^[ƒl[ƒ€";
        public UnitType UnitType = UnitType.AR;
        public float hp = 100;
        public int recoverySpeed = 5;
        public float moveSpeed = 6f;
        public float attackRange = 200;
        public float attackPower = 30;
        public float armor = 0;
        public float moveHitRate = 60;
        public float staticHitRate = 90;
        public float reloadSpeed = 2f;
        public float searchRange = 150;
    }
}