using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldObject
{
    string Name { get; set; }
    GameObject GameObject { get; }
    
    Transform Transform { get; }
    Vector3 Position { get; set; }
}
