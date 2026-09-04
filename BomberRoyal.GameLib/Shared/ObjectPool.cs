using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Shared
{
    public class ObjectPool<T> where T : class 
    {
        private readonly Func<T> _factory;
        private readonly Stack<T> _pool = new();

        public ObjectPool(Func<T> factory, int InitialCapacity = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            for(int i = 0; i < InitialCapacity; i++)
            {
                _pool.Push(_factory());
            }

        }
        public T Get()
        {
            return _pool.Count > 0 ? _pool.Pop() : _factory();
        }
        public void Return(T obj)
        {
            _pool.Push(obj);
        }
        public int Count => _pool.Count;
    }
}
