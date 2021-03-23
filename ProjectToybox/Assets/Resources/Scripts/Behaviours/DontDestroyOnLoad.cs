using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto.Behaviours
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
