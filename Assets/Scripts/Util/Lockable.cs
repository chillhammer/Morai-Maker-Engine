using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class Lockable : MonoBehaviour
    {
        public bool IsLocked { get { return locks.Count > 0; } }

        private Dictionary<object, int> locks;

        protected virtual void Awake()
        {
            locks = new Dictionary<object, int>();
        }
        
        public void AddLock(object key)
        {
            if(locks.ContainsKey(key))
            {
                locks[key] += 1;
            }
            else
            {
                locks[key] = 1;
            }
        }

        public void RemoveLock(object key)
        {
            if(locks.ContainsKey(key))
            {
                if(locks[key] > 1)
                    locks[key] -= 1;
                else
                    locks.Remove(key);
            }
        }
    }
}