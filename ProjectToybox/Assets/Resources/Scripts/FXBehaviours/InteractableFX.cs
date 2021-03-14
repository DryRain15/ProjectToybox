using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class InteractableFX : MonoBehaviour, IPooledObject 
{
    protected string _name;

    public string Name
    {
        get => _name; 
        set => _name = value;
    }

    public void Dispose()
    {
        ObjectPoolController.Self.Dispose(this);
    }
}
