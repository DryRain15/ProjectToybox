using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum KeyType
{
    None,
    Use,
    Cancel,
    Dash,
    Debug,
    Menu,
}

public enum InputType
{
    None,
    WASD,
    Arrow,
    PointClick
}

public class GlobalInputController : MonoBehaviour
{
    public static GlobalInputController Instance;

    private bool _getControl = true;
    public bool GetControl { get => _getControl; }

    public float hInput;
    public float vInput;

    public void RemoveControl()
    {
        _getControl = false;
    }

    public void RestoreControl()
    {
        _getControl = true;
    }

    #region KeyAssign

    public InputType InputType { get; set; } = InputType.None;

    public KeyCode UseKey { get; set; }
    public KeyCode CancelKey { get; set; }
    public KeyCode DashKey { get; set; }
    public KeyCode DebugKey { get; set; }
    public KeyCode MenuKey { get; set; }

    public KeyCode LastKey;

    #endregion

    #region KeyDownEvent

    public bool useKeyDown;
    public bool cancelKeyDown;
    public bool dashKeyDown;
    public bool debugKeyDown;
    public bool menuKeyDown;

    #endregion

    #region KeyEvent

    public bool useKey;
    public bool cancelKey;
    public bool dashKey;
    public bool debugKey;
    public bool menuKey;

    #endregion
    
    #region KeyReleaseEvent

    public bool useKeyRelease;
    public bool cancelKeyRelease;
    public bool dashKeyRelease;
    public bool debugKeyRelease;
    public bool menuKeyRelease;
    
    #endregion

    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            // AssignDefault();
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        hInput = 0f;
        vInput = 0f;
    }

    private void AssignDefault()
    {
        Debug.Log("Assigning default key settings");
        InputType = InputType.WASD;
        AssignKey(KeyType.Use, KeyCode.J);
        AssignKey(KeyType.Dash, KeyCode.L);
        AssignKey(KeyType.Cancel, KeyCode.K);
        AssignKey(KeyType.Debug, KeyCode.Tab);
        AssignKey(KeyType.Menu, KeyCode.Escape);
    }

    // Update is called once per frame
    void Update()
    {
        ResetKeyDown();
        ResetKeyRelease();

        hInput = 0f;
        vInput = 0f;
        
        if (!_getControl) return;
        
        foreach (var targetKey in GetKeyCodes(InputType))
        {
            if (Input.GetKey(targetKey))
            {
                var data = FindInputTypeWithIndex(targetKey);
                switch (data.Value)
                {
                    case 0:
                        vInput = 1f;
                        break;
                    case 1:
                        hInput = -1f;
                        break;
                    case 2:
                        vInput = -1f;
                        break;
                    case 3:
                        hInput = 1f;
                        break;

                }
            }
        }

        if (CheckKeyAssigned(KeyType.Use))
        {
            if (Input.GetKeyDown(UseKey))
            {
                useKeyDown = true;
                useKey = true;
            }
            if (Input.GetKeyUp(UseKey))
            {
                useKey = false;
                useKeyRelease = true;
            }
        }

        if (CheckKeyAssigned(KeyType.Dash))
        {
            if (Input.GetKeyDown(DashKey))
            {
                dashKeyDown = true;
                dashKey = true;
            }
            if (Input.GetKeyUp(DashKey))
            {
                dashKey = false;
                dashKeyRelease = true;
            }
        }

        if (CheckKeyAssigned(KeyType.Cancel))
        {
            if (Input.GetKeyDown(CancelKey))
            {
                cancelKeyDown = true;
                cancelKey = true;
            }
            if (Input.GetKeyUp(CancelKey))
            {
                cancelKey = false;
                cancelKeyRelease = true;
            }
        }

        if (CheckKeyAssigned(KeyType.Debug))
        {
            if (Input.GetKeyDown(DebugKey))
            {
                debugKeyDown = true;
                debugKey = true;
            }
            if (Input.GetKeyUp(DebugKey))
            {
                debugKey = false;
                debugKeyRelease = true;
            }
        }

        if (CheckKeyAssigned(KeyType.Menu))
        {
            if (Input.GetKeyDown(MenuKey))
            {
                menuKeyDown = true;
                menuKey = true;
            }
            if (Input.GetKeyUp(MenuKey))
            {
                menuKey = false;
                menuKeyRelease = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventParameter param = new EventParameter(
                "SpaceKey".EventParameterPairing(true), 
                "EnterKey".EventParameterPairing(true)); 
            EventController.Instance.EventCall("DebugEvent", param);
        }
    }

    private void ResetKeyDown()
    {
        useKeyDown = false;
        cancelKeyDown = false;
        dashKeyDown = false;
        debugKeyDown = false;
        menuKeyDown = false;
    }

    private void ResetKeyRelease()
    {
        useKeyRelease = false;
        cancelKeyRelease = false;
        dashKeyRelease = false;
        debugKeyRelease = false;
        menuKeyRelease = false;
    }

    public KeyCode GetKeyCode(KeyType type)
    {
        switch (type)
        {
            case KeyType.Use:
                return UseKey;
            case KeyType.Dash:
                return DashKey;
            case KeyType.Cancel:
                return CancelKey;
            case KeyType.Debug:
                return DebugKey;
            case KeyType.Menu:
                return MenuKey;
        }

        return KeyCode.None;
    }

    public bool CheckKeyAssigned(KeyType type)
    {
        if (GetKeyCode(type) != KeyCode.None)
            return true;

        print("key " + type + " not assigned yet!");
        return false;
    }

    public bool IsAssignedKey(KeyCode key)
    {
        if (UseKey == key) return true;
        if (DashKey == key) return true;
        if (CancelKey == key) return true;
        if (DebugKey == key) return true;
        if (MenuKey == key) return true;
        return false;
    }

    public void AssignKey(KeyType type, KeyCode key)
    {
        if (IsAssignedKey(key) && key != KeyCode.None)
        {
            Debug.Log(key + " is already assigned key!");
            return;
        }
        switch (type)
        {
            case KeyType.Use:
                UseKey = key;
                break;
            case KeyType.Dash:
                DashKey = key;
                break;
            case KeyType.Cancel:
                CancelKey = key;
                break;
            case KeyType.Debug:
                DebugKey = key;
                break;
            case KeyType.Menu:
                MenuKey = key;
                break;
        }
        
        print("assigned " + type + " as " + key);
    }

    void OnGUI()
    {
        if (Event.current.isKey && 
            Event.current.type == EventType.KeyDown &&
            Event.current.keyCode != KeyCode.None)
        {
            LastKey = Event.current.keyCode;
            // Debug.Log(LastKey);
        }
    }

    public static InputType FindInputType(KeyCode key)
    {
        return FindInputTypeWithIndex(key).Key;
    }

    public static KeyValuePair<InputType, int> FindInputTypeWithIndex(KeyCode key)
    {
        var wasd = new[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        var arrow = new[] { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow };
        
        if (wasd.Contains(key)) return new KeyValuePair<InputType, int>(InputType.WASD, Array.IndexOf(wasd, key));
        if (arrow.Contains(key)) return new KeyValuePair<InputType, int>(InputType.Arrow, Array.IndexOf(arrow, key));
        return new KeyValuePair<InputType, int>(InputType.None, -1);
    }

    public static KeyCode[] GetKeyCodes(InputType type)
    {
        var wasd = new[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        var arrow = new[] { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow };
        return type switch
        {
            InputType.WASD => wasd,
            InputType.Arrow => arrow,
            _ => new KeyCode[0]
        };
    }
}
