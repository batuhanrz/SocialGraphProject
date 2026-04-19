using System;
using System.Collections.Generic; // Sadece IEnumerable icin, Dictionary kullanilmiyor
using System.Collections;

namespace SocialGraph.API.DataStructures
{
    // Custom Linear Probing Hash Table. (0=Bos, 1=Dolu, 2=Silinmis)
    public class CustomHashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int DefaultCapacity = 16;
        private const float LoadFactorThreshold = 0.75f;

        private TKey[] _keys;
        private TValue[] _values;
        private byte[] _states; 
        private int _count;
        private int _capacity;

        public int Count => _count;

        public CustomHashTable() : this(DefaultCapacity) { }

        public CustomHashTable(int capacity)
        {
            _capacity = capacity;
            _keys = new TKey[_capacity];
            _values = new TValue[_capacity];
            _states = new byte[_capacity];
        }

        private int Hash(TKey key)
        {
            return (Math.Abs(key.GetHashCode()) % _capacity);
        }

        public void Put(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (_count >= _capacity * LoadFactorThreshold)
            {
                Rehash();
            }

            int index = Hash(key);
            int firstDeleted = -1;

            while (true)
            {
                if (_states[index] == 0) // Bos slot bulundu
                {
                    int insertIndex = firstDeleted != -1 ? firstDeleted : index;
                    _keys[insertIndex] = key;
                    _values[insertIndex] = value;
                    _states[insertIndex] = 1;
                    _count++;
                    return;
                }
                else if (_states[index] == 1 && _keys[index].Equals(key)) // Guncelleme
                {
                    _values[index] = value;
                    return;
                }
                else if (_states[index] == 2 && firstDeleted == -1) // Ilk silinen slotun konumu
                {
                    firstDeleted = index;
                }

                index = (index + 1) % _capacity;
            }
        }

        public TValue Get(TKey key)
        {
            if (TryGetValue(key, out TValue val))
            {
                return val;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in custom hash table.");
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = Hash(key);

            for (int i = 0; i < _capacity; i++)
            {
                if (_states[index] == 0)
                {
                    value = default;
                    return false;
                }
                if (_states[index] == 1 && _keys[index].Equals(key))
                {
                    value = _values[index];
                    return true;
                }
                index = (index + 1) % _capacity;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = Hash(key);

            for (int i = 0; i < _capacity; i++)
            {
                if (_states[index] == 0) return false;
                if (_states[index] == 1 && _keys[index].Equals(key))
                {
                    _states[index] = 2; // Silindi isareti (tombstone)
                    _keys[index] = default;
                    _values[index] = default;
                    _count--;
                    return true;
                }
                index = (index + 1) % _capacity;
            }

            return false;
        }

        private void Rehash()
        {
            int newCapacity = _capacity * 2;
            var oldKeys = _keys;
            var oldValues = _values;
            var oldStates = _states;

            _capacity = newCapacity;
            _keys = new TKey[_capacity];
            _values = new TValue[_capacity];
            _states = new byte[_capacity];
            _count = 0;

            for (int i = 0; i < oldStates.Length; i++)
            {
                if (oldStates[i] == 1) // Gecerli verileri tasi
                {
                    Put(oldKeys[i], oldValues[i]);
                }
            }
        }

        public IEnumerable<TKey> Keys()
        {
            for (int i = 0; i < _capacity; i++)
            {
                if (_states[i] == 1)
                {
                    yield return _keys[i];
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < _capacity; i++)
            {
                if (_states[i] == 1)
                {
                    yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Test amaclidir
        public int GetCapacity() => _capacity;
    }
}
