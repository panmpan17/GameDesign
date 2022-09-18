using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Loot Table")]
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