using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto
{
    public class PooledFX : MonoBehaviour, IPooledObject
    {
        protected string _name;

        protected float _duration;

        public string Name
        {
            get => _name; 
            set => _name = value;
        }

        public virtual void Update()
        {
            if (_duration > 0f)
            {
                _duration -= Time.deltaTime;
            }
            else
            {
                _duration = 9999f;
                Dispose();
            }
        }


        public virtual void Dispose()
        {
            ObjectPoolController.Self.Dispose(this);
        }
        
        public virtual void Initialize(float duration)
        {
            _duration = duration;
        }
    }
}