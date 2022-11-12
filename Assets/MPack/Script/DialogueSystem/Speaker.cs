using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Dialogue/Speaker")]
    public class Speaker : ScriptableObject
    {
        public Sprite CharacterSprite;
        [LauguageID]
        public int NameLanguageID;
        public bool IsNPC;
    }
}