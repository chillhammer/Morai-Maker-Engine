using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class Lockable : MonoBehaviour
    {
        public bool IsLocked { get { return locks.Count > 0; } }

        private List<object> locks;

        protected virtual void Awake()
        {
            locks = new List<object>();
        }
        
        public void AddLock(object key)
        {
            locks.Add(key);
        }

        public void RemoveLock(object key)
        {
            locks.Remove(key);
        }
    }
}