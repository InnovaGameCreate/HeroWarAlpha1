using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    [CreateAssetMenu(menuName = "Create UnitIcon", fileName = "UnitIcon")]
    public class UnitIcon : ScriptableObject
    {
        public List<Sprite> unitIconList;
    }
}