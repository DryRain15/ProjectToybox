using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto
{
    public class ObjectPoolController : MonoBehaviour
    {
        public static ObjectPoolController Self;
        private Dictionary<string, ObjectPool> _poolList;

        private void Awake()
        {
            if(Self != null) Destroy(gameObject);
            else Self = this;
            
            _poolList = new Dictionary<string, ObjectPool>();

            var prefabs = Resources.LoadAll<GameObject>("Prefabs");

            foreach (var prefab in prefabs)
            {
                var poolObj = new GameObject(prefab.name);
                poolObj.transform.SetParent(transform);
                var pool = poolObj.AddComponent<ObjectPool>().Initialize(prefab.name, prefab);
                _poolList.Add(prefab.name, pool);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        private void Update()
        {

        }

        public IPooledObject Instantiate(string poolName, PoolParameters param)
        {
            return _poolList[poolName].Instantiate(param);
        }

        public void Dispose(IPooledObject obj)
        {
            _poolList[obj.Name].Dispose(obj);
        }

        public void DisposeAll(string objName)
        {
            _poolList[objName].DisposeAll();
        }
    }

}
