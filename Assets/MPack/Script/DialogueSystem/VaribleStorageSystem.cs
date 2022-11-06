using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [System.Serializable]
    public class VaribleStorageSystem
    {
        public BooleanVaribleSet[] defaultBooleans;

        private Dictionary<string, int> m_integerVaribles;
        private Dictionary<string, float> m_floatVaribles;
        private Dictionary<string, bool> m_booleanVaribles;

        public VaribleStorageSystem()
        {
            m_integerVaribles = new Dictionary<string, int>();
            m_floatVaribles = new Dictionary<string, float>();
            m_booleanVaribles = new Dictionary<string, bool>();
        }

        public void Initial()
        {
            for (int i = 0; i < defaultBooleans.Length; i++)
            {
                m_booleanVaribles.Add(defaultBooleans[i].name, defaultBooleans[i].value);
            }
        }

        public void Set(string key, float floatValue)
        {
            if (m_floatVaribles.ContainsKey(key)) m_floatVaribles[key] = floatValue;
            else m_floatVaribles.Add(key, floatValue);
        }
        public void Set(string key, int intValue)
        {
            if (m_integerVaribles.ContainsKey(key)) m_integerVaribles[key] = intValue;
            else m_integerVaribles.Add(key, intValue);
        }
        public void Set(string key, bool booleanValue)
        {
            if (m_booleanVaribles.ContainsKey(key)) m_booleanVaribles[key] = booleanValue;
            else m_booleanVaribles.Add(key, booleanValue);
        }

        public bool GetInt(string key, out int value)
        {
            return m_integerVaribles.TryGetValue(key, out value);
        }

        public bool GetFloat(string key, out float value)
        {
            return m_floatVaribles.TryGetValue(key, out value);
        }

        public bool GetBool(string key, out bool value)
        {
            return m_booleanVaribles.TryGetValue(key, out value);
        }

        public void Remove(string key)
        {
            if (m_booleanVaribles.Remove(key)) return;
            else if (m_integerVaribles.Remove(key)) return;
            else if (m_floatVaribles.Remove(key)) return;
        }

        [System.Serializable]
        public class BooleanVaribleSet
        {
            public string name;
            public bool value;
        }
    }
}