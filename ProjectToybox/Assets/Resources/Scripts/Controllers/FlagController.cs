using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    public static FlagController Instance;
    private Dictionary<string, bool> _flags;
    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        else Instance = this;
        
        _flags = new Dictionary<string, bool>();
    }

    public bool GetFlag(string flagName)
    {
        if (!_flags.ContainsKey(flagName))
            return false;

        return _flags[flagName];
    }

    public void SetFlag(string flagName, bool value = true)
    {
        if (_flags.ContainsKey(flagName))
            _flags[flagName] = value;
        else 
            _flags.Add(flagName, value);
    }
}
