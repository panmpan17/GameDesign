using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/戰利品表")]
public class LootTable : ScriptableObject
{
    public LootRule[] LootRules;

    [System.Serializable]
    public struct LootRule
    {
        public ItemType Type;
        [Range(0, 1)]
        public float Chance;
    }
}