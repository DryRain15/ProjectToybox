using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto
{
    public struct PoolParameters
    {
        private Vector2 _position;
        public Vector2 Position => _position;
        
        private Vector2 _direction;
        public Vector2 Direction => _direction;

        private Transform _parent;
        public Transform Parent => _parent;

        public PoolParameters(Vector2 pos, Vector2 dir, Transform parent = null)
        {
            _position = pos;
            _direction = dir;
            _parent = parent;
        }
        
        public PoolParameters(Vector2 pos)
        {
            _position = pos;
            _direction = Vector2.up;
            _parent = null;
        }
    } 
    public interface IPooledObject
    {
        string Name { get; set; }
        GameObject gameObject { get; }

        void Dispose();
    }

}