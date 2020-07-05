using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife
{
    // totally not gonna use in prod lol
    internal static class ObjectPool<T> where T : class
    {
        private static ConcurrentDictionary<T, bool> _rentalPool = new ConcurrentDictionary<T, bool>();
        private static object _lock = new object();

        public static T Rent(Func<T> factory)
        {
            foreach (var kvp in _rentalPool)
            {
                if (!kvp.Value)
                {
                    lock (_lock)
                    {
                        _rentalPool[kvp.Key] = true;
                        return kvp.Key;
                    }
                }
            }

            var obj = factory();
            _rentalPool[obj] = true;
            return obj;
        }

        public static void Return(T obj)
        {
            _rentalPool[obj] = false;
        }
    }
}
