using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectController : MonoBehaviour
{
    public static FieldObjectController Instance;
    public static Dictionary<string, IFieldObject> FOs;
    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        else Instance = this;
        
        FOs = new Dictionary<string, IFieldObject>();
    }

    
}
