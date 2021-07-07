using System;
using System.Collections.Generic;
using System.Data;

namespace Cache
{
    class Program
    {
        public class Cache<T>
        {
            private Dictionary<string, ValueTuple<T, DateTime>> _map;
            private uint _capacity;
            private TimeSpan _lifeеimeRecords;

            public Cache(uint inputCapacity, TimeSpan inputLifeеimeRecords)
            {
                _map = new Dictionary<string, ValueTuple<T, DateTime>>();
                _capacity = inputCapacity;
                _lifeеimeRecords = inputLifeеimeRecords;
            }

            private void RefreshLifetime()
            {
                DateTime currentTime = DateTime.Now;
                foreach (var (key, value) in _map)
                    if (currentTime - value.Item2 >= _lifeеimeRecords)
                    {
                        _map.Remove(key);
                    }
            }

            public void Save(string key, T value)
            {
                if (value == null) 
                    throw new ArgumentNullException(nameof(value));
                if (_map.TryGetValue(key, out _))
                    throw new ArgumentException("Key already in cache!");
                
                if (_map.Count == _capacity)
                {
                    string oldestKey = null;
                    DateTime currentTime = DateTime.Now;
                    TimeSpan maxLifetimeRecord = TimeSpan.Zero;
                    
                    foreach (var (itemString, (_, dateTime)) in _map)
                    {
                        if (maxLifetimeRecord < currentTime - dateTime)
                        {
                            maxLifetimeRecord = dateTime - currentTime;
                            oldestKey = itemString;
                        }
                    }

                    if (oldestKey == null)
                        throw new DataException("Something go wrong...");
                    _map.Remove(oldestKey);
                    _map.Add(key, ValueTuple.Create(value, currentTime));
                }
                else
                {
                    DateTime currentTime = DateTime.Now;
                    _map.Add(key, ValueTuple.Create(value, currentTime));
                }

                RefreshLifetime();
            }

            public T Get(string key)
            {
                ValueTuple<T, DateTime> returnedValue;
                if (key == null) 
                    throw new ArgumentNullException(nameof(key));
                if (!_map.TryGetValue(key, out returnedValue))
                    throw new KeyNotFoundException("Key not found!");
                
                RefreshLifetime();
                return returnedValue.Item1;
            }
        }
        static void Main(string[] args)
        {
            Cache<int> myCache = new Cache<int>(2, new TimeSpan(0, 0, 10));
            myCache.Save("firstValue", 12);
            myCache.Save("secondValue", 14);
            myCache.Save("thirdValue", 666);

            Console.WriteLine(myCache.Get("thirdValue"));
            
            /* Exceptions same keys */
            myCache.Save("fourthValue", 14);
            myCache.Save("fourthValue", 14);
            
            /* Exceptions not found key */
            myCache.Get("something wrong key");
        }
    }
}