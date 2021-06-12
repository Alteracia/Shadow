using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Alteracia.Patterns
{
    public class Collector<T> : MonoBehaviour where T : UnityEngine.Object
    {
        private List<T> _collection;

        public void CollectFromScene(Func<T, bool> predicate)
        {
            CollectFromScene(true, predicate);
        }

        public void CollectFromScene(bool rewrite = true, Func<T, bool> predicate = null)
        {
            var range = predicate != null ? FindObjectsOfType<T>().Where(predicate) : FindObjectsOfType<T>();
            
            if (range == null) return;
            
            if (rewrite || _collection == null)
            {
                _collection?.Clear();
                _collection = range.ToList();
            }
            else
                _collection?.AddRange(range.ToList());
        }

        public T GetInIndex(int index)
        {
            if (index < 0 || index > _collection.Count - 1) return null;
            return _collection[index];
        }

        public void ExecuteForEach(UnityAction<T> action)
        {
            foreach (var obj in _collection)
            {
                action.Invoke(obj);
            }
        }
    }
}
