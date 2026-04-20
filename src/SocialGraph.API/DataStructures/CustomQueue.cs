using System;

namespace SocialGraph.API.DataStructures
{
    /// <summary>
    /// Dairesel dizi (circular array) tabanlı, thread-safe kuyruk yapısı.
    /// </summary>
    /// <typeparam name="T">Type of elements in the queue.</typeparam>
    public class CustomQueue<T>
    {
        
        private T[] _items;
        private int _head;
        private int _tail;
        private int _count;
        private int _capacity;
        private readonly object _lockObj = new object();

        public CustomQueue(int initialCapacity = 16)
        {
            if (initialCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));

            _capacity = initialCapacity;
            _items = new T[_capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        /// <summary>
        /// Kuyruktaki eleman sayısı.
        /// Karmaşıklık: O(1)
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lockObj)
                {
                    return _count;
                }
            }
        }

        /// <summary>
        /// Kuyruğun boş olup olmadığını döndürür.
        /// Karmaşıklık: O(1)
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                lock (_lockObj)
                {
                    return _count == 0;
                }
            }
        }

        /// <summary>
        /// Kuyruğun sonuna eleman ekler. Kapasite dolarsa boyutu 2 katına çıkarır.
        /// Karmaşıklık: O(1) amortized
        /// </summary>
        public void Enqueue(T item)
        {
            lock (_lockObj)
            {
                if (_count == _capacity)
                {
                    Resize();
                }

                _items[_tail] = item;
                _tail = (_tail + 1) % _capacity;
                _count++;
            }
        }

        /// <summary>
        /// Kuyruğun başındaki elemanı çıkarır ve döndürür.
        /// Karmaşıklık: O(1)
        /// </summary>
        public T Dequeue()
        {
            lock (_lockObj)
            {
                if (_count == 0)
                {
                    throw new InvalidOperationException("Queue is empty.");
                }

                T item = _items[_head];
                _items[_head] = default(T); // Clear reference
                _head = (_head + 1) % _capacity;
                _count--;
                return item;
            }
        }

        /// <summary>
        /// Kuyruğun başındaki elemanı silmeden döndürür.
        /// Karmaşıklık: O(1)
        /// </summary>
        public T Peek()
        {
            lock (_lockObj)
            {
                if (_count == 0)
                {
                    throw new InvalidOperationException("Queue is empty.");
                }

                return _items[_head];
            }
        }

        private void Resize()
        {
            int newCapacity = _capacity * 2;
            T[] newArray = new T[newCapacity];

            if (_head < _tail)
            {
                Array.Copy(_items, _head, newArray, 0, _count);
            }
            else
            {
                Array.Copy(_items, _head, newArray, 0, _capacity - _head);
                Array.Copy(_items, 0, newArray, _capacity - _head, _tail);
            }

            _items = newArray;
            _head = 0;
            _tail = _count;
            _capacity = newCapacity;
        }
    }
}
