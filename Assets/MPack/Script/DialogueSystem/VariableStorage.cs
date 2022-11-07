using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/變數儲存")]
    public class VariableStorage : ScriptableObject
    {
        public BooleanVaribleSet[] defaultBooleans;

        [System.NonSerialized]
        private Dictionary<string, int> m_integerVaribles;
        [System.NonSerialized]
        private Dictionary<string, float> m_floatVaribles;
        [System.NonSerialized]
        private Dictionary<string, bool> m_booleanVaribles;


        private Dictionary<string, int> _integerVaribles {
            get {
                if (m_integerVaribles == null)
                    m_integerVaribles = new Dictionary<string, int>();
                return m_integerVaribles;
            }
        }
        private Dictionary<string, float> _floatVaribles {
            get {
                if (m_floatVaribles == null)
                    m_floatVaribles = new Dictionary<string, float>();
                return m_floatVaribles;
            }
        }
        private Dictionary<string, bool> _booleanVaribles {
            get {
                if (m_booleanVaribles == null)
                    m_booleanVaribles = new Dictionary<string, bool>();
                return m_booleanVaribles;
            }
        }

        public void Initial()
        {
            for (int i = 0; i < defaultBooleans.Length; i++)
            {
                _booleanVaribles.Add(defaultBooleans[i].name, defaultBooleans[i].value);
            }
        }

        public void Set(string key, float floatValue)
        {
            if (_floatVaribles.ContainsKey(key)) _floatVaribles[key] = floatValue;
            else _floatVaribles.Add(key, floatValue);
        }
        public void Set(string key, int intValue)
        {
            if (_integerVaribles.ContainsKey(key)) _integerVaribles[key] = intValue;
            else _integerVaribles.Add(key, intValue);
        }
        public void Set(string key, bool booleanValue)
        {
            if (_booleanVaribles.ContainsKey(key)) _booleanVaribles[key] = booleanValue;
            else _booleanVaribles.Add(key, booleanValue);
        }

        public bool GetInt(string key, out int value)
        {
            return _integerVaribles.TryGetValue(key, out value);
        }

        public bool GetFloat(string key, out float value)
        {
            return _floatVaribles.TryGetValue(key, out value);
        }

        public bool GetBool(string key, out bool value)
        {
            return _booleanVaribles.TryGetValue(key, out value);
        }

        public void Remove(string key)
        {
            if (_booleanVaribles.Remove(key)) return;
            else if (_integerVaribles.Remove(key)) return;
            else if (_floatVaribles.Remove(key)) return;
        }

        [System.Serializable]
        public class BooleanVaribleSet
        {
            public string name;
            public bool value;
        }
    }
}