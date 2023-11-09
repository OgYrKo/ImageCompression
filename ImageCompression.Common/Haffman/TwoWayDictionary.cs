using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.Haffman
{
    public class TwoWayDictionary<TKey, TValue>
    {
        public int Count => _keyToValue.Count;
        private Dictionary<TKey, TValue> _keyToValue;
        private Dictionary<TValue, TKey> _valueToKey;

        public TwoWayDictionary(IEqualityComparer<TValue> valueComparer)
        {
            _keyToValue = new Dictionary<TKey, TValue>();
            _valueToKey = new Dictionary<TValue, TKey>(valueComparer);
        }

        public void Add(TKey key, TValue value)
        {
            _keyToValue.Add(key,value);
            _valueToKey.Add(value, key);
        }

        public bool ContainsKey(TKey key)
        {
            return _keyToValue.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _valueToKey.ContainsKey(value);
        }

        public TValue GetValueByKey(TKey key)
        {
            return _keyToValue[key];
        }

        public TKey GetKeyByValue(TValue value)
        {
            return _valueToKey[value];
        }

        public void Update(TKey key, TValue value)
        {
            TValue oldValue = _keyToValue[key];
            _valueToKey.Remove(oldValue);

            _keyToValue[key] = value;
            _valueToKey[value] = key;
        }

    }

}
